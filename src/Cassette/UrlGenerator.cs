using System;
using Cassette.Utilities;

namespace Cassette
{
    class UrlGenerator : IUrlGenerator
    {
        readonly IUrlModifier urlModifier;
        readonly string routePrefix;

        public UrlGenerator(IUrlModifier urlModifier, string routePrefix)
        {
            this.urlModifier = urlModifier;
            this.routePrefix = routePrefix;
        }

        public string CreateBundleUrl(Bundle bundle, bool absoluteUrl = false)
        {
            var url = routePrefix + "/" + bundle.Url;
            url = urlModifier.PreCacheModify(url);
            if (absoluteUrl)
            {
                return urlModifier.PostCacheModify(url);
            }
            return url;
        }

        public string CreateAssetUrl(IAsset asset, bool absoluteUrl = false)
        {
            var url = urlModifier.PreCacheModify(
                String.Format(
                    "{0}/asset/{1}?{2}",
                    routePrefix,
                    asset.SourceFile.FullPath.Substring(2),
                    asset.Hash.ToHexString()
                )
            );
            if (absoluteUrl)
            {
                return urlModifier.PostCacheModify(url);
            }
            return url;
        }

        public string CreateRawFileUrl(string filename, string hash)
        {
            if (filename.StartsWith("~") == false)
            {
                throw new ArgumentException("Image filename must be application relative (starting with '~').");
            }

            filename = filename.Substring(2); // Remove the "~/"
            var dotIndex = filename.LastIndexOf('.');
            var name = filename.Substring(0, dotIndex);
            var extension = filename.Substring(dotIndex + 1);

            return urlModifier.PreCacheModify(
                String.Format(
                    "{0}/file/{1}_{2}.{3}",
                    routePrefix,
                    ConvertToForwardSlashes(name),
                    hash,
                    extension
                )
            );
        }

        // TODO: move RoutePrefix to settings?
        public const string RoutePrefix = "_cassette";
        
        string ConvertToForwardSlashes(string path)
        {
            return path.Replace('\\', '/');
        }
    }
}