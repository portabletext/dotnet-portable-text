using System.Collections.Generic;
using Xunit;
using Shouldly;
using System.IO;

namespace PortableText
{
    public partial class Tests
    {
        [Fact]
        public void HandlesNull()
        {
            var result = PortableTextToHtml.Render(null);
            result.ShouldBeNull();
        }

        [Fact]
        public void AcceptsEmptyJson()
        {
            var result = PortableTextToHtml.Render(string.Empty);
            result.ShouldBeNull();
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

            result.ShouldBeNull();
        }

        [Fact]
        public void HandlesJsonArrayWithEmptyObject()
        {
            var result = PortableTextToHtml.Render("[{}]");

            result.ShouldBeNull();
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

            result.ShouldBeNull();
        }

        [Fact]
        public void GivenUnknownBlockStyle_RendersNormalBlock()
        {
            const string json = @"
[
    {
        ""_type"": ""block"",
        ""style"": ""unknown"",
        ""children"": [
            { ""_type"": ""span"", ""text"": ""Test"" }
        ],
        ""markDefs"": []
    }
]
";
            var result = PortableTextToHtml.Render(json);

            result.ShouldBe("<p>Test</p>");
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
            result.ShouldBe("<p>Test</p>");
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
            result.ShouldBe("<p>Test</p>");
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
            result.ShouldBe(@"<p><span style=""color: #888"">Test</span></p>");
        }

        [Fact]
        public void GivenMarkedSpan_CallsEachMarkSerializerOnce()
        {
            var annotationCalls = 0;
            var decoratorCalls = 0;
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
                                    annotationCalls++;
                                    return (@$"<span style=""color: {((Highlighting)value).HexColor}"">", "</span>");
                                }
                            }
                        }
                    },
                    Decorators = new()
                    {
                        {
                            "strong", () =>
                            {
                                decoratorCalls++;
                                return ("<strong>", "</strong>");
                            }
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
                ""marks"": [""highlightingKey"", ""strong""]
            }
        ],
        ""markDefs"": [
            {
                ""_type"": ""highlighting"",
                ""_key"": ""highlightingKey"",
                ""hexColor"": ""#888""
            }
        ]
    }
]
";

            var result = PortableTextToHtml.Render(json, serializers);
            result.ShouldBe(@"<p><span style=""color: #888""><strong>Test</strong></span></p>");
            annotationCalls.ShouldBe(1);
            decoratorCalls.ShouldBe(1);
        }

        [Fact]
        public void GivenBlockWithNumericKey_ShouldNotCrash()
        {
            const string json = @"
[
    {
        ""_type"": ""block"",
        ""_key"": 123,
        ""children"": [
            {
                ""_type"": ""span"",
                ""text"": ""Test""
            }
        ]
    }
]
";

            var result = PortableTextToHtml.Render(json);
            result.ShouldBe("<p>Test</p>");
        }

        [Fact]
        public void GivenMarkDefinitionWithoutKey_ShouldNotCrash()
        {
            const string json = @"
[
    {
        ""_type"": ""block"",
        ""children"": [
            {
                ""_type"": ""span"",
                ""text"": ""Test"",
                ""marks"": [""linkKey""]
            }
        ],
        ""markDefs"": [
            {
                ""_type"": ""link"",
                ""href"": ""https://example.com/without-key""
            },
            {
                ""_type"": ""link"",
                ""_key"": ""linkKey"",
                ""href"": ""https://sanity.io""
            }
        ]
    }
]
";

            var result = PortableTextToHtml.Render(json);
            result.ShouldBe(@"<p><a href=""https://sanity.io"">Test</a></p>");
        }

        private class Highlighting
        {
            public string HexColor { get; set; }
        }
    }
}