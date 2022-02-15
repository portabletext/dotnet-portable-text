using FluentAssertions;
using Xunit;

namespace PortableText;

public partial class Tests
{
    [Fact]
    public void BasicMarkSingleSpan()
    {
        var result = PortableTextToHtml.Render(@"
[
    {
        ""_key"": ""R5FvMrjo"",
        ""_type"": ""block"",
        ""children"": [
            {
                ""_key"": ""cZUQGmh4"",
                ""_type"": ""span"",
                ""marks"": [""code""],
                ""text"": ""sanity""
            },
            {
                ""_key"": ""toaiCqIK"",
                ""_type"": ""span"",
                ""marks"": [],
                ""text"": "" is the name of the CLI tool.""
            }
        ],
        ""markDefs"": [],
        ""style"": ""normal""
    }
]
");
        result.Should().Be("<p><code>sanity</code> is the name of the CLI tool.</p>");
    }
}