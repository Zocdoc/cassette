using System.Linq;
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

        [Fact]
        public void WhenParseHtmlCommentWithI18NComplex_ThenReturnLocalizeComments()
        {
            var mustache =
                @"<div class=""-dialog-close -close"" data-test=""close-modal""></div>
<div class=""-header-bar"">
  <div class=""-header-text"">
	{{#IsLive}}
		{{#i18n}}
            Csr.CustomSearchExperience.Footer.TurnOffAllTheThings{{/ i18n }}
	{{/IsLive}}
	{{^IsLive}}
		{{# i18n }}
            Csr.CustomSearchExperience.Footer.TurnOnAllTheThings
        {{/i18n}}
	{{/IsLive}}
  </div>
</div>
<div class=""-body"">
	<img class=""-all-the-things"" src=""/images/csr/customsearchexperience/allthethings.png""/>
	<div class=""-warning"">
		{{#IsLive}}
			{{#i18n}}Csr.CustomSearchExperience.Footer.TurnOffHowSureAreYou{{/i18n}}
		{{/IsLive}}
		{{^IsLive}}
			{{# i18n }}Csr.CustomSearchExperience.Footer.TurnOnHowSureAreYou{{/ i18n }}
		{{/IsLive}}
	</div>
	<div class=""-options"">
		<div class=""-submit-column"">
			<span class=""-button-primary "">
				{{# i18n }}Csr.CustomSearchExperience.Footer.EleventyBillion{{/ i18n }}
				<button type=""submit"" name=""EleventyBillion"" data-test=""EleventyBillion"" id=""submit"">{{# i18n }}Csr.CustomSearchExperience.Footer.EleventyBillion{{/ i18n }}</button>
			</span>
		</div>
		<div class=""-cancel-column"">
				<a class=""-cancel"">{{# i18n }}Csr.CustomSearchExperience.Footer.Cancel{{/ i18n }}</a>
		</div>
		
	</div>
</div>";
            var comments = parser.Parse(mustache).ToArray();
            comments.Length.ShouldEqual(7);
            comments[0].Value.ShouldEqual("@localize Csr.CustomSearchExperience.Footer.TurnOffAllTheThings");
            comments[1].Value.ShouldEqual("@localize Csr.CustomSearchExperience.Footer.TurnOnAllTheThings");
            comments[2].Value.ShouldEqual("@localize Csr.CustomSearchExperience.Footer.TurnOffHowSureAreYou");
            comments[3].Value.ShouldEqual("@localize Csr.CustomSearchExperience.Footer.TurnOnHowSureAreYou");
            comments[4].Value.ShouldEqual("@localize Csr.CustomSearchExperience.Footer.EleventyBillion");
            comments[5].Value.ShouldEqual("@localize Csr.CustomSearchExperience.Footer.EleventyBillion");
            comments[6].Value.ShouldEqual("@localize Csr.CustomSearchExperience.Footer.Cancel");
        }
    }
}
