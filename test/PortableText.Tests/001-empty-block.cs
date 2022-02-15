using FluentAssertions;
using Xunit;

namespace PortableText;

public partial class Tests
{
    [Fact]
    public void EmptyBlock()
    {
        var result = PortableTextToHtml.Render(@"
[
    {
        ""_key"": ""R5FvMrjo"",
        ""_type"": ""block"",
        ""children"": [],
        ""markDefs"": [],
        ""style"": ""normal""
    }
]
");
        result.Should().Be("");
    }
}