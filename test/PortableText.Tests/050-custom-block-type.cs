using System.Web;
using FluentAssertions;
using Xunit;

namespace PortableText;

public partial class Tests
{
    [Fact]
    public void CustomBlockType()
    {
        var serializers = new PortableTextSerializers
        {
            TypeSerializers = new()
            {
                {
                    "code",
                    new TypeSerializer
                    {
                        Type = typeof(CodeBlock),
                        Serialize = (value, _, _, _) =>
                        {
                            var code = value as CodeBlock;
                            return $@"<pre data-language=""{code.Language}""><code>{HttpUtility.HtmlEncode(code.Code)}</code></pre>";
                        }
                    }
                }
            }
        };
        var result = PortableTextToHtml.Render(@"
[
    {
        ""_type"": ""code"",
        ""_key"": ""9a15ea2ed8a2"",
        ""language"": ""javascript"",
        ""code"": ""const foo = require('foo')\n\nfoo('hi there', (err, thing) => {\n  console.log(err)\n})\n""
    }
]
", serializers);
        
        result.Should().Be(string.Join("",
            @"<pre data-language=""javascript"">",
            "<code>",
            "const foo = require(&#39;foo&#39;)\n\n",
            "foo(&#39;hi there&#39;, (err, thing) =&gt; {\n",
            "  console.log(err)\n",
            "})\n",
            "</code></pre>"
        ));
    }

    private class CodeBlock
    {
        public string Language { get; set; }
        public string Code { get; set; }
    }
}

