using System.Linq;
using Should;
using Xunit;

namespace Cassette.Scripts
{
    public class JavaScriptCommentParser_Tests
    {
        [Fact]
        public void WhenParseSingleLineComment_ThenReturnOneComment()
        {
            var parser = new JavaScriptCommentParser();
            var comment = parser.Parse("// text").Single();
            comment.LineNumber.ShouldEqual(1);
            comment.Value.ShouldEqual(" text");
        }

        [Fact]
        public void WhenParseTwoSingleLineComments_ThenReturnTwoComments()
        {
            var parser = new JavaScriptCommentParser();
            var comments = parser.Parse("// text1\r\n// text2").ToArray();
            comments[0].LineNumber.ShouldEqual(1);
            comments[0].Value.ShouldEqual(" text1");
            comments[1].LineNumber.ShouldEqual(2);
            comments[1].Value.ShouldEqual(" text2");
        }

        [Fact]
        public void WhenParseTwoSingleLineCommentsSeperatedByUnixNewLine_ThenReturnTwoComments()
        {
            var parser = new JavaScriptCommentParser();
            var comments = parser.Parse("// text1\n// text2").ToArray();
            comments[0].LineNumber.ShouldEqual(1);
            comments[0].Value.ShouldEqual(" text1");
            comments[1].LineNumber.ShouldEqual(2);
            comments[1].Value.ShouldEqual(" text2");
        }

        [Fact]
        public void WhenParseMultilineComment_ThenReturnCommentPerLine()
        {
            var parser = new JavaScriptCommentParser();
            var comments = parser.Parse("/*text1\r\ntext2*/").ToArray();
            comments[0].LineNumber.ShouldEqual(1);
            comments[0].Value.ShouldEqual("text1");
            comments[1].LineNumber.ShouldEqual(2);
            comments[1].Value.ShouldEqual("text2");
        }

        [Fact]
        public void WhenParseMultilineCommentWithUnixNewLines_ThenReturnCommentPerLine()
        {
            var parser = new JavaScriptCommentParser();
            var comments = parser.Parse("/*text1\ntext2*/").ToArray();
            comments[0].LineNumber.ShouldEqual(1);
            comments[0].Value.ShouldEqual("text1");
            comments[1].LineNumber.ShouldEqual(2);
            comments[1].Value.ShouldEqual("text2");
        }

        [Fact]
        public void WhenParseLocalizedStringOnOneLineSingleQuotes_ReturnCorrectComment()
        {
            var parser = new JavaScriptCommentParser();
            var comment = parser.Parse("var x = i18n.t('Localized.String.Here');").Single();
            comment.LineNumber.ShouldEqual(1);
            comment.Value.ShouldEqual("@localize Localized.String.Here");
        }

        [Fact]
        public void WhenParseLocalizedStringOnOneLineDoubleQuotes_ReturnCorrectComment()
        {
            var parser = new JavaScriptCommentParser();
            var comment = parser.Parse("var x = i18n.t(\"Localized.String.Here\");").Single();
            comment.LineNumber.ShouldEqual(1);
            comment.Value.ShouldEqual("@localize Localized.String.Here");
        }

        [Fact]
        public void WhenParseLocalizedStringOnOneLineWeirdSpacing_ReturnCorrectComment()
        {
            var parser = new JavaScriptCommentParser();
            var comment = parser.Parse("var x = i18n.t(   'Localized.String.Here' );").Single();
            comment.LineNumber.ShouldEqual(1);
            comment.Value.ShouldEqual("@localize Localized.String.Here");
        }

        [Fact]
        public void WhenParseLocalizedStringOnMultipleLines_ReturnCorrectComment()
        {
            var parser = new JavaScriptCommentParser();
            var comment = parser.Parse("var x = i18n.t(\r'Localized.String.Here'\n);").Single();
            comment.LineNumber.ShouldEqual(2);
            comment.Value.ShouldEqual("@localize Localized.String.Here");
        }

        [Fact]
        public void WhenParseLocalizedStringComplex_DontReturnInvalidComments()
        {
            var js =
                @"Hogan.Template.prototype.render = function render(context, partials, indent) {
    
    context = context || {};
    
    context.i18n = function () {
        return function (s) {
            
            if (!i18n
                || typeof i18n.t !== 'function')
            {
                throw new Error('Could not find locale data.');
            }
            
            if (!i18n.t(s))
            {
                throw new Error('Could not find locale data for key: ""' + s + '""');
            }
            
            return i18n.t(s);
        };
    };
    
    return this.ri([context], partials || {}, indent);
};";
            var parser = new JavaScriptCommentParser();
            var comments = parser.Parse(js).ToArray();
            comments.Length.ShouldEqual(0);
        }

