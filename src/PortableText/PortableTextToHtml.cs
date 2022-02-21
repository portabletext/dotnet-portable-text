using System.Linq;
using System.Text.Json;
using System.Collections.Generic;
using System;
using PortableText.Internal;

namespace PortableText;

public static class PortableTextToHtml
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private static readonly PortableTextSerializers DefaultBlockSerializers = new()
    {
        TypeSerializers = new Dictionary<string, TypeSerializer>
        {
            {
                "block", new TypeSerializer
                {
                    Type = typeof(PortableTextBlock),
                    Serialize = (value, rawValue, serializers, _) =>
                    {
                        var typedBlock = value as PortableTextBlock;
                        if (typedBlock == null)
                        {
                            return string.Empty;
                        }

                        using var doc = JsonDocument.Parse(rawValue);
                        var childrenRaw = doc.RootElement.GetProperty("children");
                        var childrenLength = childrenRaw.GetArrayLength();

                        if (childrenLength == 0)
                        {
                            return string.Empty;
                        }

                        var accumulatedHtml = new List<string>();
                        foreach (var child in childrenRaw.EnumerateArray())
                        {
                            var childType = child.GetProperty("_type").GetString();
                            if (childType != "span")
                            {
                                if (!PtHelpers.SerializerForTypeExists(childType, serializers))
                                {
                                    continue;
                                }
                                
                                var SpanTypeSerializer = serializers.TypeSerializers[childType];
                                var typedValue = child.Deserialize(SpanTypeSerializer.Type, JsonSerializerOptions);
                                var content = SpanTypeSerializer.Serialize(typedValue, child.ToString(), serializers, true);
                                accumulatedHtml.Add(content);
                            }
                            else
                            {
                                var blockChild = child.Deserialize<PortableTextBlockChildSpan>(JsonSerializerOptions);
                                var text = blockChild.Text.PortableTextSerialize();
                                if (blockChild.Marks == null || !blockChild.Marks.Any())
                                {
                                    accumulatedHtml.Add(text);
                                }
                                else
                                {
                                    var tags = blockChild.Marks.Select(mark =>
                                    {
                                        var markInMarkDef =
                                            typedBlock.MarkDefinitions.FirstOrDefault(x => x.Key == mark);

                                        if (markInMarkDef != null && PtHelpers.SerializerForMarkAnnotationExists(markInMarkDef.Type, serializers))
                                        {
                                            var annotatedMarkSerializer =
                                                serializers.MarkSerializers.Annotations[markInMarkDef.Type];
                                            string rawMarkDef = null;
                                            var markDefs = doc.RootElement.GetProperty("markDefs");
                                            foreach (var markDef in markDefs.EnumerateArray())
                                            {
                                                if (markDef.GetProperty("_key").GetString() == mark)
                                                {
                                                    rawMarkDef = markDef.ToString();
                                                }
                                            }

                                            var customMark = JsonSerializer.Deserialize(rawMarkDef, annotatedMarkSerializer.Type, JsonSerializerOptions);
                                            return annotatedMarkSerializer.Serialize(customMark, rawMarkDef);
                                        }

                                        if (PtHelpers.SerializerForMarkDecoratorExists(mark, serializers))
                                        {
                                            return serializers.MarkSerializers.Decorators[mark]();
                                        }

                                        return (null, null);
                                    });

                                    var startTags = tags.Where(x => x.Item1 != null).Select(x => x.Item1);
                                    var endTags = tags.Where(x => x.Item2 != null).Select(x => x.Item2).Reverse();

                                    accumulatedHtml.AddRange(startTags);
                                    accumulatedHtml.Add(text);
                                    accumulatedHtml.AddRange(endTags);
                                }
                            }
                        }

                        var style = string.IsNullOrWhiteSpace(typedBlock.Style) ? "normal" : typedBlock.Style;
                        var serialized = serializers.BlockStyleSerializers[style](accumulatedHtml);
                        
                        // NOTE: Not the most elegant approach, but ListItemSerializers are kind of inconsistent when
                        // NOTE:    using block style serializers - if it's not a normal style, they should use the style,
                        // NOTE:    but if it's a normal style, the p-tag is omitted. That is kind of inconsistent, but maybe
                        // NOTE:    ideal from a user perspective. You don't necessarily want the p-tags inside your li-s
                        if (style == "normal" && string.IsNullOrWhiteSpace(typedBlock.ListItem) && typedBlock.Level == default)
                        {
                            return $"<p>{serialized}</p>";
                        }

                        return serialized;
                    }
                }
            }
        },
        MarkSerializers = new MarkSerializer
        {
            Annotations = new Dictionary<string, AnnotatedMarkSerializer>
            {
                {
                    "link",
                    new AnnotatedMarkSerializer
                    {
                        Type = typeof(LinkPortableTextMarkAnnotation),
                        Serialize = (value, rawValue) =>
                        {
                            var typed = value as LinkPortableTextMarkAnnotation;

                            return ($@"<a href=""{typed.Href}"">", "</a>");
                        }
                    }
                }
            },
            Decorators = new Dictionary<string, Func<(string startTag, string endTag)>>
            {
                { "strong", () => ("<strong>", "</strong>") },
                { "em", () => ("<em>", "</em>") },
                { "code", () => ("<code>", "</code>") },
                { "underline", () => (@"<span style=""text-decoration:underline"">", "</span>") },
                { "strike-through", () => ("<del>", "</del>") },
            }
        },
        BlockStyleSerializers = new Dictionary<string, Func<IEnumerable<string>, string>>
        {
            { "normal", blocks => $"{string.Join(string.Empty, blocks)}" },
            { "h1", blocks => $"<h1>{string.Join(string.Empty, blocks)}</h1>" },
            { "h2", blocks => $"<h2>{string.Join(string.Empty, blocks)}</h2>" },
            { "h3", blocks => $"<h3>{string.Join(string.Empty, blocks)}</h3>" },
            { "h4", blocks => $"<h4>{string.Join(string.Empty, blocks)}</h4>" },
            { "h5", blocks => $"<h5>{string.Join(string.Empty, blocks)}</h5>" },
            { "h6", blocks => $"<h6>{string.Join(string.Empty, blocks)}</h6>" },
            { "blockquote", blocks => $"<blockquote>{string.Join(string.Empty, blocks)}</blockquote>" }
        },
        ListSerializers = new Dictionary<string, Func<IEnumerable<string>, string>>
        {
            { "bullet", listItems => $"<ul>{string.Join(string.Empty, listItems)}</ul>" },
            { "number", listItems => $"<ol>{string.Join(string.Empty, listItems)}</ol>" }
        },
        ListItemSerializers = new Dictionary<string, Func<(string, string)>>
        {
            { "bullet", () => ("<li>", "</li>") },
            { "number", () => ("<li>", "</li>") }
        }
    };

    public static string Render(string json, PortableTextSerializers customSerializers = null)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return null;
        }

        var serializers = PtHelpers.MergeSerializers(DefaultBlockSerializers, customSerializers);

        using var document = JsonDocument.Parse(json);
        var documentLength = document.RootElement.GetArrayLength();
        if (documentLength == 0)
        {
            return null;
        }

        var accumulatedHtml = new List<string>(documentLength);
        for (var i = 0; i < documentLength; i++)
        {
            var currentElement = document.RootElement[i];
            if (!PtHelpers.IsPortableTextElementValid(currentElement))
            {
                continue;
            }

            var elementType = currentElement.GetProperty("_type").GetString();
            if (!PtHelpers.SerializerForTypeExists(elementType, serializers))
            {
                // Warn?
                continue;
            }

            var serializer = serializers.TypeSerializers[elementType];
            if (PtHelpers.IsPortableTextElementList(currentElement))
            {
                accumulatedHtml.Add(SerializeList(document, currentElement, ref i, serializers, serializer));
            }
            else
            {
                var currentElementJson = currentElement.ToString();
                var value = JsonSerializer.Deserialize(currentElement.ToString(), serializer.Type, JsonSerializerOptions);
                accumulatedHtml.Add(serializer.Serialize(value, currentElementJson, serializers, false));
            }
        }

        if (!accumulatedHtml.Any())
        {
            return null;
        }

        return string.Join("", accumulatedHtml);
    }
    
    private static string SerializeList(JsonDocument document, JsonElement currentElement, ref int currentIndex, PortableTextSerializers serializers, TypeSerializer serializer)
    {
        var documentLength = document.RootElement.GetArrayLength();
        var listVariant = currentElement.GetProperty("listItem").GetString();
        var level = PtHelpers.GetListLevel(currentElement);

        var listItemValue = JsonSerializer.Deserialize(currentElement.ToString(), serializer.Type, JsonSerializerOptions);
        var (startTag, endTag) = PtHelpers.GetListItemSerializer(listVariant, serializers)();
        var listItems = new List<string>
        {
            $"{serializer.Serialize(listItemValue, currentElement.ToString(), serializers, false)}"
        };
        
        var siblingIndex = currentIndex + 1;
        while (true)
        {
            if (siblingIndex == documentLength)
            {
                break;
            }
            
            var siblingElement = document.RootElement[siblingIndex];
            if (!PtHelpers.IsPortableTextElementList(siblingElement))
            {
                break;
            }
            
            var siblingListItem = siblingElement.GetProperty("listItem").GetString();
            var siblingLevel = PtHelpers.GetListLevel(siblingElement);
            
            // NOTE: Since we are checking the levels first, the case where a list has a deeper level and a different variant
            // NOTE:     will work correctly when "recovering" from the deeper list. Since we check if the sibling level is less
            // NOTE:     than the current level, it will short-circuit that. Therefore we won't serialize another new list
            // NOTE:     with the level we have already been to, as that most likely has the same variant. If the variant is different
            // NOTE:     however, that will probably crash.
            if (siblingLevel > level)
            {
                var serialized = SerializeList(document, siblingElement, ref siblingIndex, serializers, serializer);
                listItems[^1] = listItems.Last() + serialized;
                siblingIndex++;
                currentIndex++;
            }
            else if (siblingLevel < level || siblingListItem != listVariant)
            {
                break;
            }
            else
            {
                currentIndex++;
                siblingIndex++;
                var siblingListItemValue = JsonSerializer.Deserialize(siblingElement.ToString(), serializer.Type, JsonSerializerOptions);
                listItems.Add($"{serializer.Serialize(siblingListItemValue, siblingElement.ToString(), serializers, false)}");
            }
        }

        currentIndex = siblingIndex - 1;
        return PtHelpers.GetListSerializer(listVariant, serializers)(listItems.Select(x => $"{startTag + x + endTag}"));
    }
}
