using System.Text.Json.Serialization;

namespace Sanity;

public class MarkDefinition
{
    [JsonPropertyName("_key")]
    public string Key { get; set; }
    [JsonPropertyName("_type")]
    public string Type { get; set; }
    public string Href { get; set; }
}