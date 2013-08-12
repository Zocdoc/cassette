using System;
using System.Collections.Generic;
using System.Linq;
using Cassette.Configuration;
using Cassette.DependencyGraphInteration.InterationResults;
using Cassette.Utilities;

namespace Cassette.DependencyGraphInteration.InMemory
{
    public class InMemoryDependencyGraphInteraction : IInteractWithDependencyGraph
    {
        readonly ICassetteApplication application;

        public InMemoryDependencyGraphInteraction(ICassetteApplication application)
        {
            this.application = application;
        }

        public BundleContainerInteractionResult CreateBundleContainer(CassetteSettings settings, IEnumerable<ICassetteConfiguration> configs)
        {
            return PerformInteraction(() =>
            {
                var bundleContainerFactory = settings.GetBundleContainerFactory(configs);
                return new BundleContainerInteractionResult
                {
                    BundleContainer = bundleContainerFactory.CreateBundleContainer()
                };
            });   
        }

        public SimpleInteractionResult ReferenceBundle(string path, string location)
        {
            return PerformInteraction(() =>
            {
                var referenceBuilder = application.GetReferenceBuilder();
                referenceBuilder.Reference(path, location);
                return new SimpleInteractionResult();
            });
        }

        public StringInterationResult Render<T>(string location) where T : Bundle
        {
            return PerformInteraction(() =>
            {
                var referenceBuilder = application.GetReferenceBuilder();
                return new StringInterationResult
                {
                    ResourceString = referenceBuilder.Render<T>(location)
                };
            });
        }

        public StreamInterationResult GetAsset(string path)
        {
            return PerformInteraction(() =>
            {
                IAsset asset;
                Bundle bundle;
                if (!application.Bundles.TryGetAssetByPath(path, out asset, out bundle))
                {
                    return new StreamInterationResult
                    {
                        NotFound = true
                    };
                }

                return new StreamInterationResult(asset.OpenStream())
                {
                    Hash = asset.Hash.ToHexString(),
                    ContentType = bundle.ContentType
                };

            });

            
        }

        public StreamInterationResult GetBundle<T>(string path)
            where T: Bundle
        {
            return PerformInteraction(() =>
            {
                var bundle = application.FindBundleContainingPath<T>(path);
                if (bundle == null)
                {
                    return new StreamInterationResult
                    {
                        NotFound = true
                    };
                }

                return new StreamInterationResult(bundle.OpenStream())
                {
                    Hash = bundle.Hash.ToHexString(),
                    ContentType = bundle.ContentType
                };

            });
        }

        public ImageExistsInteractionResult ImageExists(string path)
        {
            var rawFileReferenceFinder = new RawFileReferenceFinder(path);
            foreach (var bundle in application.Bundles)
            {
                bundle.Accept(rawFileReferenceFinder);
                if (rawFileReferenceFinder.IsRawFileReferenceFound)
                {
                    return new ImageExistsInteractionResult
                    {
                        ImageExists = true
                    };
                }
            }
            return new ImageExistsInteractionResult
            {
                ImageExists = false
            };
        }

        public EnumerableInterationResult GetReferencedBundleUrls<T>(string location, bool absoluteUrl) where T : Bundle
        {
            return PerformInteraction(() =>
            {
                var referenceBuilder = application.GetReferenceBuilder();
                var bundles = referenceBuilder.GetBundles(location).OfType<T>();
                if (application.Settings.IsDebuggingEnabled)
                {
                    return new EnumerableInterationResult
                    {
                        Enumerable = bundles
                            .SelectMany(GetAllAssets)
                            .Select(a => application.Settings.UrlGenerator.CreateAssetUrl(a, absoluteUrl))
                            .ToArray()
                    };
                }
                return new EnumerableInterationResult
                {
                    Enumerable = bundles
                        .Select(a => application.Settings.UrlGenerator.CreateBundleUrl(a, absoluteUrl))
                        .ToArray()
                };
            });
        }

        private IEnumerable<IAsset> GetAllAssets(Bundle bundle)
        {
            var collector = new AssetCollector();
            bundle.Accept(collector);
            return collector.Assets;
        }

        class AssetCollector : IBundleVisitor
        {
            public AssetCollector()
            {
                Assets = new List<IAsset>();
            }

            public List<IAsset> Assets { get; private set; }

            public void Visit(Bundle bundle)
            {
            }

            public void Visit(IAsset asset)
            {
                Assets.Add(asset);
            }
        }

        public EnumerableInterationResult GetReferencedLocalizedStrings(string location)
        {
            return PerformInteraction(() =>
            {
                var referenceBuilder = application.GetReferenceBuilder();
                var bundles = referenceBuilder.GetBundles(location);
                return new EnumerableInterationResult
                {
                    Enumerable =
                        bundles.SelectMany(b => b.Assets).SelectMany(a => a.LocalizedStrings).Select(l => l.Name).ToArray()
                };
            });
        }

        private T PerformInteraction<T>(Func<T> action)
            where T : IInterationResult, new()
        {
            try
            {
                return action();
            }
            catch (Exception exception)
            {
                return new T
                {
                    Exception = exception
                };
            }
        }


        
    }
}
