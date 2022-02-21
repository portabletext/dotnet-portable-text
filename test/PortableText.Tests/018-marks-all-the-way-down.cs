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
                Annotations =
                {
                    {
                        "highlight",
                        new AnnotatedMarkSerializer
                        {
                            Type = typeof(Highlight),
                            Serialize = (value, _) =>
                            {
                                var highlight = value as Highlight;
                                return ($@"<span style=""border:{highlight.Thickness}px solid"">", "</span>");
                            }
                        }
                    }
                }
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
        
        
        // TODO: Technically more correct, as it joins other marks when they are the same.
        // result.Should().Be(string.Join("",
        //     "<p>",
        //     @"<span style=""border:1px solid"">",
        //     @"<span style=""border:3px solid"">",
        //     "<em>Sanity FTW</em>",
        //     "</span>",
        //     "</span>",
        //     "</p>"
        // ));
        result.Should().Be(string.Join("",
            "<p>",
            @"<span style=""border:1px solid"">",
            "<em>",
            @"<span style=""border:3px solid"">",
            "Sanity",
            "</span>",
            "</em>",
            "</span>",
            @"<span style=""border:3px solid"">",
            @"<span style=""border:1px solid"">",
            "<em> FTW</em>",
            "</span>",
            "</span>",
            "</p>"
        ));
    }
}