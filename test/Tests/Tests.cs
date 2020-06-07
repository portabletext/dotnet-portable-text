using System;
using Sanity;
using Xunit;
using FluentAssertions;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;
using Snapshooter.Xunit;
using Xunit.Abstractions;

namespace Tests
{
    public class Test
    {
        private readonly ITestOutputHelper output;

        public Test(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void HandlesNullObjects()
        {
            var result = BlockContentToHtml.Render((PortableTextBlock[])null);
            result.Should().BeNull();
        }

        [Fact]
        public void HandlesNullStrings()
        {
            var result = BlockContentToHtml.Render((string)null);
            result.Should().BeNull();
        }

        [Fact]
        public void AcceptsEmptyArray()
        {
            var result = BlockContentToHtml.Render(new PortableTextBlock[]{});

            result.Should().BeNull();
        }

        [Fact]
        public void GivenOneSimpleArrayElementOfTypeBlock()
        {
            var result = BlockContentToHtml.Render(new PortableTextBlock[]
            {
                new PortableTextBlock
                {
                    Type = "block",
                    Children = new PortableTextChild[]
                    {
                        new PortableTextChild
                        {
                            Type = "span",
                            Text = "test"
                        }
                    },
                    Style = "normal"
                }
            });

            result.Should().Be("<p>test</p>");
        }

        [Fact]
        public void GivenMultipleSimpleArrayElementOfTypeBlock_ShouldCombineText()
        {
            var result = BlockContentToHtml.Render(new PortableTextBlock[]
            {
                new PortableTextBlock
                {
                    Type = "block",
                    Children = new PortableTextChild[]
                    {
                        new PortableTextChild
                        {
                            Type = "span",
                            Text = "test "
                        },
                        new PortableTextChild
                        {
                            Type = "span",
                            Text = "test2"
                        }
                    },
                    Style = "normal"
                }
            });

            result.Should().Be("<p>test test2</p>");
        }

        [Fact]
        public void ConvertsSlashNToBr()
        {
            var result = BlockContentToHtml.Render(new PortableTextBlock[]
            {
                new PortableTextBlock
                {
                    Type = "block",
                    Children = new PortableTextChild[]
                    {
                        new PortableTextChild
                        {
                            Type = "span",
                            Text = "test\ntest2"
                        }
                    },
                    Style = "normal"
                }
            });

            result.Should().Be("<p>test<br>test2</p>");
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
        public void HandlesSimpleJson()
        {
            var serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
            var json = File.ReadAllText("../../../data/simple.json");
            var stuff = JsonSerializer.Deserialize<PortableTextBlock[]>(json, serializeOptions);
            var result = BlockContentToHtml.Render(json);

            result.Should().Be("<p>Jeg er <strong>kul</strong>!</p>");
        }

        [Fact]
        public void HandlesLinks()
        {
            var serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            var json = File.ReadAllBytes("../../../data/links.json");
            var readOnlySpan = new ReadOnlySpan<byte>(json);
            var stuff = JsonSerializer.Deserialize<PortableTextBlock[]>(readOnlySpan, serializeOptions);
            var result = BlockContentToHtml.Render(stuff);

            result.Should().Be(@"<p><strong>Spotify -\u003e </strong><strong><a href=""https://open.spotify.com/episode/2ON1aZSJvYieU8Pewz7yUH?si=KARaXtuaQDaOkeyxs0f7OQ"">Lytt her!</a></strong></p>");
        }

        // [Fact]
        // public async Task MassiveTest()
        // {
        //     BlockContent[] blockContent;
        //     using (FileStream fs = File.OpenRead("../../../data/bigcontent.json"))
        //     {
        //         blockContent = await JsonSerializer.DeserializeAsync<BlockContent[]>(fs);
        //     }

        //     var result = BlockContentToHtml.Render(blockContent);
        //     result.Should().Be("dijoajdas");
        //     Snapshot.Match(result);
        // }
    }
}
