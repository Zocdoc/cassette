using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cassette
{
    public class AssetAbConfig
    {
        public AssetAbConfig(string name, IAsset sourceAsset, int sourceLineNumber)
        {
            Name = name;
            SourceAsset = sourceAsset;
            SourceLineNumber = sourceLineNumber;
        }

        /// <summary>
        /// Name of an Ab Config.
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
