using FluentAssertions;
using Xunit;

namespace PortableText;

public partial class Tests
{
    [Fact]
    public void PlainHeaderBlock()
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
                ""text"": ""Dat heading""
            }
        ],
        ""markDefs"": [],
        ""style"": ""h2""
    }
]
");
        result.Should().Be("<h2>Dat heading</h2>");
    }
}