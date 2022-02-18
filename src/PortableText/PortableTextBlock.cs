using System.Text.Json.Serialization;

namespace PortableText;

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
    [JsonPropertyName("listItem")]
    public string ListItem { get; set; }
    [JsonPropertyName("level")]
    public int Level { get; set; }
}