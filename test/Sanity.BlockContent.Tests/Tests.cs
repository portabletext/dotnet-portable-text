using System;
using Sanity;
using Xunit;
using FluentAssertions;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;
using Xunit.Abstractions;
using System.Collections.Generic;
using Snapshooter.Xunit;

namespace Tests
{
    public class YoutubeEmbed : PortableTextBlock
    {
        public string Title { get; set; }
        public string Url { get; set; }
    }

    public class Test
    {
        private readonly ITestOutputHelper output;

        public Test(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void HandlesNull()
        {
            var result = BlockContentToHtml.Render(null);
            result.Should().BeNull();
        }

        [Fact]
        public void AcceptsEmptyJson()
        {
            var result = BlockContentToHtml.Render(string.Empty);
            result.Should().BeNull();
        }

        [Fact]
        public void HandlesEmptyJsonArray()
        {
            var result = BlockContentToHtml.Render("[]");

            result.Should().Be(null);
        }

        [Fact]
        public void HandlesJsonArrayWithEmptyObject()
        {
            var result = BlockContentToHtml.Render("[{}]");

            result.Should().Be(null);
        }

        [Fact]
        public void GivenOneSimpleArrayElementOfTypeBlock()
        {
            var result = BlockContentToHtml.Render(@"
[
    {
        ""_type"": ""block"",
        ""children"": [
            {
                ""_type"": ""span"",
                ""text"": ""test""
            }
        ],
        ""style"": ""normal""
    }
]
");
                
            result.Should().Be("<p>test</p>");
        }

        [Fact]
        public void GivenMultipleSimpleArrayElementOfTypeBlock_ShouldCombineText()
        {
            var result = BlockContentToHtml.Render(@"
[
    {
        ""_type"": ""block"",
        ""children"": [
            {
                ""_type"": ""span"",
                ""text"": ""test""
            },
            {
                ""_type"": ""span"",
                ""text"": ""test2""
            }
        ],
        ""style"": ""normal""
    }
]
");

            result.Should().Be("<p>testtest2</p>");
        }

        [Fact]
        public void ConvertsSlashNToBr()
        {
            var result = BlockContentToHtml.Render(@"
[
    {
        ""_type"": ""block"",
        ""children"": [
            {
                ""_type"": ""span"",
                ""text"": ""test\ntest2""
            }
        ],
        ""style"": ""normal""
    }
]
");

            result.Should().Be("<p>test<br>test2</p>");
        }

        [Fact]
        public void HandlesSimpleJson()
        {
            var json = ReadTestJsonFile("simple.json");
            var result = BlockContentToHtml.Render(json);

            result.Should().Be("<p>Jeg er <strong>kul</strong>!</p>");
        }

        [Fact]
        public void HandlesLinks()
        {
            var json = ReadTestJsonFile("links.json");
            var result = BlockContentToHtml.Render(json);

            result.Should().Be(@"<p><strong>Spotify -> </strong><strong><a href=""https://open.spotify.com/episode/2ON1aZSJvYieU8Pewz7yUH?si=KARaXtuaQDaOkeyxs0f7OQ"">Lytt her!</a></strong></p>");
        }

        [Fact]
        public void GivenNoCustomSerializers_AndCustomObjectsArePresent_ShouldNotCrash()
        {
            var json = ReadTestJsonFile("customobjects.json");
            var result = BlockContentToHtml.Render(json);

            result.Should().Be(null);
        }

        [Fact]
        public void GivenCustomSerializers_AndCustomObjectsArePresent()
        {
            var json = ReadTestJsonFile("customobjects.json");
            var result = BlockContentToHtml.Render(json, new PortableTextSerializers
            {
                TypeSerializers = new Dictionary<string, TypeSerializer>
                {
                    {
                        "youtubeEmbed", new TypeSerializer
                        {
                            Type = typeof(YoutubeEmbed),
                            Serialize = (block, serializers) =>
                            {
                                var typedBlock = block as YoutubeEmbed;
                                if (typedBlock == null)
                                {
                                    return string.Empty;
                                }

                                return $@"<iframe title=""{typedBlock.Title}"" href=""{typedBlock.Url}""></iframe>";
                            }
                        }
                    }
                }
            });

            result.Should().Be(@"<iframe title=""Top 10 goals Jon Dahl Tomasson"" href=""https://youtu.be/8d9vXiGrYck""></iframe>");
        }
        
        [Fact]
        public void List()
        {
            var result = BlockContentToHtml.Render(ReadTestJsonFile("list.json"));
            SnapshotExtensions.MatchFormattedHtml(result);
        }

        [Fact]
        public void MassiveTest()
        {
            var result = BlockContentToHtml.Render(ReadTestJsonFile("bigcontent.json"));
            SnapshotExtensions.MatchFormattedHtml(result);
        }

        private static string ReadTestJsonFile(string filename)
        {
            return File.ReadAllText($"../../../data/{filename}");
        }
    }

    public static class SnapshotExtensions
    {
        public static void MatchFormattedHtml(string html)
        {
            var parser = new AngleSharp.Html.Parser.HtmlParser();
            using var document = parser.ParseDocument(html);
            using var sw = new StringWriter();
            var formatter = new AngleSharp.Html.PrettyMarkupFormatter { Indentation = "  ", NewLine = Environment.NewLine };
            document.ToHtml(sw, formatter);
            Snapshot.Match(sw.ToString());
        }
    }
}
