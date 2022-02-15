using FluentAssertions;
using Xunit;

namespace PortableText;

public partial class Tests
{
    [Fact]
    public void CustomMarks()
    {
        var serializers = new PortableTextSerializers
        {
            MarkSerializers = new()
            {
                { "highlight", (block, child, mark) => (@"<span style=""border:VALUEpx solid"">", "</span>") }
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
                ""marks"": [""mark1""],
                ""text"": ""Sanity""
            }
        ],
        ""markDefs"": [
            {
                ""_key"": ""mark1"",
                ""_type"": ""highlight"",
                ""thickness"": 5
            }
        ]
    }
]
", serializers);
        
        // TODO: This test fails because we don't support de-serializing custom marks
        result.Should().Be(@"<p><span style=""border:5px solid"">Sanity</span></p>");
    }
}