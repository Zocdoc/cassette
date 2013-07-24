using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Cassette.BundleProcessing;

namespace Cassette.Scripts
{
    class JavaScriptReferenceParser : ReferenceParser
    {
        static readonly Regex ReferenceRegex = new Regex(
            @"\<reference \s+ path \s* = \s* (?<quote>[""']) (?<path>.*?) \<quote> \s* /?>",
            RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase
            );

        public JavaScriptReferenceParser(ICommentParser commentParser)
            : base(commentParser)
        {
        }

        protected override IEnumerable<ParsedReference> ParseReferences(string comment, IAsset sourceAsset, int lineNumber)
        {
            var simplePaths = base.ParseReferences(comment, sourceAsset, lineNumber);
            var xmlCommentPaths = ParseXmlDocCommentPaths(comment, lineNumber);
            return simplePaths.Concat(xmlCommentPaths);
        }

        static IEnumerable<ParsedReference> ParseXmlDocCommentPaths(string comment, int lineNumber)
        {
            return ReferenceRegex
                .Matches(comment)
                .Cast<Match>()
                .Select(m => new ParsedReference
                {
                    Type = ReferenceType.Asset,
                    Path = m.Groups["path"].Value,
                    LineNumber = lineNumber
                });
        }
    }
}
