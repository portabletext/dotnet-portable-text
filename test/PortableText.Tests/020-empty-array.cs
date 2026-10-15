using Shouldly;
using Xunit;

namespace PortableText;

public partial class Tests
{
    [Fact]
    public void EmptyArray()
    {
        var result = PortableTextToHtml.Render("[]");
        
        result.ShouldBe(null);
    }
}