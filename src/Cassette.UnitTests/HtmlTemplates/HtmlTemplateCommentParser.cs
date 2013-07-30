﻿using System.Linq;
using Should;
using Xunit;

namespace Cassette.HtmlTemplates
{
    public class HtmlTemplateCommentParser_Tests
    {
        readonly HtmlTemplateCommentParser parser = new HtmlTemplateCommentParser();
        
        [Fact]
        public void WhenParseEmptyComment_ThenReturnCommentWithEmptyValue()
        {
            parser.Parse("<!---->").Single().Value.ShouldEqual("");
        }

        [Fact]
        public void WhenParseHtmlComment_ThenReturnComment()
        {
            var comment = parser.Parse("<!-- text -->").Single();
            comment.LineNumber.ShouldEqual(1);
            comment.Value.ShouldEqual(" text ");
        }

        [Fact]
        public void WhenParseHtmlCommentWithNewLines_ThenReturnCommentPerLine()
        {
            var comments = parser.Parse("<!--text1\r\ntext2-->").ToArray();
            comments[0].LineNumber.ShouldEqual(1);
            comments[0].Value.ShouldEqual("text1");
            comments[1].LineNumber.ShouldEqual(2);
            comments[1].Value.ShouldEqual("text2");
        }

        [Fact]
        public void WhenParseHtmlCommentWithUnixNewLines_ThenReturnCommentPerLine()
        {
            var comments = parser.Parse("<!--text1\ntext2-->").ToArray();
            comments[0].LineNumber.ShouldEqual(1);
            comments[0].Value.ShouldEqual("text1");
            comments[1].LineNumber.ShouldEqual(2);
            comments[1].Value.ShouldEqual("text2");
        }

        [Fact]
        public void SkipsNewLinesBeforeComment()
        {
            var comments = parser.Parse("\r\n<!--text-->").ToArray();
            comments[0].LineNumber.ShouldEqual(2);
        }

        [Fact]
        public void WhenParseHtmlWithNoComments_ThenReturnNoComments()
        {
            parser.Parse("<div></div>").ToArray().Length.ShouldEqual(0);
        }

        [Fact]
        public void WhenParseHtmlCommentWithI18NSameLine_ThenReturnLocalizeComment()
        {
            var comment = parser.Parse("<div>{{#i18n}}Test.Localized.String{{/i18n}}</div>").Single();
            comment.Value.ShouldEqual("@localize Test.Localized.String");
        }

        [Fact]
        public void WhenParseHtmlCommentWithI18NSameLineWithSpaces_ThenReturnLocalizeComment()
        {
            var comment = parser.Parse("<div>{{# i18n }}Test.Localized.String{{/ i18n }}</div>").Single();
            comment.Value.ShouldEqual("@localize Test.Localized.String");
        }

        [Fact]
        public void WhenParseHtmlCommentWithI18NDifferentLines_ThenReturnLocalizeComment()
        {
            var comment = parser.Parse("<div>\r{{#i18n}}\r\nTest.Localized.String\n{{/i18n}}\r\n</div>").Single();
            comment.Value.ShouldEqual("@localize Test.Localized.String");
        }

        [Fact]
        public void WhenParseHtmlCommentWithI18NDifferentLinesWithSpaces_ThenReturnLocalizeComment()
        {
            var comment = parser.Parse("<div>\r{{# i18n }}\r\nTest.Localized.String\n{{/ i18n }}\r\n</div>").Single();
            comment.Value.ShouldEqual("@localize Test.Localized.String");
        }
    }
}
