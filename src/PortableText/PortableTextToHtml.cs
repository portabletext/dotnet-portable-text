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
                        return RenderBlock(doc.RootElement, typedBlock.Style, typedBlock.ListItem, typedBlock.Level, serializers);
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

    private static readonly TypeSerializer DefaultBlockTypeSerializer = DefaultBlockSerializers.TypeSerializers["block"];

    /// <summary>
    /// Serializes a given JSON string representing Portable Text to HTML.
    /// See the README for more information.
    /// </summary>
    /// <param name="json">JSON containing your Portable Text.</param>
    /// <param name="customSerializers">Your extension point for customizing the rendering of the Portable Text nodes.</param>
    /// <returns>A HTML representation of the Portable Text.</returns>
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
                accumulatedHtml.Add(SerializeElement(currentElement, serializers, serializer));
            }
        }

        if (!accumulatedHtml.Any())
        {
            return null;
        }

        return string.Join("", accumulatedHtml);
    }
    
    /// <summary>
    /// The built-in block serializer renders directly from the parsed element, instead of receiving the element
    /// as raw JSON and parsing it again. Other serializers receive the deserialized value and the raw JSON as before.
    /// </summary>
    private static string SerializeElement(JsonElement element, PortableTextSerializers serializers, TypeSerializer serializer)
    {
        if (ReferenceEquals(serializer, DefaultBlockTypeSerializer))
        {
            // Read what the block needs directly, instead of deserializing it into a PortableTextBlock. A block that
            // is not a list item, e.g. because its level is not a number, is rendered without its listItem.
            var listItem = PtHelpers.IsPortableTextElementList(element) ? ReadString(element, "listItem") : null;
            return RenderBlock(element, ReadString(element, "style"), listItem, ReadLevel(element), serializers);
        }

        var value = element.Deserialize(serializer.Type, JsonSerializerOptions);
        return serializer.Serialize(value, element.GetRawText(), serializers, false);
    }

    /// <summary>
    /// Reads a string property: null when the property is missing or not a string.
    /// </summary>
    private static string ReadString(JsonElement element, string propertyName)
    {
        return element.TryGetProperty(propertyName, out var property) && property.ValueKind == JsonValueKind.String
            ? property.GetString()
            : null;
    }

    /// <summary>
    /// Reads the level of a block: 0 when the level is missing or not a number.
    /// </summary>
    private static int ReadLevel(JsonElement element)
    {
        return PtHelpers.TryGetLevel(element, out var level) ? level ?? default : default;
    }

    private static string RenderBlock(JsonElement blockElement, string blockStyle, string listItem, int level, PortableTextSerializers serializers)
    {
        var childrenRaw = blockElement.GetProperty("children");
        var childrenLength = childrenRaw.GetArrayLength();

        if (childrenLength == 0)
        {
            return string.Empty;
        }

        var accumulatedHtml = new List<string>();
        foreach (var child in childrenRaw.EnumerateArray())
        {
            var typeElement = child.GetProperty("_type");
            if (!typeElement.ValueEquals("span"))
            {
                var childType = typeElement.GetString();
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
                // Read the text and marks directly, instead of deserializing the span into a PortableTextBlockChildSpan
                var text = (child.TryGetProperty("text", out var textElement) ? textElement.GetString() : null).PortableTextSerialize();
                var marks = ReadMarks(child);
                if (marks == null || marks.Length == 0)
                {
                    accumulatedHtml.Add(text);
                }
                else
                {
                    var tags = marks.Select(mark =>
                    {
                        // A mark definition without _type is not an annotation we can render, so the text is rendered
                        // without it. The specification does not say what to do with it, and rendering should not fail.
                        if (TryFindMarkDefinition(blockElement, mark, out var markType, out var markDefElement) &&
                            markType != null &&
                            PtHelpers.SerializerForMarkAnnotationExists(markType, serializers))
                        {
                            var annotatedMarkSerializer =
                                serializers.MarkSerializers.Annotations[markType];
                            var rawMarkDef = markDefElement.GetRawText();
                            var customMark = markDefElement.Deserialize(annotatedMarkSerializer.Type, JsonSerializerOptions);
                            return annotatedMarkSerializer.Serialize(customMark, rawMarkDef);
                        }

                        if (PtHelpers.SerializerForMarkDecoratorExists(mark, serializers))
                        {
                            return serializers.MarkSerializers.Decorators[mark]();
                        }

                        return (null, null);
                    }).ToArray();

                    var startTags = tags.Where(x => x.Item1 != null).Select(x => x.Item1);
                    var endTags = tags.Where(x => x.Item2 != null).Select(x => x.Item2).Reverse();

                    accumulatedHtml.AddRange(startTags);
                    accumulatedHtml.Add(text);
                    accumulatedHtml.AddRange(endTags);
                }
            }
        }

        // NOTE: The spec does not say how to render a style without a serializer. We render it as the normal style.
        var style = string.IsNullOrWhiteSpace(blockStyle) || !serializers.BlockStyleSerializers.ContainsKey(blockStyle)
            ? "normal"
            : blockStyle;
        var serialized = serializers.BlockStyleSerializers[style](accumulatedHtml);
        
        // NOTE: Not the most elegant approach, but ListItemSerializers are kind of inconsistent when
        // NOTE:    using block style serializers - if it's not a normal style, they should use the style,
        // NOTE:    but if it's a normal style, the p-tag is omitted. That is kind of inconsistent, but maybe
        // NOTE:    ideal from a user perspective. You don't necessarily want the p-tags inside your li-s
        if (style == "normal" && string.IsNullOrWhiteSpace(listItem) && level == default)
        {
            return $"<p>{serialized}</p>";
        }

        return serialized;
    }

    /// <summary>
    /// Finds the mark definition with the given key in the block's markDefs.
    /// </summary>
    /// <remarks>
    /// We assume that <c>_key</c> is unique, since it identifies the definition. The Portable Text specification does
    /// not say what to do with duplicate keys, so we use the first definition with the key, for both its type and its
    /// data, and rendering does not fail.
    /// </remarks>
    private static bool TryFindMarkDefinition(JsonElement blockElement, string key, out string type, out JsonElement definition)
    {
        type = null;
        definition = default;
        if (!blockElement.TryGetProperty("markDefs", out var markDefs) || markDefs.ValueKind == JsonValueKind.Null)
        {
            return false;
        }

        foreach (var markDef in markDefs.EnumerateArray())
        {
            if (markDef.TryGetProperty("_key", out var keyElement) && keyElement.ValueKind == JsonValueKind.String && keyElement.ValueEquals(key))
            {
                type = ReadString(markDef, "_type");
                definition = markDef;
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Reads the marks of a span the way deserializing it into <see cref="PortableTextBlockChildSpan"/> would:
    /// null when the property is missing or null, otherwise one string per mark.
    /// </summary>
    private static string[] ReadMarks(JsonElement span)
    {
        if (!span.TryGetProperty("marks", out var marksElement) || marksElement.ValueKind == JsonValueKind.Null)
        {
            return null;
        }

        var marks = new string[marksElement.GetArrayLength()];
        var i = 0;
        foreach (var mark in marksElement.EnumerateArray())
        {
            marks[i++] = mark.GetString();
        }

        return marks;
    }

    private static string SerializeList(JsonDocument document, JsonElement currentElement, ref int currentIndex, PortableTextSerializers serializers, TypeSerializer serializer)
    {
        var documentLength = document.RootElement.GetArrayLength();
        var listVariant = currentElement.GetProperty("listItem").GetString();
        var level = PtHelpers.GetListLevel(currentElement);

        var (startTag, endTag) = PtHelpers.GetListItemSerializer(listVariant, serializers)();
        var listItems = new List<string>
        {
            SerializeElement(currentElement, serializers, serializer)
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
                listItems.Add(SerializeElement(siblingElement, serializers, serializer));
            }
        }

        currentIndex = siblingIndex - 1;
        return PtHelpers.GetListSerializer(listVariant, serializers)(listItems.Select(x => $"{startTag + x + endTag}"));
    }
}
