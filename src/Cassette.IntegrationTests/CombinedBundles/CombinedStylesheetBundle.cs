using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using Cassette.BundleProcessing;
using Cassette.Configuration;
using Cassette.IO;
using Cassette.MSBuild;
using Cassette.ScriptAndTemplate;
using Cassette.Stylesheets;
using Microsoft.Scripting.Hosting.Shell;
using Moq;
using Should;
using Xunit;

namespace Cassette.IntegrationTests
{
    public class CombinedStylesheetBundle_Tests
    {
        CombinedStylesheetBundle underTest;
        StylesheetBundle inner;
        CassetteSettings settings;

        class InMemoryCssFile : IFile
        {
            public IDirectory Directory
            {
                get
                {
                    var mock = new Mock<IDirectory>();
                    mock.Setup(x => x.FullPath).Returns("/foo");
                    return mock.Object;
                }
            }

            public bool Exists
            {
                get { return true; }
            }

            public DateTime LastWriteTimeUtc
            {
                get { return new DateTime(); }
            }

            public string FullPath
            {
                get { return "/foo/foo.css"; }
            }

            public System.IO.Stream Open(System.IO.FileMode mode, System.IO.FileAccess access, System.IO.FileShare fileShare)
            {
                return new MemoryStream(
                    System.Text.Encoding.UTF8.GetBytes(
                        ".className { display: none; }\n"
                    )
                );
            }

            public void Delete()
            {
            }
        }

        public CombinedStylesheetBundle_Tests()
        {
            settings = new CassetteSettings("1");
            var factory = new StylesheetBundleFactory(settings);
            
            var bd = new BundleDescriptor();
            bd.AssetFilenames.Add("*");
            inner = factory.CreateBundle("foo", new IFile[]{new InMemoryCssFile()} , bd);

            underTest = new CombinedStylesheetBundle(
                "test bundle",
                new List<StylesheetBundle>(new StylesheetBundle[] { inner }),
                new List<string>(new string[] {"foo"})
            );
        }

        [Fact]
        public void GivenCombinedStyleSheetBundle_WhenRenderedAndMinified_ThenCssIsProperlyMinifiedAsCssAndNotJavascript()
        {
            CassetteSettings settings = new CassetteSettings("1");
            IBundleProcessor<StylesheetBundle> processor = new StylesheetPipeline();
            underTest.Processor = processor;
            inner.Processor = processor;
            underTest.Process(settings);
            // underTest.Renderer = new StylesheetHtmlRenderer(new UrlGenerator(new UrlPlaceholderWrapper(), ""));
            var cssContent = underTest.OpenStream();
            string output = new StreamReader(cssContent).ReadToEnd();
            output.ShouldEqual(".className{display:none}");
        }
    }
}
