using System.Text.Json.Serialization;

namespace PortableText;

public class PortableTextMarkDefinition
{
    [JsonPropertyName("_key")]
    public string Key { get; set; }

    [JsonPropertyName("_type")]
    public string Type { get; set; }
}