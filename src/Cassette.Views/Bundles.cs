using System;
using System.Collections.Generic;
using System.Linq;
using Cassette.DependencyGraphInteration;
using Cassette.HtmlTemplates;
using Cassette.Scripts;
using Cassette.Stylesheets;
#if !NET35
using System.Web;
#endif

namespace Cassette.Views
{
    /// <summary>
    /// Cassette API facade used by views to reference bundles and render the required HTML elements.
    /// </summary>
    /// <remarks>
    /// Keep overloads to stay compatible with .NET 3.5 (otherwise consumers will get exceptions)
    /// </remarks>
    public static class Bundles
    {
        /// <summary>
        /// Adds a reference to a bundle for the current page.
        /// </summary>
        /// <param name="assetPathOrBundlePathOrUrl">Either an application relative path to an asset file or bundle. Or a URL of an external resource.</param>
        public static void Reference(string assetPathOrBundlePathOrUrl)
        {
            Reference(assetPathOrBundlePathOrUrl, null);
        }

        /// <summary>
        /// Adds a reference to a bundle for the current page.
        /// </summary>
        /// <param name="assetPathOrBundlePathOrUrl">Either an application relative path to an asset file or bundle. Or a URL of an external resource.</param>
        /// <param name="pageLocation">The optional page location of the referenced bundle. This controls where it will be rendered.</param>
        public static void Reference(string assetPathOrBundlePathOrUrl, string pageLocation)
        {
            ExternalGraphInteraction.ReferenceBundle(assetPathOrBundlePathOrUrl, pageLocation);
        }

        /// <summary>
        /// Adds a page reference to an inline JavaScript block.
        /// </summary>
        /// <param name="scriptContent">The JavaScript code.</param>
        public static void AddInlineScript(string scriptContent)
        {
            AddInlineScript(scriptContent, null, null);
        }

        /// <summary>
        /// Adds a page reference to an inline JavaScript block.
        /// </summary>
        /// <param name="scriptContent">The JavaScript code.</param>
        /// <param name="pageLocation">The optional page location of the script. This controls where it will be rendered.</param>
        public static void AddInlineScript(string scriptContent, string pageLocation) {
            AddInlineScript(scriptContent, pageLocation, null);
        }

        /// <summary>
        /// Adds a page reference to an inline JavaScript block.
        /// </summary>
        /// <param name="scriptContent">The JavaScript code.</param>
        /// <param name="pageLocation">The optional page location of the script. This controls where it will be rendered.</param>
        /// <param name="customizeBundle">The optional delegate used to customize the created bundle before adding it to the collection.</param>
        public static void AddInlineScript(string scriptContent, string pageLocation, Action<ScriptBundle> customizeBundle)
        {
            var bundle = new InlineScriptBundle(scriptContent);

            AddScriptBundle(bundle, pageLocation, customizeBundle);
        }

        /// <summary>
        /// Adds a page reference to an inline JavaScript block.
        /// </summary>
        /// <param name="scriptContent">The Razor template for the Javascript code.</param>        
        /// <code lang="CS">
        /// @{
        ///   Bundles.AddInlineScript(@&lt;text&gt;
        ///     var foo = "Hello World";
        ///     alert( foo );&lt;/text&gt;);
        /// }
        /// </code>
        public static void AddInlineScript(Func<object, object> scriptContent)
        {
            AddInlineScript(scriptContent, null, null);
        }

        /// <summary>
        /// Adds a page reference to an inline JavaScript block.
        /// </summary>
        /// <param name="scriptContent">The Razor template for the Javascript code.</param>     
        /// <param name="pageLocation">The optional page location of the script. This controls where it will be rendered.</param>   
        /// <code lang="CS">
        /// @{
        ///   Bundles.AddInlineScript(@&lt;text&gt;
        ///     var foo = "Hello World";
        ///     alert( foo );&lt;/text&gt;);
        /// }
        /// </code>
        public static void AddInlineScript(Func<object, object> scriptContent, string pageLocation) {
            AddInlineScript(scriptContent, pageLocation, null);
        }

        /// <summary>
        /// Adds a page reference to an inline JavaScript block.
        /// </summary>
        /// <param name="scriptContent">The Razor template for the Javascript code.</param>
        /// <param name="pageLocation">The optional page location of the script. This controls where it will be rendered.</param>
        /// <param name="customizeBundle">The optional delegate used to customize the created bundle before adding it to the collection.</param>
        /// <code lang="CS">
        /// @{
        ///   Bundles.AddInlineScript(@&lt;text&gt;
        ///     var foo = "Hello World";
        ///     alert( foo );&lt;/text&gt;);
        /// }
        /// </code>
        public static void AddInlineScript(Func<object, object> scriptContent, string pageLocation, Action<ScriptBundle> customizeBundle)
        {
            AddInlineScript(scriptContent(null).ToString(), pageLocation, customizeBundle);
        }

