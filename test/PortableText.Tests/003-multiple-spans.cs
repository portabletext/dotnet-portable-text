using FluentAssertions;
using Xunit;

namespace PortableText;

public partial class Tests
{
    [Fact]
    public void MultipleSpans()
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
                ""marks"": [],
                ""text"": ""Span number one. ""
            },
            {
                ""_key"": ""toaiCqIK"",
                ""_type"": ""span"",
                ""marks"": [],
                ""text"": ""And span number two.""
            }
        ],
        ""markDefs"": [],
        ""style"": ""normal""
    }
]
");
        result.Should().Be("<p>Span number one. And span number two.</p>");
    }
}