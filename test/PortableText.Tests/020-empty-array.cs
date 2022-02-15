using FluentAssertions;
using Xunit;

namespace PortableText;

public partial class Tests
{
    [Fact]
    public void EmptyArray()
    {
        var result = PortableTextToHtml.Render("[]");
        
        result.Should().Be(null);
    }
}