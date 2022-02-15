using FluentAssertions;
using Xunit;

namespace PortableText;

public partial class Tests
{
    [Fact]
    public void MarksAllTheWayDown()
    {
        var serializers = new PortableTextSerializers
        {
            MarkSerializers = new()
            {
                // TODO: we don't support custom mark objects de-serialized, so it's impossible to access custom fields on the markDef
                { "highlight", (block, child, mark) => ($@"<span style=""border: VALUEpx solid""", "") }
            }
        };
        var result = PortableTextToHtml.Render(@"
[
    {
        ""_type"": ""block"",
        ""children"": [
            {
                ""_key"": ""a1ph4"",
                ""_type"": ""span"",
                ""marks"": [""mark1"", ""em"", ""mark2""],
                ""text"": ""Sanity""
            },
            {
                ""_key"": ""b374"",
                ""_type"": ""span"",
                ""marks"": [""mark2"", ""mark1"", ""em""],
                ""text"": "" FTW""
            }
        ],
        ""markDefs"": [
            {
                ""_type"": ""highlight"",
                ""_key"": ""mark1"",
                ""thickness"": 1
            },
            {
                ""_type"": ""highlight"",
                ""_key"": ""mark2"",
                ""thickness"": 3
            }
        ],
        ""style"": ""normal""
    }
]
", serializers);
        
        // TODO: This test fails, as we don't support de-serializing to custom mark objects
        result.Should().Be(string.Join("",
            "<p>",
               @"<span style=""border:1px solid"">",
               @"<span style=""border:3px solid"">",
                "<em>Sanity FTW</em>",
                "</span>",
                "</span>",
            "</p>"
        ));
    }
}