        [Fact]
        public void WhenParseAbConfigIsOnOnOneLineSingleQuotes_ReturnCorrectComment()
        {
            var parser = new JavaScriptCommentParser();
            var comment = parser.Parse("var x = AB.isOn('AbConfigs.Test.Name');").Single();
            comment.LineNumber.ShouldEqual(1);
            comment.Value.ShouldEqual("@abconfig AbConfigs.Test.Name");
        }

        [Fact]
        public void WhenParseAbConfigGetVariantOnOneLineSingleQuotes_ReturnCorrectComment()
        {
            var parser = new JavaScriptCommentParser();
            var comment = parser.Parse("var x = AB.getVariant('AbConfigs.Test.Name');").Single();
            comment.LineNumber.ShouldEqual(1);
            comment.Value.ShouldEqual("@abconfig AbConfigs.Test.Name");
        }

        [Fact]
        public void WhenParseAbConfigIsOnOnOneLineDoubleQuotes_ReturnCorrectComment()
        {
            var parser = new JavaScriptCommentParser();
            var comment = parser.Parse("var x = AB.isOn(\"AbConfigs.Test.Name\");").Single();
            comment.LineNumber.ShouldEqual(1);
            comment.Value.ShouldEqual("@abconfig AbConfigs.Test.Name");
        }

        [Fact]
        public void WhenParseAbConfigGetVariantOnOneLineDoubleQuotes_ReturnCorrectComment()
        {
            var parser = new JavaScriptCommentParser();
            var comment = parser.Parse("var x = AB.getVariant(\"AbConfigs.Test.Name\");").Single();
            comment.LineNumber.ShouldEqual(1);
            comment.Value.ShouldEqual("@abconfig AbConfigs.Test.Name");
        }

        [Fact]
        public void WhenParseAbConfigIsOnOnOneLineWeirdSpacing_ReturnCorrectComment()
        {
            var parser = new JavaScriptCommentParser();
            var comment = parser.Parse("var x = AB.isOn(   'AbConfigs.Test.Name' );").Single();
            comment.LineNumber.ShouldEqual(1);
            comment.Value.ShouldEqual("@abconfig AbConfigs.Test.Name");
        }

        [Fact]
        public void WhenParseAbConfigGetVariantOnOneLineWeirdSpacing_ReturnCorrectComment()
        {
            var parser = new JavaScriptCommentParser();
            var comment = parser.Parse("var x = AB.getVariant(   'AbConfigs.Test.Name' );").Single();
            comment.LineNumber.ShouldEqual(1);
            comment.Value.ShouldEqual("@abconfig AbConfigs.Test.Name");
        }

        [Fact]
        public void WhenParseAbConfigIsOnOnMultipleLines_ReturnCorrectComment()
        {
            var parser = new JavaScriptCommentParser();
            var comment = parser.Parse("var x = AB.isOn(\r'AbConfigs.Test.Name'\n);").Single();
            comment.LineNumber.ShouldEqual(2);
            comment.Value.ShouldEqual("@abconfig AbConfigs.Test.Name");
        }

        [Fact]
        public void WhenParseAbConfigGetVariantOnMultipleLines_ReturnCorrectComment()
        {
            var parser = new JavaScriptCommentParser();
            var comment = parser.Parse("var x = AB.getVariant(\r'AbConfigs.Test.Name'\n);").Single();
            comment.LineNumber.ShouldEqual(2);
            comment.Value.ShouldEqual("@abconfig AbConfigs.Test.Name");
        }

        [Fact]
        public void WhenParseAbConfigIsOnComplex_DontReturnInvalidComments()
        {
            var js =
                @"Hogan.Template.prototype.render = function render(context, partials, indent) {
    
    context = context || {};
    
    context.AB = function () {
        return function (s) {
            
            if (!AB
                || typeof AB.isOn !== 'function')
            {
                throw new Error('Could not check if AB test is on.');
            }
            
            if (!AB.isOn(s))
            {
                throw new Error('Could not check if AB test is on for test: ""' + s + '""');
            }
            
            return AB.isOn(s);
        };
    };
    
    return this.ri([context], partials || {}, indent);
};";
            var parser = new JavaScriptCommentParser();
            var comments = parser.Parse(js).ToArray();
            comments.Length.ShouldEqual(0);
        }

        [Fact]
        public void WhenParseAbConfigGetVariantComplex_DontReturnInvalidComments()
        {
            var js =
                @"Hogan.Template.prototype.render = function render(context, partials, indent) {
    
    context = context || {};
    
    context.AB = function () {
        return function (s) {
            
            if (!AB
                || typeof AB.getVariant !== 'function')
            {
                throw new Error('Could not check if AB test is on.');
            }
            
            if (!AB.getVariant(s))
            {
                throw new Error('Could not check if AB test is on for test: ""' + s + '""');
            }
            
            return AB.getVariant(s);
        };
    };
    
    return this.ri([context], partials || {}, indent);
};";
            var parser = new JavaScriptCommentParser();
            var comments = parser.Parse(js).ToArray();
            comments.Length.ShouldEqual(0);
        }
    }
}
