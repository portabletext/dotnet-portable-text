using FluentAssertions;
using Xunit;

namespace PortableText;

public partial class Tests
{
    [Fact]
    public void MessyLinkText()
    {
        var result = PortableTextToHtml.Render(@"
[
    {
        ""_type"": ""block"",
        ""children"": [
            {
                ""_key"": ""a1ph4"",
                ""_type"": ""span"",
                ""marks"": [""zomgLink""],
                ""text"": ""Sanity""
            },
            {
                ""_key"": ""b374"",
                ""_type"": ""span"",
                ""marks"": [],
                ""text"": "" can be used to power almost any ""
            },
            {
                ""_key"": ""ch4r1i3"",
                ""_type"": ""span"",
                ""marks"": [""zomgLink"", ""strong"", ""em""],
                ""text"": ""app""
            },
            {
                ""_key"": ""d3174"",
                ""_type"": ""span"",
                ""marks"": [""em"", ""zomgLink""],
                ""text"": "" or website""
            },
            {
                ""_key"": ""ech0"",
                ""_type"": ""span"",
                ""marks"": [],
                ""text"": "".""
            }
        ],
        ""markDefs"": [
            {
                ""_type"": ""link"",
                ""_key"": ""zomgLink"",
                ""href"": ""https://sanity.io/""
            }
        ],
        ""style"": ""blockquote""
    }
]
");
        // TODO: This is more technically correct, but the latter works as well...
        // result.Should().Be(@"<blockquote><a href=""https://sanity.io/"">Sanity</a> can be used to power almost any <a href=""https://sanity.io/""><em><strong>app</strong> or website</em></a>.</blockquote>");
        result.Should().Be(@"<blockquote><a href=""https://sanity.io/"">Sanity</a> can be used to power almost any <a href=""https://sanity.io/""><strong><em>app</em></strong></a><em><a href=""https://sanity.io/""> or website</a></em>.</blockquote>");
    }
}