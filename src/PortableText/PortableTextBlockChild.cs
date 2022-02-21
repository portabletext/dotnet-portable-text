using System.Text.Json.Serialization;

namespace PortableText;

/// <summary>
/// A class representing the least amount of data a block child can have.
/// </summary>
public class PortableTextBlockChild
{
    [JsonPropertyName("_key")]
    public string Key { get; set; }

    [JsonPropertyName("_type")]
    public string Type { get; set; }
}

/// <summary>
/// A class representing the a block child with _type = span.
/// </summary>
public sealed class PortableTextBlockChildSpan : PortableTextBlockChild
{
    public string[] Marks { get; set; }
    public string Text { get; set; }
}