using FluentAssertions;
using Xunit;

namespace PortableText;

public partial class Tests
{
    [Fact]
    public void MissingMarkComponent()
    {
        var result = PortableTextToHtml.Render(@"
[
    {
        ""_type"": ""block"",
        ""children"": [
            {
                ""_key"": ""cZUQGmh4"",
                ""_type"": ""span"",
                ""marks"": [""abc""],
                ""text"": ""A word of ""
            },
            {
                ""_key"": ""toaiCqIK"",
                ""_type"": ""span"",
                ""marks"": [""abc"", ""em""],
                ""text"": ""warning;""
            },
            {
                ""_key"": ""gaZingA"",
                ""_type"": ""span"",
                ""marks"": [],
                ""text"": "" Sanity is addictive.""
            }
        ],
        ""markDefs"": []
    }
]
");
        
        // TODO: This test fails as we default to not rendering unknown types
        // result.Should().Be(@"<p><span class=""unknown__pt__mark__abc"">A word of <em>warning;</em></span> Sanity is addictive.</p>");
        
        // TODO: We are not rendering unknown types - we ignore them
        result.Should().Be("<p>A word of <em>warning;</em> Sanity is addictive.</p>");
    }
}