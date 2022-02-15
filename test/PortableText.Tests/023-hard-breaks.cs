using FluentAssertions;
using Xunit;

namespace PortableText;

public partial class Tests
{
    [Fact]
    public void HardBreaks()
    {
        var result = PortableTextToHtml.Render(@"
[
    {
        ""_key"": ""bd73ec5f61a1"",
        ""_type"": ""block"",
        ""children"": [
            {
                ""_type"": ""span"",
                ""marks"": [],
                ""text"": ""A paragraph\ncan have hard\n\nbreaks.""
            }
        ],
        ""markDefs"": [],
        ""style"": ""normal""
    }
]
");
        
        // TODO: This test fails as we are serializing hard breaks to <br>, not <br/>
        result.Should().Be("<p>A paragraph<br/>can have hard<br/><br/>breaks.</p>");
    }
}