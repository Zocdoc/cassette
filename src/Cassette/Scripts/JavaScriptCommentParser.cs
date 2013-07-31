using System.Collections.Generic;
using Cassette.BundleProcessing;

namespace Cassette.Scripts
{
    class JavaScriptCommentParser : ICommentParser
    {
        enum State
        {
            Code, SingleLineComment, MultiLineComment, I18N
        }

        public IEnumerable<Comment> Parse(string code)
        {
            var state = State.Code;
            var commentStart = 0;
            var line = 1;
            for (var i = 0; i < code.Length; i++)
            {
                var c = code[i];

                if (c == '\r')
                {
                    if (i < code.Length - 1 && code[i + 1] == '\n')
                    {
                        i++;
                    }
                    line++;
                    continue;
                }
                else if (c == '\n')
                {
                    line++;
                    continue;
                }

                switch (state)
                {
                    case State.Code:
                        if (i < code.Length - 7 && code.Substring(i, 7) == "i18n.t(")
                        {
                            state = State.I18N;
                            commentStart = i + 7;
                            i = i + 6;
                            continue;
                        }
                        if (c != '/') continue;
                        if (i >= code.Length - 2) yield break;
                        if (code[i + 1] == '/')
                        {
                            state = State.SingleLineComment;
                            commentStart = i + 2;
                            i++; // Skip the '/'
                        }
                        else if (code[i + 1] == '*')
                        {
                            state = State.MultiLineComment;
                            commentStart = i + 2;
                            i++; // Skip the '*'
                        }
                        break;

                    case State.SingleLineComment:
                        // Scan forward until newline or end of code.
                        while (i < code.Length && code[i] != '\r' && code[i] != '\n')
                        {
                            i++;
                        }
                        yield return new Comment
                        {
                            LineNumber = line,
                            Value = code.Substring(commentStart, i - commentStart)
                        };
                        if (i < code.Length - 1 && code[i] == '\r' && code[i + 1] == '\n') i++;
                        line++;
                        state = State.Code;
                        break;

                    case State.MultiLineComment:
                        // Scan forwards until "*/" or end of code.
                        while (i < code.Length - 1 && (code[i] != '*' || code[i + 1] != '/'))
                        {
                            // Track new lines within the comment.
                            switch (code[i])
                            {
                                case '\r':
                                    yield return new Comment
                                    {
                                        LineNumber = line,
                                        Value = code.Substring(commentStart, i - commentStart)
                                    };
                                    i++;
                                    if (i < code.Length && code[i] == '\n')
                                    {
                                        i++;
                                    }
                                    commentStart = i;
                                    line++;
                                    break;

                                case '\n':
                                    yield return new Comment
                                    {
                                        LineNumber = line,
                                        Value = code.Substring(commentStart, i - commentStart)
                                    };
                                    i++;
                                    commentStart = i;
                                    line++;
                                    break;

                                default:
                                    i++;
                                    break;
                            }
                        }
                        yield return new Comment
                        {
                            LineNumber = line,
                            Value = code.Substring(commentStart, i - commentStart)
                        };
                        i++; // Skip the '/'
                        state = State.Code;
                        break;

                    case State.I18N:
                        // Scan forward until we get past the open quote for the string.
                        while (i < code.Length && code[i] != '\'' && code[i] != '"')
                        {
                            switch (code[i])
                            {
                                case '\r':
                                case '\n':
                                    line++;
                                    break;
                            }
                            i++;
                        }
                        i++;
                        commentStart = i;
                        while (i < code.Length && code[i] != '\'' && code[i] != '"')
                        {
                            switch (code[i])
                            {
                                case '\r':
                                case '\n':
                                    line++;
                                    break;
                            }
                            i++;
                        }
                        yield return new Comment
                        {
                            LineNumber = line,
                            Value = "@localize " +  code.Substring(commentStart, i - commentStart)
                        };
                        state = State.Code;
                        break;
                }
            }
        }
    }
}
