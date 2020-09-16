using System.Linq;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Collections.Generic;
using System;
using System.Text;

namespace Sanity
{
    public class PortableText
    {
        [JsonPropertyName("_key")]
        public string Key { get; set; }
        [JsonPropertyName("_type")]
        public string Type { get; set; }
    }

    public class MarkDefinition
    {
        [JsonPropertyName("_key")]
        public string Key { get; set; }

        [JsonPropertyName("_type")]
        public string Type { get; set; }

        public string Href { get; set; }
    }

    public class PortableTextBlock : PortableText
    {
        public PortableTextChild[] Children { get; set; }
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

    public class TypeSerializer
    {
        public Type Type { get; set; }
        public Func<object, PortableTextSerializers, string> Serialize { get; set; }
    }

    public static class BlockContentToHtml
    {
        private static JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public static PortableTextSerializers DefaultBlockSerializers = GetDefaultBlockSerializers();
        private static PortableTextSerializers GetDefaultBlockSerializers()
        {
            return new PortableTextSerializers
            {
                TypeSerializers = new Dictionary<string, TypeSerializer>
                {
                    {
                        "block", new TypeSerializer
                        {
                            Type = typeof(PortableTextBlock),
                            Serialize = (block, serializers) =>
                            {
                                var typedBlock = block as PortableTextBlock;
                                if (typedBlock == null)
                                {
                                    return string.Empty;
                                }

                                if (string.IsNullOrWhiteSpace(typedBlock.Style) ||
                                    !typedBlock.Children.Any())
                                {
                                    return string.Empty;
                                }

                                var children = typedBlock.Children;

                                var blocks = new List<string>();
                                foreach (var blockChild in children)
                                {
                                    if (blockChild.Marks == null || !blockChild.Marks.Any())
                                    {
                                        blocks.Add(blockChild.Text.Replace("\n", "<br>"));
                                    }
                                    else
                                    {
                                        var tags = blockChild.Marks.Select(mark =>
                                        {
                                            var defaultSerializer = serializers.MarkSerializers.TryGetValue(mark, out var defaultMarkSerializerExists);
                                            if (defaultMarkSerializerExists != null)
                                            {
                                                return serializers.MarkSerializers[mark](typedBlock, blockChild, mark);
                                            }

                                            return serializers.MarkSerializers[typedBlock.MarkDefinitions.First(markDef => markDef.Key == mark).Type](typedBlock, blockChild, mark);
                                        });
                                        var startTags = tags.Select(x => x.Item1);
                                        var endTags = tags.Select(x => x.Item2).Reverse();

                                        blocks.AddRange(startTags);
                                        blocks.Add(blockChild.Text);
                                        blocks.AddRange(endTags);
                                    }
                                }

                                return serializers.BlockStyleSerializers[typedBlock.Style](blocks);
                            }
                        }
                    }
                },
                MarkSerializers = new Dictionary<string, Func<PortableTextBlock, PortableTextChild, string, (string startTag, string endTag)>>
                {
                    { "strong", (block, blockChild, mark) => ("<strong>", "</strong>") },
                    { "em", (block, blockChild, mark) => ("<em>", "</em>") },
                    { "code", (block, blockChild, mark) => ("<code>", "</code>") },
                    { "underline", (block, blockChild, mark) => (@"<span style=""text-decoration: underline;"">", "</span>") },
                    { "strike-through", (block, blockChild, mark) => ("<del>", "</del>") },

                    // Not happy with this. Do we really need the block in itself? Maybe implement a more dynamic approach?
                    {
                        "link", (block, blockChild, mark) =>
                        {
                            var link = block.MarkDefinitions.First(x => x.Key == mark);
                            return ($"<a href=\"{link.Href}\">", "</a>");
                        }
                    }
                },
                BlockStyleSerializers = new Dictionary<string, Func<IEnumerable<string>, string>>
                {
                    { "normal", blocks => $"<p>{string.Join(string.Empty, blocks)}</p>" },
                    { "h1", blocks => $"<h1>{string.Join(string.Empty, blocks)}</h1>" },
                    { "h2", blocks => $"<h2>{string.Join(string.Empty, blocks)}</h2>" },
                    { "h3", blocks => $"<h3>{string.Join(string.Empty, blocks)}</h3>" },
                    { "h4", blocks => $"<h4>{string.Join(string.Empty, blocks)}</h4>" },
                    { "h5", blocks => $"<h5>{string.Join(string.Empty, blocks)}</h5>" },
                    { "h6", blocks => $"<h6>{string.Join(string.Empty, blocks)}</h6>" },
                    { "blockquote", blocks => $"<blockquote>{string.Join(string.Empty, blocks)}</blockquote>" }
                }
            };
        }

