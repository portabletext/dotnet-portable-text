using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace PortableText.Internal;

internal static class PtHelpers
{
    internal static bool SerializerForTypeExists(string type, PortableTextSerializers serializers)
    {
        return serializers.TypeSerializers.TryGetValue(type, out _);
    }
    
    internal static bool SerializerForMarkAnnotationExists(string mark, PortableTextSerializers serializers)
    {
        return serializers.MarkSerializers.Annotations.TryGetValue(mark, out _);
    }
    
    internal static bool SerializerForMarkDecoratorExists(string mark, PortableTextSerializers serializers)
    {
        return serializers.MarkSerializers.Decorators.TryGetValue(mark, out _);
    }
    
    internal static Func<IEnumerable<string>, string> GetListSerializer(string listVariant, PortableTextSerializers serializers)
    {
        if (serializers.ListSerializers.TryGetValue(listVariant, out _))
        {
            return serializers.ListSerializers[listVariant];
        }

        return serializers.ListSerializers["bullet"];
    }
    
    internal static Func<(string, string)> GetListItemSerializer(string listVariant, PortableTextSerializers serializers)
    {
        if (serializers.ListItemSerializers.TryGetValue(listVariant, out _))
        {
            return serializers.ListItemSerializers[listVariant];
        }

        return serializers.ListItemSerializers["bullet"];
    }

    internal static int GetListLevel(JsonElement listElement)
    {
        return TryGetLevel(listElement, out var level) ? level ?? 1 : 1;
    }

    // NOTE: The spec does not give a type for listItem and level. We expect a string and a number: a block with
    // NOTE:    any other listItem or level is not a list item. A null value counts as a missing property.
    internal static bool IsPortableTextElementList(JsonElement currentElement)
    {
        return currentElement.TryGetProperty("listItem", out var listItem)
               && listItem.ValueKind == JsonValueKind.String
               && TryGetLevel(currentElement, out _);
    }

    /// <summary>
    /// Reads the level of a block, rounded down. The level is null when it is missing or null, and the method
    /// returns false when it is not a number that fits in an int.
    /// </summary>
    internal static bool TryGetLevel(JsonElement element, out int? level)
    {
        level = null;
        if (!element.TryGetProperty("level", out var levelElement) || levelElement.ValueKind == JsonValueKind.Null)
        {
            return true;
        }

        if (levelElement.ValueKind != JsonValueKind.Number)
        {
            return false;
        }

        if (levelElement.TryGetInt32(out var wholeLevel))
        {
            level = wholeLevel;
            return true;
        }

        var roundedLevel = Math.Floor(levelElement.GetDouble());
        if (roundedLevel < int.MinValue || roundedLevel > int.MaxValue)
        {
            return false;
        }

        level = (int)roundedLevel;
        return true;
    }
    
    internal static bool IsPortableTextElementValid(JsonElement element)
    {
        if (!element.TryGetProperty("_type", out JsonElement typeElement) || string.IsNullOrWhiteSpace(typeElement.GetString()))
        {
            return false;
        }

        return true;
    }

    internal static PortableTextSerializers MergeSerializers(PortableTextSerializers defaultSerializers, PortableTextSerializers customSerializers)
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

        if (customSerializers.MarkSerializers == null)
        {
            serializers.MarkSerializers = defaultSerializers.MarkSerializers;
        }

        if (customSerializers.MarkSerializers?.Annotations == null ||
            !customSerializers.MarkSerializers.Annotations.Any())
        {
            serializers.MarkSerializers.Annotations = defaultSerializers.MarkSerializers.Annotations;
        }
        else
        {
            defaultSerializers.MarkSerializers.Annotations.ToList().ForEach(x => serializers.MarkSerializers.Annotations.Add(x.Key, x.Value));
            customSerializers.MarkSerializers.Annotations.ToList().ForEach(x => serializers.MarkSerializers.Annotations[x.Key] = x.Value);
        }

        if (customSerializers.MarkSerializers?.Decorators == null || !customSerializers.MarkSerializers.Decorators.Any())
        {
            serializers.MarkSerializers.Decorators = defaultSerializers.MarkSerializers.Decorators;
        }
        else
        {
            defaultSerializers.MarkSerializers.Decorators.ToList().ForEach(x => serializers.MarkSerializers.Decorators.Add(x.Key, x.Value));
            customSerializers.MarkSerializers.Decorators.ToList().ForEach(x => serializers.MarkSerializers.Decorators[x.Key] = x.Value);
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
        
        if (customSerializers.ListSerializers == null || !customSerializers.ListSerializers.Any())
        {
            serializers.ListSerializers = defaultSerializers.ListSerializers;
        }
        else
        {
            defaultSerializers.ListSerializers.ToList().ForEach(x => serializers.ListSerializers.Add(x.Key, x.Value));
            customSerializers.ListSerializers.ToList().ForEach(x => serializers.ListSerializers[x.Key] = x.Value);
        }
        
        if (customSerializers.ListItemSerializers == null || !customSerializers.ListItemSerializers.Any())
        {
            serializers.ListItemSerializers = defaultSerializers.ListItemSerializers;
        }
        else
        {
            defaultSerializers.ListItemSerializers.ToList().ForEach(x => serializers.ListItemSerializers.Add(x.Key, x.Value));
            customSerializers.ListItemSerializers.ToList().ForEach(x => serializers.ListItemSerializers[x.Key] = x.Value);
        }

        return serializers;
    }
}