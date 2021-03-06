﻿using System;
using System.IO;
using System.Text;
using Cassette.Utilities;

namespace Cassette.HtmlTemplates
{
    class RegisterTemplateWithHandlebars : IAssetTransformer
    {
        readonly HtmlTemplateBundle bundle;
        readonly string javaScriptVariableName;

        public RegisterTemplateWithHandlebars(HtmlTemplateBundle bundle, string javaScriptVariableName)
        {
            this.bundle = bundle;
            this.javaScriptVariableName = javaScriptVariableName ?? "JST";
        }

        public Func<Stream> Transform(Func<Stream> openSourceStream, IAsset asset)
        {
            return delegate
            {
                var id = bundle.GetTemplateId(asset);
                var compiled = openSourceStream().ReadToEnd();
                var template = javaScriptVariableName + "['" + id + "']";
                var sb = new StringBuilder();
                
                sb.AppendLine("var " + javaScriptVariableName + " = " + javaScriptVariableName + "|| {};");
                sb.AppendLine(template + " = Handlebars.template(" + compiled + ");");
                // Create a duplicate `render` method to support our legacy Hogan implementation.
                sb.AppendLine(template + ".render = " + template + ";");
                
                return sb.ToString().AsStream();
            };
        }
    }
}