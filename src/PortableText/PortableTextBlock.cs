using System.Text.Json.Serialization;

namespace PortableText;

public class PortableTextBlock
{
    [JsonPropertyName("_key")]
    public string Key { get; set; }
    
    [JsonPropertyName("_type")]
    public string Type { get; set; }
    
    [JsonPropertyName("markDefs")]
    public PortableTextMarkDefinition[] MarkDefinitions { get; set; }

    public string Style { get; set; }
    
    [JsonPropertyName("listItem")]
    public string ListItem { get; set; }
    
    [JsonPropertyName("level")]
    public int Level { get; set; }
}