        /// <summary>
        /// Add a page reference to a script that initializes a global JavaScript variable with the given data.
        /// </summary>
        /// <param name="globalVariable">The name of the global JavaScript variable to assign.</param>
        /// <param name="data">The data object, serialized into JSON.</param>        
        public static void AddPageData(string globalVariable, object data)
        {
            AddPageData(globalVariable, data, null, null);
        }

        /// <summary>
        /// Add a page reference to a script that initializes a global JavaScript variable with the given data.
        /// </summary>
        /// <param name="globalVariable">The name of the global JavaScript variable to assign.</param>
        /// <param name="data">The data object, serialized into JSON.</param>        
        /// <param name="pageLocation">The optional page location of the script. This controls where it will be rendered.</param>
        public static void AddPageData(string globalVariable, object data, string pageLocation) {
            AddPageData(globalVariable, data, pageLocation, null);
        }

        /// <summary>
        /// Add a page reference to a script that initializes a global JavaScript variable with the given data.
        /// </summary>
        /// <param name="globalVariable">The name of the global JavaScript variable to assign.</param>
        /// <param name="data">The data object, serialized into JSON.</param>
        /// <param name="pageLocation">The optional page location of the script. This controls where it will be rendered.</param>
        /// <param name="customizeBundle">The optional delegate used to customize the created bundle before adding it to the collection.</param>
        public static void AddPageData(string globalVariable, object data, string pageLocation, Action<ScriptBundle> customizeBundle)
        {
            AddScriptBundle(new PageDataScriptBundle(globalVariable, data), pageLocation, customizeBundle);
        }

        /// <summary>
        /// Add a page reference to a script that initializes a global JavaScript variable with the given data.
        /// </summary>
        /// <param name="globalVariable">The name of the global JavaScript variable to assign.</param>
        /// <param name="data">The dictionary of data, serialized into JSON.</param>        
        public static void AddPageData(string globalVariable, IEnumerable<KeyValuePair<string, object>> data)
        {
            AddPageData(globalVariable, data, null, null);
        }

        /// <summary>
        /// Add a page reference to a script that initializes a global JavaScript variable with the given data.
        /// </summary>
        /// <param name="globalVariable">The name of the global JavaScript variable to assign.</param>
        /// <param name="data">The dictionary of data, serialized into JSON.</param>        
        /// <param name="pageLocation">The optional page location of the script. This controls where it will be rendered.</param>
        public static void AddPageData(string globalVariable, IEnumerable<KeyValuePair<string, object>> data, string pageLocation) {
            AddPageData(globalVariable, data, pageLocation, null);
        }

        /// <summary>
        /// Add a page reference to a script that initializes a global JavaScript variable with the given data.
        /// </summary>
        /// <param name="globalVariable">The name of the global JavaScript variable to assign.</param>
        /// <param name="data">The dictionary of data, serialized into JSON.</param>
        /// <param name="pageLocation">The optional page location of the script. This controls where it will be rendered.</param>
        /// <param name="customizeBundle">The optional delegate used to customize the created bundle before adding it to the collection.</param>
        public static void AddPageData(string globalVariable, IEnumerable<KeyValuePair<string, object>> data, string pageLocation, Action<ScriptBundle> customizeBundle)
        {
            AddScriptBundle(new PageDataScriptBundle(globalVariable, data), pageLocation, customizeBundle);
        }

        static void AddScriptBundle(ScriptBundle bundle, string pageLocation, Action<ScriptBundle> customizeBundle)
        {
            if (customizeBundle != null)
            {
                customizeBundle(bundle);
            }

            ReferenceBuilder.Reference(bundle, pageLocation);
        }

        /// <summary>
        /// Renders the required HTML elements for the scripts referenced by the current page.
        /// </summary>        
        /// <returns>HTML script elements.</returns>
        public static IHtmlString RenderScripts()
        {
            return RenderScripts(null);
        }

        /// <summary>
        /// Renders the required HTML elements for the scripts referenced by the current page.
        /// </summary>
        /// <param name="pageLocation">The optional page location being rendered. Only scripts with this location are rendered.</param>
        /// <returns>HTML script elements.</returns>
        public static IHtmlString RenderScripts(string pageLocation)
        {
            return Render<ScriptBundle>(pageLocation);
        }

        /// <summary>
        /// Renders the required HTML elements for the stylesheets referenced by the current page.
        /// </summary>        
        /// <returns>HTML stylesheet link elements.</returns>
        public static IHtmlString RenderStylesheets()
        {
            return RenderStylesheets(null);
        }

        /// <summary>
        /// Renders the required HTML elements for the stylesheets referenced by the current page.
        /// </summary>
        /// <param name="pageLocation">The optional page location being rendered. Only stylesheets with this location are rendered.</param>
        /// <returns>HTML stylesheet link elements.</returns>
        public static IHtmlString RenderStylesheets(string pageLocation)
        {
            return Render<StylesheetBundle>(pageLocation);
        }

        /// <summary>
        /// Renders the required HTML elements for the HTML templates referenced by the current page.
        /// </summary>        
        /// <returns>HTML script elements.</returns>
        public static IHtmlString RenderHtmlTemplates()
        {
            return RenderHtmlTemplates(null);
        }

