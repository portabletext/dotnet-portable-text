namespace PortableText;

/// <summary>
/// A class representing a annotation mark that contains information for rendering a link.
/// </summary>
public sealed class LinkPortableTextMarkAnnotation : PortableTextMarkDefinition
{
    public string Href { get; set; }   
}
