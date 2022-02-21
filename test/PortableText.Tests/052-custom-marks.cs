using FluentAssertions;
using Xunit;

namespace PortableText;

public partial class Tests
{
    public class Highlight
    {
        public int Thickness { get; set; }
    }
    
    [Fact]
    public void CustomMarks()
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
        
        result.Should().Be(@"<p><span style=""border:5px solid"">Sanity</span></p>");
    }
}