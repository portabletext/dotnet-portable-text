using System.Text.Json.Serialization;

namespace PortableText;

public class PortableTextChild
{
    [JsonPropertyName("_key")]
    public string Key { get; set; }
    [JsonPropertyName("_type")]
    public string Type { get; set; }
    public string[] Marks { get; set; }
    public string Text { get; set; }
}