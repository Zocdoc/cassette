using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using Cassette.IO;

namespace Cassette.Web.Jasmine
{
    /// <summary>
    /// Partial implementation of IAsset that reads from an assembly resource stream.
    /// </summary>
    class ResourceAsset : IAsset
    {
        readonly string resourceName;
        readonly Assembly assembly;

        public ResourceAsset(string resourceName, Assembly assembly)
        {
            this.resourceName = resourceName;
            this.assembly = assembly;
        }

        public byte[] Hash
        {
            get
            {
                using (var stream = OpenStream())
                {
                    using (var sha1 = SHA1.Create())
                    {
                        return sha1.ComputeHash(stream);
                    }
                }
            }
        }

        public IFile SourceFile
        {
            get { return new ResourceFile("~/" + resourceName); }
        }

        public IEnumerable<AssetReference> References
        {
            get { yield break; }
        }

        public IEnumerable<AssetLocalizedString> LocalizedStrings
        {
            get { yield break; }
        }

        public IEnumerable<AssetAbConfig> AbConfigs
        {
            get { yield break; }
        } 

        public void Accept(IBundleVisitor visitor)
        {
            visitor.Visit(this);
        }

        public void AddAssetTransformer(IAssetTransformer transformer)
        {
            // No transformations applied. Asset served as-is.
        }

        public void AddReference(string assetRelativePath, int lineNumber)
        {
        }

        public void AddRawFileReference(string relativeFilename)
        {
        }

        public void AddLocalizedString(string localizedString, int lineNumber)
        {
        }

        public void AddAbConfig(string abConfig, int lineNumber)
        {
        }

        public Stream OpenStream()
        {
            return assembly.GetManifestResourceStream(resourceName);
        }
    }
}