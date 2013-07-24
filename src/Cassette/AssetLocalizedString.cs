using System;

namespace Cassette
{
    public class AssetLocalizedString
    {
        public AssetLocalizedString(string name, IAsset sourceAsset, int sourceLineNumber)
        {
            Name = name;
            SourceAsset = sourceAsset;
            SourceLineNumber = sourceLineNumber;
        }

        /// <summary>
        /// Name of a localized string.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The asset that made this reference.
        /// </summary>
        public IAsset SourceAsset { get; private set; }

        /// <summary>
        /// The line number in the asset file that made this reference.
        /// </summary>
        public int SourceLineNumber { get; private set; }
    }
}
