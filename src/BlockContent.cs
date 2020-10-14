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
                for (int i = 0; i < documentLength; i++)
                {
                    var currentElement = document.RootElement[i];
                    JsonElement typeElement;
                    try
                    {
                        typeElement = currentElement.GetProperty("_type");
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

                    try
                    {
                        // TODO: Refactor into own code branch for handling lists.
                        //       Make it readable, not performant to begin with (multiple reads of the same value is fine).
                        JsonElement listItemElement = currentElement.GetProperty("listItem");
                        var listItem = listItemElement.GetString();

                        JsonElement levelElement = currentElement.GetProperty("level");
                        var level = levelElement.GetInt32();

                        var listStuff = new List<string>();
                        int siblingCounter = i + 1;
                        // search for siblings with same listItem and level and loop over them
                        for (int j = siblingCounter; j < documentLength - i + 1; j++)
                        {
                            // TODO: Find function for searching a list until condition?
                            var siblingElement = document.RootElement[j];
                            try
                            {
                                var siblingListItemElement = siblingElement.GetProperty("listItem");
                                var siblingListItem = siblingListItemElement.GetString();

                                if (siblingListItem != listItem)
                                {
                                    break;
                                }

                                var siblingLevelElement = siblingElement.GetProperty("level");
                                var siblingLevel = siblingLevelElement.GetInt32();

                                if (siblingLevel != level)
                                {
                                    // TODO: Recursively add nested lists...
                                    break;
                                }

                                siblingCounter++;
                                var siblingListItemValue = JsonSerializer.Deserialize(siblingElement.ToString(), serializer.Type, jsonSerializerOptions);
                                listStuff.Add($"<li>{serializer.Serialize(siblingListItemValue, serializers)}</li>");
                            }
                            catch (KeyNotFoundException)
                            {
                                break;
                            }
                        }

                        var listItemValue = JsonSerializer.Deserialize(currentElement.ToString(), serializer.Type, jsonSerializerOptions);

                        switch (listItem)
                        {
                            // TODO: Maybe it would be wise to add custom serializers for lists as well?
                            case "number":
                                {
                                    accumulatedHtml.Add($"<ol><li>{serializer.Serialize(listItemValue, serializers)}</li>{string.Join(string.Empty, listStuff)}</ol>");
                                    i = siblingCounter - 1;
                                    break;
                                }
                            case "bullet":
                                {
                                    accumulatedHtml.Add($"<ul><li>{serializer.Serialize(listItemValue, serializers)}</li>{string.Join(string.Empty, listStuff)}</ul>");
                                    i = siblingCounter - 1;
                                    break;
                                }
                            default:
                                break;
                        }
                    }
                    catch (KeyNotFoundException)
                    {
                        //var utf8Value = Encoding.UTF8.GetBytes(currentElement.ToString());
                        //var readOnlySpan = new ReadOnlySpan<byte>(utf8Value);
                        var value = JsonSerializer.Deserialize(currentElement.ToString(), serializer.Type, jsonSerializerOptions);

                        accumulatedHtml.Add(serializer.Serialize(value, serializers));
                    }
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
}
