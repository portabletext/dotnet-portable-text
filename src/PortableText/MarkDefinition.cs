using System.Text.Json.Serialization;

namespace PortableText;

// This class is kind of hardcoded to a link markDef
public class MarkDefinition
{
    [JsonPropertyName("_key")]
    public string Key { get; set; }
    [JsonPropertyName("_type")]
    public string Type { get; set; }
    public string Href { get; set; }
}