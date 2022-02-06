using System.Text.Json.Serialization;

namespace Sanity;

public class PortableTextBlock
{
    [JsonPropertyName("_key")]
    public string Key { get; set; }
    [JsonPropertyName("_type")]
    public string Type { get; set; }
    public PortableTextChild[] Children { get; set; }
    [JsonPropertyName("markDefs")]
    public MarkDefinition[] MarkDefinitions { get; set; }
    public string Style { get; set; }
}