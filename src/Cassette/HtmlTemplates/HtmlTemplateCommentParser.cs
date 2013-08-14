using System;
using System.Collections.Generic;
using Cassette.BundleProcessing;

namespace Cassette.HtmlTemplates
{
    class HtmlTemplateCommentParser : ICommentParser
    {        enum State
        {
            Code, Comment, I18N
        }

        const string Localize = "@localize ";

        public IEnumerable<Comment> Parse(string code)
        {
            var state = State.Code;
            var commentStart = 0;
            var line = 1;
            for (var i = 0; i <= code.Length - 3; i++)
            {
                switch (state)
                {
                    case State.Code:
                        if (code[i] == '\r')
                        {
                            if (i < code.Length - 1 && code[i + 1] == '\n')
                            {
                                i++;
                            }
                            line++;
                            continue;
                        }
                        else if (code[i] == '\n')
                        {
                            line++;
                            continue;
                        }
                        else if (i < code.Length - 4 && code.Substring(i, 4) == "<!--")
                        {
                            state = State.Comment;
                            i += 3;
                            commentStart = i + 1;
                        }
                        else if (i < code.Length - 9 && code.Substring(i, 9) == "{{#i18n}}")
                        {
                            state = State.I18N;
                            i += 9;
                            commentStart = i;
                        }
                        else if (i < code.Length - 11 && code.Substring(i, 11) == "{{# i18n }}")
                        {
                            state = State.I18N;
                            i += 11;
                            commentStart = i;
                        }
                        break;

                    case State.Comment:
                        if (i < code.Length - 3 && code.Substring(i, 3) == "-->")
                        {
                            yield return new Comment
                            {
                                LineNumber = line,
                                Value = code.Substring(commentStart, i - commentStart)
                            };
                            i += 2;
                            state = State.Code;
                        }
                        else if (code[i] == '\r')
                        {
                            if (i < code.Length - 1 && code[i + 1] == '\n')
                            {
                                yield return new Comment
                                {
                                    LineNumber = line,
                                    Value = code.Substring(commentStart, i - commentStart)
                                };
                                i++;
                                commentStart = i + 1;
                            }
                            line++;
                        }
                        else if (code[i] == '\n')
                        {
                            yield return new Comment
                            {
                                LineNumber = line,
                                Value = code.Substring(commentStart, i - commentStart)
                            };
                            i++;
                            line++;
                            commentStart = i;
                        }
                        break;

                    case State.I18N:
                        if (i < code.Length - 9 && code.Substring(i, 9) == "{{/i18n}}")
                        {
                            yield return new Comment
                            {
                                LineNumber = line,
                                Value = Localize + code.Substring(commentStart, i - commentStart).Trim()
                            };
                            i += 9;
                            state = State.Code;
                        }
                        else if (i < code.Length - 11 && code.Substring(i, 11) == "{{/ i18n }}")
                        {
                            yield return new Comment
                            {
                                LineNumber = line,
                                Value = Localize + code.Substring(commentStart, i - commentStart).Trim()
                            };
                            i += 11;
                            state = State.Code;
                        }
                        break;
                }
            }
        }
    }
}
