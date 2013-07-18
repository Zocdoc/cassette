using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Cassette.DependencyGraphInteration;

#if NET35
using Cassette.Utilities;
#endif

namespace Cassette
{
    /// <summary>
    /// Provides access to the current Cassette application object.
    /// </summary>
    public static class CassetteApplicationContainer
    {
        static Func<ICassetteApplication> _getApplication;

        /// <summary>
        /// Gets the current <see cref="ICassetteApplication"/> used by Cassette.
        /// </summary>
        public static ICassetteApplication Application
        {
            get { return _getApplication(); }
        }

        /// <summary>
        /// Sets the function used to access the current Cassette application object.
        /// Unit tests can use this method to assign a stub application for testing purposes.
        /// </summary>
        public static void SetApplicationAccessor(Func<ICassetteApplication> getApplication)
        {
            if (getApplication == null) throw new ArgumentNullException("getApplication");
            _getApplication = getApplication;
        }

        internal static event Action Shutdown = delegate { };

        internal static void TriggerShutdown()
        {
            Shutdown();
        }
    }

    public class CassetteApplicationContainer<T> : ICassetteApplicationContainer<T>
        where T : ICassetteApplication
    {
        readonly Func<T> createApplication;
        FileSystemWatcher watcher;
        Lazy<T> application;
        bool creationFailed;
        readonly List<Regex> ignorePaths = new List<Regex>();

        public CassetteApplicationContainer(Func<T> createApplication)
        {
            this.createApplication = createApplication;
            application = new Lazy<T>(CreateApplication);
        }

        public T Application
        {
            get
            {
                return application.Value;
            }
        }

        public void CreateNewApplicationWhenFileSystemChanges(string rootDirectoryToWatch, IDependencyGraphInteractionFactory factory)
        {
            // In production mode we don't expect the asset files to change
            // while the application is running. Changes to assets will involve a 
            // re-deployment and restart of the app pool. So new assets are loaded then.

            // In development mode, asset files will likely change while application is
            // running. So watch the file system and recycle the application object 
            // when files are created/changed/deleted/etc.
            watcher = new FileSystemWatcher(rootDirectoryToWatch)
            {
                IncludeSubdirectories = true,
                EnableRaisingEvents = true
            };

            watcher.Created += (sender, e) => HandleFileSystemChange(sender, e, factory);
            watcher.Changed += (sender, e) => HandleFileSystemChange(sender, e, factory);
            watcher.Renamed += (sender, e) => HandleFileSystemChange(sender, e, factory);
            watcher.Deleted += (sender, e) => HandleFileSystemChange(sender, e, factory);
        }

        void HandleFileSystemChange(object sender, FileSystemEventArgs e, IDependencyGraphInteractionFactory factory)
        {
            if (ShouldRecycleOnFileSystemChange(e.FullPath))
            {
                RecycleApplication(factory);
            }
        }

        public void IgnoreFileSystemChange(Regex pathRegex)
        {
            ignorePaths.Add(pathRegex);
        }

        bool ShouldRecycleOnFileSystemChange(string path)
        {
            return !ignorePaths.Any(regex => regex.IsMatch(path));
        }

        public void RecycleApplication(IDependencyGraphInteractionFactory factory)
        {
            if (IsPendingCreation) return; // Already recycled, awaiting first creation.

            lock (this)
            {
                if (IsPendingCreation) return;

                if (creationFailed)
                {
                    creationFailed = false;
                }
                else
                {
                    application.Value.Dispose();
                }
                // Re-create the lazy object. So the application isn't created until it's asked for.
                application = new Lazy<T>(() =>
                {
                    var app = CreateApplication();
                    factory.SetCassetteApplication(app);
                    app.SetDependencyInteractionFactory(factory);
                    return app;
                });
            }
        }

        bool IsPendingCreation
        {
            get { return creationFailed == false && application.IsValueCreated == false; }
        }

        T CreateApplication()
        {
            try
            {
                var app = createApplication();
                creationFailed = false;
                return app;
            }
            catch(Exception e)
            {
                creationFailed = true;
                throw e; 
            }
        }

        public void Dispose()
        {
            if (watcher != null)
            {
                watcher.Dispose();
            }
            if (application.IsValueCreated)
            {
                application.Value.Dispose();
            }
        }
    }
}