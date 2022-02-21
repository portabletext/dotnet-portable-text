using System.Text.Json.Serialization;

namespace PortableText;

/// <summary>
/// A class representing the least amount of data a mark definition can have.
/// </summary>
public class PortableTextMarkDefinition
{
    [JsonPropertyName("_key")]
    public string Key { get; set; }

    [JsonPropertyName("_type")]
    public string Type { get; set; }
}