        private static PortableTextSerializers MergeTypeSerializers(PortableTextSerializers defaultSerializers, PortableTextSerializers customSerializers)
        {
            if (customSerializers == null)
            {
                return defaultSerializers;
            }

            var serializers = new PortableTextSerializers();

            if (customSerializers.TypeSerializers == null || !customSerializers.TypeSerializers.Any())
            {
                serializers.TypeSerializers = defaultSerializers.TypeSerializers;
            }
            else
            {
                defaultSerializers.TypeSerializers.ToList().ForEach(x => serializers.TypeSerializers.Add(x.Key, x.Value));
                customSerializers.TypeSerializers.ToList().ForEach(x => serializers.TypeSerializers[x.Key] = x.Value);
            }

            if (customSerializers.MarkSerializers == null || !customSerializers.MarkSerializers.Any())
            {
                serializers.MarkSerializers = defaultSerializers.MarkSerializers;
            }
            else
            {
                defaultSerializers.MarkSerializers.ToList().ForEach(x => serializers.MarkSerializers.Add(x.Key, x.Value));
                customSerializers.MarkSerializers.ToList().ForEach(x => serializers.MarkSerializers[x.Key] = x.Value);
            }

            if (customSerializers.BlockStyleSerializers == null || !customSerializers.BlockStyleSerializers.Any())
            {
                serializers.BlockStyleSerializers = defaultSerializers.BlockStyleSerializers;
            }
            else
            {
                defaultSerializers.BlockStyleSerializers.ToList().ForEach(x => serializers.BlockStyleSerializers.Add(x.Key, x.Value));
                customSerializers.BlockStyleSerializers.ToList().ForEach(x => serializers.BlockStyleSerializers[x.Key] = x.Value);
            }

            return serializers;
        }

        public static string Render(string json, PortableTextSerializers customSerializers = null)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return null;
            }

            var serializers = MergeTypeSerializers(GetDefaultBlockSerializers(), customSerializers);
            
