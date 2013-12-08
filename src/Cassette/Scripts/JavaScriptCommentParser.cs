using System;
using System.Collections.Generic;
using Cassette.BundleProcessing;

namespace Cassette.Scripts
{
    class JavaScriptCommentParser : ICommentParser
    {
        enum State
        {
            Code, SingleLineComment, MultiLineComment, I18N, AbConfig
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
                            i += 6;
                            continue;
                        }
                        if (i < code.Length - 8 && code.Substring(i, 8) == "AB.isOn(")
                        {
                            state = State.AbConfig;
                            i += 7;
                            continue;
                        }
                        if (i < code.Length - 14 && code.Substring(i, 14) == "AB.getVariant(")
                        {
                            state = State.AbConfig;
                            i += 13;
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
                        switch (code[i])
                        {
                            // Whitespace is allowed
                            case '\t':
                            case ' ':
                                continue;

                            // If we found the open quote, we can begin
                            case '\'':
                            case '"':
                                i++;
                                break;

                            // If we found any other symbols, this isn't a localized string we can reference
                            default:
                                state = State.Code;
                                continue;
                        }
                        commentStart = i;
                        // Scan until we find the closing quote
                        while (i < code.Length && code[i] != '\'' && code[i] != '"')
                        {
                            i++;
                        }
                        yield return new Comment
                        {
                            LineNumber = line,
                            Value = "@localize " +  code.Substring(commentStart, i - commentStart)
                        };
                        state = State.Code;
                        break;

                    case State.AbConfig:
                        switch (code[i])
                        {
                            // Whitespace is allowed
                            case '\t':
                            case ' ':
                                continue;

                            // If we found the open quote, we can begin
                            case '\'':
                            case '"':
                                i++;
                                break;

                            // If we found any other symbols, this isn't an AB config we can reference
                            default:
                                state = State.Code;
                                continue;
                        }
                        commentStart = i;
                        // Scan until we find the closing quote
                        while (i < code.Length && code[i] != '\'' && code[i] != '"')
                        {
                            i++;
                        }
                        yield return new Comment
                        {
                            LineNumber = line,
                            Value = "@abconfig " +  code.Substring(commentStart, i - commentStart)
                        };
                        state = State.Code;
                        break;
                }
            }
        }
    }
}
