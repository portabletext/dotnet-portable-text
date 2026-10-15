using Shouldly;
using Xunit;

namespace PortableText;

public partial class Tests
{
    [Fact]
    public void SingleSpan()
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
                ""text"": ""Plain text.""
            }
        ],
        ""markDefs"": [],
        ""style"": ""normal""
    }
]
");
        result.ShouldBe("<p>Plain text.</p>");
    }
}