using System.Collections.Generic;
using Microsoft.Ajax.Utilities;

namespace Cassette.Manifests
{
    class BundleManifestBuilder<TBundle, TManifest> : IBundleVisitor
        where TBundle : Bundle
        where TManifest : BundleManifest, new()
    {
        TManifest bundleManifest;

        public bool IncludeContent { get; set; }

        public virtual TManifest BuildManifest(TBundle bundle)
        {
            bundle.Accept(this);
            return bundleManifest;
        }

        void IBundleVisitor.Visit(Bundle bundle)
        {
            bundleManifest = new TManifest
            {
                Path = bundle.Path,
                Hash = bundle.Hash,
                ContentType = bundle.ContentType,
                PageLocation = bundle.PageLocation,
                Content = GetContent(bundle),
                Html = bundle.Render
            };
            AddReferencesToBundleManifest(bundle.References);
            AddLocalizedStringsToBundleManifest(bundle.LocalizedStrings);
            AddAbConfigsToBundleManifest(bundle.AbConfigs);
            AddHtmlAttributesToBundleManifest(bundle.HtmlAttributes);
        }

        byte[] GetContent(Bundle bundle)
        {
            if (!IncludeContent || bundle.Assets.Count == 0) return null;
            using (var stream = bundle.OpenStream())
            {
                var bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);
                return bytes;
            }
        }

        void AddReferencesToBundleManifest(IEnumerable<string> references)
        {
            foreach (var reference in references)
            {
                bundleManifest.References.Add(reference);
            }
        }

        void AddLocalizedStringsToBundleManifest(IEnumerable<string> localizedStrings)
        {
            foreach (var localizedString in localizedStrings)
            {
                bundleManifest.LocalizedStrings.Add(localizedString);
            }
        }

        void AddAbConfigsToBundleManifest(IEnumerable<string> abConfigs)
        {
            foreach (var abConfig in abConfigs)
            {
                bundleManifest.AbConfigs.Add(abConfig);
            }
        }

        void AddHtmlAttributesToBundleManifest(IEnumerable<KeyValuePair<string, string>> htmlAttributes)
        {
            foreach (var htmlAttribute in htmlAttributes)
            {
                bundleManifest.HtmlAttributes.Add(htmlAttribute.Key, htmlAttribute.Value);
            }
        }

        void IBundleVisitor.Visit(IAsset asset)
        {
            var assetManifest = new AssetManifestBuilder().BuildManifest(asset);
            bundleManifest.Assets.Add(assetManifest);
        }
    }
}