using System.Collections.Generic;
using Xunit;
using FluentAssertions;
using System.IO;

namespace PortableText
{
    public partial class Tests
    {
        [Fact]
        public void HandlesNull()
        {
            var result = PortableTextToHtml.Render(null);
            result.Should().BeNull();
        }

        [Fact]
        public void AcceptsEmptyJson()
        {
            var result = PortableTextToHtml.Render(string.Empty);
            result.Should().BeNull();
        }

        [Fact]
        public void ThrowsOnInvalidJson()
        {
            var success = false;
            try
            {
                PortableTextToHtml.Render("[{]");
                success = true;
            }
            catch
            {
                // ignored
            }

            Assert.False(success);
        }

        [Fact]
        public void HandlesEmptyJsonArray()
        {
            var result = PortableTextToHtml.Render("[]");

            result.Should().BeNull();
        }

        [Fact]
        public void HandlesJsonArrayWithEmptyObject()
        {
            var result = PortableTextToHtml.Render("[{}]");

            result.Should().BeNull();
        }

        [Fact]
        public void GivenNoCustomSerializers_AndCustomObjectsArePresent_ShouldNotCrash()
        {
            const string json = @"
[
    {
        ""_key"": ""88778f6b7024"",
        ""_type"": ""youtubeEmbed"",
        ""title"": ""Top 10 goals Jon Dahl Tomasson"",
        ""url"": ""https://youtu.be/8d9vXiGrYck""
    }
]
";
            var result = PortableTextToHtml.Render(json);

            result.Should().BeNull();
        }

        [Fact]
        public void GivenNoCustomSerializers_AndDecoratorMarksArePresent_ShouldNotCrash()
        {
            const string json = @"
[
    {
        ""_type"": ""block"",
        ""children"": [
            {
                ""_type"": ""span"",
                ""text"": ""Test"",
                ""marks"": [""notDefined""]
            }
        ]
    }
]
";

            var result = PortableTextToHtml.Render(json);
            result.Should().Be("<p>Test</p>");
        }
        
        [Fact]
        public void GivenNoCustomSerializers_AndAnnotationMarksArePresent_ShouldNotCrash()
        {
            const string json = @"
[
    {
        ""_type"": ""block"",
        ""children"": [
            {
                ""_type"": ""span"",
                ""text"": ""Test"",
                ""marks"": [""notDefinedKey""]
            }
        ],
        ""markDefs"": [
            {
                ""_type"": ""notDefined"",
                ""_key"": ""notDefinedKey"",
                ""something"": ""test""
            }
        ]
    }
]
";

            var result = PortableTextToHtml.Render(json);
            result.Should().Be("<p>Test</p>");
        }
        
        [Fact]
        public void GivenSerializerWithSameName_OnDecoratorMark_AndOnAnnotationMark_PrioritizesAnnotationMark()
        {
            var serializers = new PortableTextSerializers
            {
                MarkSerializers = new()
                {
                    Annotations = new()
                    {
                        {
                            "highlighting",
                            new()
                            {
                                Type = typeof(Highlighting),
                                Serialize = (value, rawValue) =>
                                {
                                    var highlighting = value as Highlighting;

                                    return (@$"<span style=""color: {highlighting.HexColor}"">", "</span>");
                                }
                            }
                        }
                    },
                    Decorators = new()
                    {
                        {
                            "highlighting", () => ("<em>", "</em>")
                        }
                    }
                }
            };
            
            const string json = @"
[
    {
        ""_type"": ""block"",
        ""children"": [
            {
                ""_type"": ""span"",
                ""text"": ""Test"",
                ""marks"": [""highlighting""]
            }
        ],
        ""markDefs"": [
            {
                ""_type"": ""highlighting"",
                ""_key"": ""highlighting"",
                ""hexColor"": ""#888""
            }
        ]
    }
]
";

            var result = PortableTextToHtml.Render(json, serializers);
            result.Should().Be(@"<p><span style=""color: #888"">Test</span></p>");
        }
        
        private class Highlighting
        {
            public string HexColor { get; set; }
        }
    }
}