            using (JsonDocument document = JsonDocument.Parse(json))
            {
                var documentLength = document.RootElement.GetArrayLength();
                if (documentLength == 0)
                {
                    return null;
                }

                var accumulatedHtml = new List<string>(documentLength);
                foreach (JsonElement element in document.RootElement.EnumerateArray())
                {
                    JsonElement typeElement;
                    try
                    {
                        typeElement = element.GetProperty("_type");
                    }
                    catch (KeyNotFoundException)
                    {
                        continue;
                    }

                    var type = typeElement.GetString();

                    if (string.IsNullOrWhiteSpace(type))
                    {
                        continue;
                    }

                    var typeSerializer = serializers.TypeSerializers.TryGetValue(type, out var serializer);
                    if (serializer == null)
                    {
                        continue;
                    }

                    var utf8Value = Encoding.UTF8.GetBytes(element.ToString());
                    var readOnlySpan = new ReadOnlySpan<byte>(utf8Value);
                    var value = JsonSerializer.Deserialize(readOnlySpan, serializer.Type, jsonSerializerOptions);

                    accumulatedHtml.Add(serializer.Serialize(value, serializers));
                }

                if (!accumulatedHtml.Any())
                {
                    return null;
                }

                return string.Join("", accumulatedHtml);
            }
        }
    }

    public class PortableTextSerializers
    {
        public PortableTextSerializers()
        {
            TypeSerializers = new Dictionary<string, TypeSerializer>();
            MarkSerializers = new Dictionary<string, Func<PortableTextBlock, PortableTextChild, string, (string, string)>>();
            BlockStyleSerializers = new Dictionary<string, Func<IEnumerable<string>, string>>();
        }
        public Dictionary<string, TypeSerializer> TypeSerializers { get; set; }
        public Dictionary<string, Func<PortableTextBlock, PortableTextChild, string, (string, string)>> MarkSerializers { get; set; }
        public Dictionary<string, Func<IEnumerable<string>, string>> BlockStyleSerializers { get; set; }
    }

    public class PortableTextSerializersCopy
    {
        public PortableTextSerializersCopy()
        {
            TypeSerializers = new Dictionary<string, Func<object, PortableTextSerializers, string>>();
            MarkSerializers = new Dictionary<string, Func<PortableTextBlock, PortableTextChild, string, (string, string)>>();
            BlockStyleSerializers = new Dictionary<string, Func<IEnumerable<string>, string>>();
        }
        public Dictionary<string, Func<object, PortableTextSerializers, string>> TypeSerializers { get; set; }
        public Dictionary<string, Func<PortableTextBlock, PortableTextChild, string, (string, string)>> MarkSerializers { get; set; }
        public Dictionary<string, Func<IEnumerable<string>, string>> BlockStyleSerializers { get; set; }
    }

    public static class BlockContentToHtmlCopy
    {
        private static JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public static PortableTextSerializersCopy DefaultBlockSerializers = GetDefaultBlockSerializers();
        private static PortableTextSerializersCopy GetDefaultBlockSerializers()
        {
            return new PortableTextSerializersCopy
            {
                TypeSerializers = new Dictionary<string, Func<object, PortableTextSerializers, string>>
                {
                    {
                        "block", (block, serializers) =>
                            {
                                var typedBlock = block as PortableTextBlock;
                                if (typedBlock == null)
                                {
                                    return string.Empty;
                                }

                                if (string.IsNullOrWhiteSpace(typedBlock.Style) ||
                                    !typedBlock.Children.Any())
                                {
                                    return string.Empty;
                                }

                                var children = typedBlock.Children;

                                var blocks = new List<string>();
                                foreach (var blockChild in children)
                                {
                                    if (blockChild.Marks == null || !blockChild.Marks.Any())
                                    {
                                        blocks.Add(blockChild.Text.Replace("\n", "<br>"));
                                    }
                                    else
                                    {
                                        var tags = blockChild.Marks.Select(mark =>
                                        {
                                            var defaultSerializer = serializers.MarkSerializers.TryGetValue(mark, out var defaultMarkSerializerExists);
                                            if (defaultMarkSerializerExists != null)
                                            {
                                                return serializers.MarkSerializers[mark](typedBlock, blockChild, mark);
                                            }

                                            return serializers.MarkSerializers[typedBlock.MarkDefinitions.First(markDef => markDef.Key == mark).Type](typedBlock, blockChild, mark);
                                        });
                                        var startTags = tags.Select(x => x.Item1);
                                        var endTags = tags.Select(x => x.Item2).Reverse();

                                        blocks.AddRange(startTags);
                                        blocks.Add(blockChild.Text);
                                        blocks.AddRange(endTags);
                                    }
                                }

                                return serializers.BlockStyleSerializers[typedBlock.Style](blocks);
                            }
                    }
                },
                MarkSerializers = new Dictionary<string, Func<PortableTextBlock, PortableTextChild, string, (string startTag, string endTag)>>
                {
                    { "strong", (block, blockChild, mark) => ("<strong>", "</strong>") },
                    { "em", (block, blockChild, mark) => ("<em>", "</em>") },
                    { "code", (block, blockChild, mark) => ("<code>", "</code>") },
                    { "underline", (block, blockChild, mark) => (@"<span style=""text-decoration: underline;"">", "</span>") },
                    { "strike-through", (block, blockChild, mark) => ("<del>", "</del>") },

                    // Not happy with this. Do we really need the block in itself? Maybe implement a more dynamic approach?
                    {
                        "link", (block, blockChild, mark) =>
                        {
                            var link = block.MarkDefinitions.First(x => x.Key == mark);
                            return ($"<a href=\"{link.Href}\">", "</a>");
                        }
                    }
                },
                BlockStyleSerializers = new Dictionary<string, Func<IEnumerable<string>, string>>
                {
                    { "normal", blocks => $"<p>{string.Join(string.Empty, blocks)}</p>" },
                    { "h1", blocks => $"<h1>{string.Join(string.Empty, blocks)}</h1>" },
                    { "h2", blocks => $"<h2>{string.Join(string.Empty, blocks)}</h2>" },
                    { "h3", blocks => $"<h3>{string.Join(string.Empty, blocks)}</h3>" },
                    { "h4", blocks => $"<h4>{string.Join(string.Empty, blocks)}</h4>" },
                    { "h5", blocks => $"<h5>{string.Join(string.Empty, blocks)}</h5>" },
                    { "h6", blocks => $"<h6>{string.Join(string.Empty, blocks)}</h6>" },
                    { "blockquote", blocks => $"<blockquote>{string.Join(string.Empty, blocks)}</blockquote>" }
                }
            };
        }

        private static PortableTextSerializersCopy MergeTypeSerializers(PortableTextSerializersCopy defaultSerializers, PortableTextSerializersCopy customSerializers)
        {
            if (customSerializers == null)
            {
                return defaultSerializers;
            }

            var serializers = new PortableTextSerializersCopy();

            if (customSerializers.TypeSerializers == null || !customSerializers.TypeSerializers.Any())
            {
                serializers.TypeSerializers = defaultSerializers.TypeSerializers;
            }
            else
            {
                defaultSerializers.TypeSerializers.ToList().ForEach(x => serializers.TypeSerializers.Add(x.Key, x.Value));
                customSerializers.TypeSerializers.ToList().ForEach(x => serializers.TypeSerializers[x.Key] = x.Value);
            }

            if (customSerializers.MarkSerializers == null || !customSerializers.MarkSerializers.Any())
            {
                serializers.MarkSerializers = defaultSerializers.MarkSerializers;
            }
            else
            {
                defaultSerializers.MarkSerializers.ToList().ForEach(x => serializers.MarkSerializers.Add(x.Key, x.Value));
                customSerializers.MarkSerializers.ToList().ForEach(x => serializers.MarkSerializers[x.Key] = x.Value);
            }

            if (customSerializers.BlockStyleSerializers == null || !customSerializers.BlockStyleSerializers.Any())
            {
                serializers.BlockStyleSerializers = defaultSerializers.BlockStyleSerializers;
            }
            else
            {
                defaultSerializers.BlockStyleSerializers.ToList().ForEach(x => serializers.BlockStyleSerializers.Add(x.Key, x.Value));
                customSerializers.BlockStyleSerializers.ToList().ForEach(x => serializers.BlockStyleSerializers[x.Key] = x.Value);
            }

            return serializers;
        }

        public static string Render(string json, PortableTextSerializersCopy customSerializers = null)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return null;
            }

            var serializers = MergeTypeSerializers(GetDefaultBlockSerializers(), customSerializers);

            using (JsonDocument document = JsonDocument.Parse(json))
            {
                var documentLength = document.RootElement.GetArrayLength();
                if (documentLength == 0)
                {
                    return null;
                }

                var accumulatedHtml = new List<string>(documentLength);
                foreach (JsonElement element in document.RootElement.EnumerateArray())
                {
                    JsonElement typeElement;
                    try
                    {
                        typeElement = element.GetProperty("_type");
                    }
                    catch (KeyNotFoundException)
                    {
                        continue;
                    }

                    var type = typeElement.GetString();

                    if (string.IsNullOrWhiteSpace(type))
                    {
                        continue;
                    }

                    var utf8Value = Encoding.UTF8.GetBytes(element.ToString());
                    var readOnlySpan = new ReadOnlySpan<byte>(utf8Value);
                    var value = JsonSerializer.Deserialize(readOnlySpan, serializer.Type, jsonSerializerOptions);

                    accumulatedHtml.Add(serializer(value, serializers));
                }

                if (!accumulatedHtml.Any())
                {
                    return null;
                }

                return string.Join("", accumulatedHtml);
            }
        }
    }
}
