using System.Collections.Generic;
using System.Linq;

namespace Cassette.Manifests
{
    class AssetManifestBuilder
    {
        public AssetManifest BuildManifest(IAsset asset)
        {
            var assetManifest = new AssetManifest
            {
                Path = asset.SourceFile.FullPath
            };
            foreach (var reference in GetReferences(asset))
            {
                assetManifest.References.Add(reference);
            }
            foreach (var localizedString in GetLocalizedStrings(asset))
            {
                assetManifest.LocalizedStrings.Add(localizedString);
            }
            return assetManifest;
        }

        IEnumerable<AssetReferenceManifest> GetReferences(IAsset asset)
        {
            return from r in asset.References
                   where r.Type != AssetReferenceType.SameBundle
                   select new AssetReferenceManifest
                   {
                       Path = r.Path,
                       Type = r.Type,
                       SourceLineNumber = r.SourceLineNumber
                   };
        }

        IEnumerable<AssetLocalizedStringManifest> GetLocalizedStrings(IAsset asset)
        {
            return from l in asset.LocalizedStrings
                   select new AssetLocalizedStringManifest
                   {
                       LocalizedString = l.Name,
                       SourceLineNumber = l.SourceLineNumber
                   };
        }
    }
}