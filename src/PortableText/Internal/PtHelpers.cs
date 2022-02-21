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
        return listElement.TryGetProperty("level", out _)
            ? listElement.GetProperty("level").GetInt32()
            : 1;
    }

    internal static bool IsPortableTextElementList(JsonElement currentElement)
    {
        try
        {
            currentElement.GetProperty("listItem");
            return true;
        }
        catch (KeyNotFoundException)
        {
            return false;
        }
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