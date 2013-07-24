using System;
using System.Collections.Generic;
using System.Linq;

namespace Cassette.BundleProcessing
{
    class ReferenceParser
    {
        enum State { None, InSingleQuote, InDoubleQuote, InRawPath }

        public enum ReferenceType { Asset, LocalizedString }

        public class ParsedReference
        {
            public ReferenceType Type;
            public string Path;
            public int LineNumber;
        }

        readonly ICommentParser commentParser;

        public ReferenceParser(ICommentParser commentParser)
        {
            this.commentParser = commentParser;
        }

        public IEnumerable<ParsedReference> Parse(string code, IAsset asset)
        {
            var comments = commentParser.Parse(code);

            return
                from comment in comments
                from reference in ParseReferences(comment.Value, asset, comment.LineNumber)
                select reference;
        }

        protected virtual IEnumerable<ParsedReference> ParseReferences(string comment, IAsset sourceAsset, int lineNumber)
        {
            int i;
            ReferenceType type;

            comment = comment.Trim().TrimEnd(';');

            if (comment.StartsWith("@reference"))
            {
                i = 10;
                type = ReferenceType.Asset;
            }
            else if (comment.StartsWith("@localize"))
            {
                i = 9;
                type = ReferenceType.LocalizedString;
            }
            else
            {
                yield break;
            }
            
            var state = State.None;
            var pathStart = 0;
            for (; i < comment.Length; i++)
            {
                var c = comment[i];
                switch (state)
                {
                    case State.None:
                        if (char.IsWhiteSpace(c)) continue;
                        else if (c == '"')
                        {
                            state = State.InDoubleQuote;
                            pathStart = i + 1;
                        }
                        else if (c == '\'')
                        {
                            state = State.InSingleQuote;
                            pathStart = i + 1;
                        }
                        else
                        {
                            state = State.InRawPath;
                            pathStart = i;
                        }
                        break;

                    case State.InSingleQuote:
                        if (c == '\'')
                        {
                            var reference = new ParsedReference
                            {
                                Type = type,
                                Path = comment.Substring(pathStart, i - pathStart),
                                LineNumber = lineNumber
                            };
                            yield return reference;
                            state = State.None;
                        }
                        break;

                    case State.InDoubleQuote:
                        if (c == '"')
                        {
                            var reference = new ParsedReference
                            {
                                Type = type,
                                Path = comment.Substring(pathStart, i - pathStart),
                                LineNumber = lineNumber
                            };
                            yield return reference;
                            state = State.None;
                        }
                        break;

                    case State.InRawPath:
                        if (char.IsWhiteSpace(c))
                        {
                            var reference = new ParsedReference
                            {
                                Type = type,
                                Path = comment.Substring(pathStart, i - pathStart),
                                LineNumber = lineNumber
                            };
                            yield return reference;
                            state = State.None;
                        }
                        break;
                }
            }
            if (state == State.InRawPath)
            {
                var reference = new ParsedReference
                {
                    Type = type,
                    Path = comment.Substring(pathStart),
                    LineNumber = lineNumber
                };
                yield return reference;
            }
            else if (state == State.InDoubleQuote)
            {
                throw new Exception(string.Format("Asset reference error in {0} line {1}. Missing closing double quote (\").", sourceAsset.SourceFile.FullPath, lineNumber));
            }
            else if (state == State.InSingleQuote)
            {
                throw new Exception(string.Format("Asset reference error in {0} line {1}. Missing closing single quote (').", sourceAsset.SourceFile.FullPath, lineNumber));
            }
        }
    }
}
