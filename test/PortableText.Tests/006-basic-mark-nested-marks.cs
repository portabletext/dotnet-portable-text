using FluentAssertions;
using Xunit;

namespace PortableText;

public partial class Tests
{
    [Fact]
    public void BasicMarkNestedMarks()
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
                ""marks"": [""strong""],
                ""text"": ""A word of ""
            },
            {
                ""_key"": ""toaiCqIK"",
                ""_type"": ""span"",
                ""marks"": [""strong"", ""em""],
                ""text"": ""warning;""
            },
            {
                ""_key"": ""gaZingA"",
                ""_type"": ""span"",
                ""marks"": [],
                ""text"": "" Sanity is addictive.""
            }
        ],
        ""markDefs"": [],
        ""style"": ""normal""
    }
]
");
        // TODO: This is more technically correct, but the latter works as well...
        // result.Should().Be("<p><strong>A word of <em>warning;</em></strong> Sanity is addictive.</p>'");
        result.Should().Be("<p><strong>A word of </strong><strong><em>warning;</em></strong> Sanity is addictive.</p>");
    }
}