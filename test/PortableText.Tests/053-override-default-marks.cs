using FluentAssertions;
using Xunit;

namespace PortableText;

public partial class Tests
{
    [Fact]
    public void OverrideDefaultMarks()
    {
        var serializers = new PortableTextSerializers
        {
            MarkSerializers = new()
            {
                Annotations =
                {
                    {
                        "link",
                        new AnnotatedMarkSerializer
                        {
                            Type = typeof(LinkPortableTextMarkAnnotation),
                            Serialize = (value, _) =>
                            {
                                var highlight = value as LinkPortableTextMarkAnnotation;
                                return ($@"<a class=""mahlink"" href=""https://sanity.io"">", "</a>");
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
                ""_type"": ""link"",
                ""href"": ""https://sanity.io""
            }
        ]
    }
]
", serializers);
        
        result.Should().Be(@"<p><a class=""mahlink"" href=""https://sanity.io"">Sanity</a></p>");
    }
}