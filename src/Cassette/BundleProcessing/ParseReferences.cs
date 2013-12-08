using System.IO;
using Cassette.Configuration;

namespace Cassette.BundleProcessing
{
    public abstract class ParseReferences<T> : IBundleProcessor<T>
        where T : Bundle
    {
        public void Process(T bundle, CassetteSettings settings)
        {
            foreach (var asset in bundle.Assets)
            {
                if (ShouldParseAsset(asset))
                {
                    ParseAssetReferences(asset);
                }
            }
        }

        protected virtual bool ShouldParseAsset(IAsset asset)
        {
            return true;
        }

        void ParseAssetReferences(IAsset asset)
        {
            string code;
            using (var reader = new StreamReader(asset.OpenStream()))
            {
                code = reader.ReadToEnd();
            }

            var commentParser = CreateCommentParser();
            var referenceParser = CreateReferenceParser(commentParser);
            var references = referenceParser.Parse(code, asset);
            foreach (var reference in references)
            {
                switch (reference.Type)
                {
                    case ReferenceParser.ReferenceType.Asset:
                        asset.AddReference(reference.Path, reference.LineNumber);
                        break;
                    case ReferenceParser.ReferenceType.LocalizedString:
                        asset.AddLocalizedString(reference.Path, reference.LineNumber);
                        break;
                    case ReferenceParser.ReferenceType.AbConfig:
                        asset.AddAbConfig(reference.Path, reference.LineNumber);
                        break;
                    default:
                        throw new InvalidDataException("Invalid reference type");
                }
            }
        }

        internal virtual ReferenceParser CreateReferenceParser(ICommentParser commentParser)
        {
            return new ReferenceParser(commentParser);
        }

        internal abstract ICommentParser CreateCommentParser();
    }
}