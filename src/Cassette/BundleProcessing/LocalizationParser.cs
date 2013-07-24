using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cassette.BundleProcessing
{
    class LocalizationParser
    {
        readonly ICommentParser commentParser;

        public LocalizationParser(ICommentParser commentParser)
        {
            this.commentParser = commentParser;
        }

        public IEnumerable<string> Parse(string code, IAsset asset)
        {
            var comments = commentParser.Parse(code);

            return
                from comment in comments
                from localizedString in ParseStrings(comment.Value, asset)
                select localizedString;
        }

        protected virtual IEnumerable<string> ParseStrings(string comment, IAsset sourceAsset)
        {
            comment = comment.Trim().TrimEnd(';');
            if (!comment.StartsWith("@localize")) yield break;

            var pathStart = 0;
            for (var i = 9; i < comment.Length; i++)
            {
                var c = comment[i];
                if (char.IsWhiteSpace(c)) continue;
                pathStart = i;
            }
            yield return comment.Substring(pathStart);
        }
    }
}
