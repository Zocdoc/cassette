using System.Collections.Generic;

namespace Cassette
{
    class BundleDescriptor
    {
        public BundleDescriptor()
        {
            AssetFilenames = new List<string>();
            References = new List<string>();
            LocalizedStrings = new List<string>();
            AbConfigs = new List<string>();
        }

        public List<string> AssetFilenames { get; private set; }
        public List<string> References { get; private set; }
        public List<string> LocalizedStrings { get; private set; } 
        public List<string> AbConfigs { get; private set; } 
        public string ExternalUrl { get; set; }
        public string FallbackCondition { get; set; }
        public bool IsFromFile { get; set; }
    }
}