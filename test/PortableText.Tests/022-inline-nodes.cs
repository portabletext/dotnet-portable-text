using FluentAssertions;
using Xunit;

namespace PortableText;

public partial class Tests
{
    private class InlineRating : PortableTextBlockChild
    {
        public new string Type { get; set; }
        public int Rating { get; set; }
    }
    
    [Fact]
    public void InlineNodes()
    {
        var serializers = new PortableTextSerializers
        {
            TypeSerializers = new()
            {
                {
                    "rating",
                    new()
                    {
                        Type = typeof(InlineRating),
                        Serialize = (value, _, _, _) =>
                        {
                            var rating = value as InlineRating;
                            return $@"<span class=""rating type-{rating.Type} rating-{rating.Rating}""></span>";
                        }
                    }
                }
            }
        };
        var result = PortableTextToHtml.Render(@"
[
    {
        ""_key"": ""bd73ec5f61a1"",
        ""_type"": ""block"",
        ""children"": [
            {
                ""_type"": ""span"",
                ""marks"": [],
                ""text"": ""I enjoyed it. It's not perfect, but I give it a strong ""
            },
            {
                ""_type"": ""rating"",
                ""_key"": ""d234a4fa317a"",
                ""type"": ""dice"",
                ""rating"": 5
            },
            {
                ""_type"": ""span"",
                ""marks"": [],
                ""text"": "", and look forward to the next season!""
            }
        ],
        ""markDefs"": [],
        ""style"": ""normal""
    },
    {
        ""_key"": ""foo"",
        ""_type"": ""block"",
        ""children"": [
            {
                ""_type"": ""span"",
                ""marks"": [],
                ""text"": ""Sibling paragraph""
            }
        ],
        ""markDefs"": [],
        ""style"": ""normal""
    }
]
", serializers);
        
        result.Should().Be(string.Join("",
            "<p>I enjoyed it. It&#39;s not perfect, but I give it a strong ",
            @"<span class=""rating type-dice rating-5""></span>",
            ", and look forward to the next season!</p>",
            "<p>Sibling paragraph</p>"
        ));
    }
}