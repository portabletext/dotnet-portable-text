using FluentAssertions;
using Xunit;

namespace PortableText;

public partial class Tests
{
    [Fact]
    public void CustomBlockType()
    {
        var result = PortableTextToHtml.Render(@"
[
    {
        ""_type"": ""code"",
        ""_key"": ""9a15ea2ed8a2"",
        ""language"": ""javascript"",
        ""code"": ""const foo = require('foo')\n\nfoo('hi there', (err, thing) => {\n  console.log(err)\n})\n""
    }
]
");
        // TODO: This test fails because we don't HTML encode the value
        result.Should().Be(string.Join("",
            @"<pre data-language=""javascript"">",
            "<code>",
            "const foo = require(&#x27;foo&#x27;)\n\n",
            "foo(&#x27;hi there&#x27;, (err, thing) =&gt; {\n",
            "  console.log(err)\n",
            "})\n",
            "</code></pre>"
        ));
    }
}