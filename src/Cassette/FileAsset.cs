﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Cassette.IO;
using Cassette.Utilities;

namespace Cassette
{
    public class FileAsset : AssetBase
    {
        public FileAsset(IFile sourceFile, Bundle parentBundle)
        {
            this.parentBundle = parentBundle;
            this.sourceFile = sourceFile;

            hash = new Lazy<byte[]>(ComputeHash); // Avoid file IO until the hash is actually needed.
        }

        readonly Bundle parentBundle;
        readonly IFile sourceFile;
        readonly Lazy<byte[]> hash;
        readonly List<AssetReference> references = new List<AssetReference>();
        readonly List<AssetLocalizedString> localizedStrings = new List<AssetLocalizedString>(); 
        readonly List<AssetAbConfig> abConfigs = new List<AssetAbConfig>(); 

        public override IFile SourceFile
        {
            get { return sourceFile; }
        }

        public override byte[] Hash
        {
            get { return hash.Value; }
        }

        public override IEnumerable<AssetReference> References
        {
            get { return references; }
        }

        public override IEnumerable<AssetLocalizedString> LocalizedStrings
        {
            get { return localizedStrings; }
        }

        public override IEnumerable<AssetAbConfig> AbConfigs
        {
            get { return abConfigs; }
        }

        public override void AddReference(string assetRelativePath, int lineNumber)
        {
            if (assetRelativePath.IsUrl())
            {
                AddUrlReference(assetRelativePath, lineNumber);
            }
            else
            {
                string appRelativeFilename;
                if (assetRelativePath.StartsWith("~"))
                {
                    appRelativeFilename = assetRelativePath;
                }
                else if (assetRelativePath.StartsWith("/"))
                {
                    appRelativeFilename = "~" + assetRelativePath;
                }
                else
                {
                    var subDirectory = SourceFile.Directory.FullPath;
                    appRelativeFilename = PathUtilities.CombineWithForwardSlashes(
                        subDirectory,
                        assetRelativePath
                        );
                }
                appRelativeFilename = PathUtilities.NormalizePath(appRelativeFilename);
                AddBundleReference(appRelativeFilename, lineNumber);
            }
        }

        public void RemoveReference(AssetReference reference)
        {
            references.Remove(reference);
        }

        void AddBundleReference(string appRelativeFilename, int lineNumber)
        {
            var type = parentBundle.ContainsPath(appRelativeFilename)
                           ? AssetReferenceType.SameBundle
                           : AssetReferenceType.DifferentBundle;
            references.Add(new AssetReference(appRelativeFilename, this, lineNumber, type));
        }

        void AddUrlReference(string url, int sourceLineNumber)
        {
            references.Add(new AssetReference(url, this, sourceLineNumber, AssetReferenceType.Url));
        }

        public override void AddRawFileReference(string relativeFilename)
        {
            if (relativeFilename.StartsWith("/"))
            {
                relativeFilename = "~" + relativeFilename;
            }
            else if (!relativeFilename.StartsWith("~"))
            {
                relativeFilename = PathUtilities.NormalizePath(PathUtilities.CombineWithForwardSlashes(
                    SourceFile.Directory.FullPath,
                    relativeFilename
                                                                   ));
            }

            var alreadyExists = references.Any(r => r.Path.Equals(relativeFilename, StringComparison.OrdinalIgnoreCase));
            if (alreadyExists) return;

            references.Add(new AssetReference(relativeFilename, this, -1, AssetReferenceType.RawFilename));
        }

        public override void AddLocalizedString(string localizedString, int lineNumber)
        {
            localizedStrings.Add(new AssetLocalizedString(localizedString, this, lineNumber));
        }

        public override void AddAbConfig(string abConfig, int lineNumber)
        {
            abConfigs.Add(new AssetAbConfig(abConfig, this, lineNumber));
        }

        byte[] ComputeHash()
        {
            using (var sha1 = SHA1.Create())
            using (var stream = OpenStream())
            {
                return sha1.ComputeHash(stream);
            }
        }

        protected override Stream OpenStreamCore()
        {
            return sourceFile.OpenRead();
        }

        public override void Accept(IBundleVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}