        /// <summary>
        /// Renders the required HTML elements for the HTML templates referenced by the current page.
        /// </summary>
        /// <param name="pageLocation">The optional page location being rendered. Only HTML templates with this location are rendered.</param>
        /// <returns>HTML script elements.</returns>
        public static IHtmlString RenderHtmlTemplates(string pageLocation)
        {
            return Render<HtmlTemplateBundle>(pageLocation);
        }

        /// <summary>
        /// Returns the URL of the bundle with the given path.
        /// </summary>
        /// <param name="bundlePath">The path of the bundle.</param>
        /// <returns>The URL of the bundle.</returns>
        public static string Url(string bundlePath)
        {
            var bundle = Application.FindBundleContainingPath<Bundle>(bundlePath);
            if (bundle == null)
            {
                throw new ArgumentException(string.Format("Bundle not found with path \"{0}\".", bundlePath));
            }

            return Application.Settings.UrlGenerator.CreateBundleUrl(bundle);
        }

        /// <summary>
        /// Returns the URL of the bundle with the given path.
        /// </summary>
        /// <param name="bundlePath">The path of the bundle.</param>
        /// <typeparamref name="T">Type of bundle.</typeparamref>
        /// <returns>The URL of the bundle.</returns>
        public static string Url<T>(string bundlePath)
            where T : Bundle
        {
            var bundle = Application.FindBundleContainingPath<T>(bundlePath);
            if (bundle == null)
            {
                throw new ArgumentException(string.Format("Bundle not found with path \"{0}\".", bundlePath));
            }

            return Application.Settings.UrlGenerator.CreateBundleUrl(bundle);
        }

        /// <summary>
        /// Returns all bundles in the application.
        /// </summary>
        /// <returns>The bundles.</returns>
        public static IEnumerable<Bundle> GetAllBundles()
        {
            return Application.Bundles;
        } 

        /// <summary>
        /// Gets the bundles that have been referenced during the current request.
        /// </summary>        
        public static IEnumerable<Bundle> GetReferencedBundles()
        {
            return GetReferencedBundles(null);
        }

        /// <summary>
        /// Gets the bundles that have been referenced during the current request.
        /// </summary>
        /// <param name="pageLocation">Optional. The page location of bundles to return.</param>
        public static IEnumerable<Bundle> GetReferencedBundles(string pageLocation)
        {
            return ReferenceBuilder.GetBundles(pageLocation);
        }

        /// <summary>
        /// Get the URLs for bundles that have been referenced during the current request.
        /// </summary>
        /// <typeparam name="T">The type of bundles.</typeparam>        
        public static IEnumerable<string> GetReferencedBundleUrls<T>(bool absoluteUrl = false)
            where T : Bundle
        {
            return GetReferencedBundleUrls<T>(null, absoluteUrl);
        }

        /// <summary>
        /// Get the URLs for bundles that have been referenced during the current request.
        /// </summary>
        /// <typeparam name="T">The type of bundles.</typeparam>
        /// <param name="pageLocation">Optional. The page location of bundles to return.</param>
        /// <param name="absoluteUrl">Optional. Whether to return the absolute URLs for the bundles. Defaults to false.</param>
        public static IEnumerable<string> GetReferencedBundleUrls<T>(string pageLocation, bool absoluteUrl = false)
            where T : Bundle
        {
            var result = ExternalGraphInteraction.GetReferencedBundleUrls<T>(pageLocation, absoluteUrl);
            if (result.Exception != null)
            {
                throw result.Exception;
            }
            return result.Enumerable;
        }

        public static IEnumerable<string> GetReferencedLocalizedStrings()
        {
            return GetReferencedLocalizedStrings(null);
        } 

        public static IEnumerable<string> GetReferencedLocalizedStrings(string pageLocation)
        {
            var result = ExternalGraphInteraction.GetReferencedLocalizedStrings(pageLocation);
            if (result.Exception != null)
            {
                throw result.Exception;
            }
            return result.Enumerable;
        }

        public static IEnumerable<string> GetReferencedAbConfigs()
        {
            return GetReferencedAbConfigs(null);
        }

        public static IEnumerable<string> GetReferencedAbConfigs(string pageLocation)
        {
            var result = ExternalGraphInteraction.GetReferencedAbConfigs(pageLocation);
            if (result.Exception != null)
            {
                throw result.Exception;
            }
            return result.Enumerable;
        } 

        static IHtmlString Render<T>(string location) where T : Bundle
        {
            var result = ExternalGraphInteraction.Render<T>(location);
            if(result.Exception != null)
            {
                throw result.Exception;
            }
            return new HtmlString(result.ResourceString);
        }

        static IReferenceBuilder ReferenceBuilder
        {
            get { return Application.GetReferenceBuilder(); }
        }

        static IInteractWithDependencyGraph ExternalGraphInteraction
        {
            get { return Application.GetInteration(); }
        }

        static ICassetteApplication Application
        {
            get { return CassetteApplicationContainer.Application; }
        }
    }
}
