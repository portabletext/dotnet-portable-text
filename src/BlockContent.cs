using System.Linq;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Collections.Generic;
using System;

namespace Sanity
{
    public class MarkDefinition
    {
        [JsonPropertyName("_key")]
        public string Key { get; set; }

        [JsonPropertyName("_type")]
        public string Type { get; set; }

        public string Href { get; set; }
    }

    public class PortableTextBlock
    {
        [JsonPropertyName("_key")]
        public string Key { get; set; }
        public PortableTextChild[] Children { get; set; }
        [JsonPropertyName("_type")]
        public string Type { get; set; }
        [JsonPropertyName("markDefs")]
        public MarkDefinition[] MarkDefinitions { get; set; }
        public string Style { get; set; }
    }

    public class PortableTextChild
    {
        [JsonPropertyName("_key")]
        public string Key { get; set; }
        [JsonPropertyName("_type")]
        public string Type { get; set; }
        public string[] Marks { get; set; }
        public string Text { get; set; }
    }

    public static class BlockContentToHtml
    {
        private static PortableTextSerializers GetDefaultBlockSerializers()
        {
            return new PortableTextSerializers
            {
                TypeSerializers = new Dictionary<string, Func<PortableTextBlock, PortableTextSerializers, string>>
                {
                    {
                        "block", (block, serializers) =>
                        {
                            var children = block.Children;

                            var blocks = new List<string>();
                            foreach (var blockChild in children)
                            {
                                if (blockChild.Marks == null || !blockChild.Marks.Any())
                                {
                                    blocks.Add(blockChild.Text.Replace("\n", "<br>"));
                                }
                                else
                                {
                                    var tags = blockChild.Marks.Select(mark => {
                                        var defaultSerializer = serializers.MarkSerializers.TryGetValue(mark, out var defaultMarkSerializerExists);
                                        if (defaultMarkSerializerExists != null)
                                        {
                                            return serializers.MarkSerializers[mark](block, blockChild);
                                        }

                                        return serializers.MarkSerializers[block.MarkDefinitions.First(markDef => markDef.Key == mark).Type](block, blockChild);
                                    });
                                    var startTags = tags.Select(x => x.Item1);
                                    var endTags = tags.Select(x => x.Item2).Reverse();

                                    blocks.AddRange(startTags);
                                    blocks.Add(blockChild.Text);
                                    blocks.AddRange(endTags);
                                }
                            }

                            return BlockStyleSerializers[block.Style](blocks);
                        }
                    }
                },
                MarkSerializers = new Dictionary<string, Func<PortableTextBlock, PortableTextChild, (string startTag, string endTag)>>
                {
                    { "strong", (block, blockChild) => ("<strong>", "</strong>") },
                    { "em", (block, blockChild) => ("<em>", "</em>") },
                    { "code", (block, blockChild) => ("<code>", "</code>") },
                    { "underline", (block, blockChild) => (@"<span style=""text-decoration: underline;"">", "</span>") },
                    { "strike-through", (block, blockChild) => ("<del>", "</del>") },

                    // Not happy with this. Do we really need the block in itself? Maybe implement a more dynamic approach?
                    // Actually, does this even work? Links aren't stored this way anyways.
                    { "link", (block, blockChild) => ($"<a href=\"{block.MarkDefinitions.First(x => x.Key == blockChild.Marks.First(y => y == x.Key)).Href}\">", "</a>") }
                }
            };
        }

        private static Dictionary<string, Func<IEnumerable<string>, string>> BlockStyleSerializers =>
            new Dictionary<string, Func<IEnumerable<string>, string>>
            {
                { "normal", blocks => $"<p>{string.Join(string.Empty, blocks)}</p>" },
                { "h1", blocks => $"<h1>{string.Join(string.Empty, blocks)}</h1>" },
                { "h2", blocks => $"<h2>{string.Join(string.Empty, blocks)}</h2>" },
                { "h3", blocks => $"<h3>{string.Join(string.Empty, blocks)}</h3>" },
                { "h4", blocks => $"<h4>{string.Join(string.Empty, blocks)}</h4>" },
                { "h5", blocks => $"<h5>{string.Join(string.Empty, blocks)}</h5>" },
                { "h6", blocks => $"<h6>{string.Join(string.Empty, blocks)}</h6>" },
                { "blockquote", blocks => $"<blockquote>{string.Join(string.Empty, blocks)}</blockquote>" }
            };

        public static string Render(string json, PortableTextSerializers customSerializers = null)
        {
            var serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };

            if (string.IsNullOrWhiteSpace(json))
            {
                return null;
            }
            var blocks = System.Text.Json.JsonSerializer.Deserialize<PortableTextBlock[]>(json, serializeOptions);
            return Render(blocks);
        }

        public static string Render(PortableTextBlock[] portableTextBlocks)
        {
            if (portableTextBlocks == null || !portableTextBlocks.Any())
            {
                return null;
            }

            var serializers = GetDefaultBlockSerializers();

            var stuff = new List<string>();
            foreach(var portableTextBlock in portableTextBlocks)
            {
                if (string.IsNullOrWhiteSpace(portableTextBlock.Type) ||
                    string.IsNullOrWhiteSpace(portableTextBlock.Style) ||
                    !portableTextBlock.Children.Any())
                    {
                        continue;
                    }
                stuff.Add(serializers.TypeSerializers[portableTextBlock.Type](portableTextBlock, serializers));
            }

            if (!stuff.Any())
            {
                return null;
            }

            return string.Join("", stuff);
        }
    }

    

    public class PortableTextSerializers
    {
        public Dictionary<string, Func<PortableTextBlock, PortableTextSerializers, string>> TypeSerializers { get; set; }
        public Dictionary<string, Func<PortableTextBlock, PortableTextChild, (string, string)>> MarkSerializers { get; set; }
    }
}
