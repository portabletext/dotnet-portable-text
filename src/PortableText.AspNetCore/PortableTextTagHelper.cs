using Microsoft.AspNetCore.Razor.TagHelpers;
using PortableText;

namespace PortableText.AspNetCore;

public class PortableTextTagHelper : TagHelper
{
    [HtmlAttributeName("value")]
    public string Value { get; set; } = string.Empty;

    [HtmlAttributeName("serializers")]
    public PortableTextSerializers Serializers { get; set; } = new ();
    
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        if (Value == string.Empty || string.IsNullOrWhiteSpace(Value))
        {
            output.SuppressOutput();
        }
        else
        {
            var result = PortableTextToHtml.Render(Value, Serializers);

            output.TagName = null;
            output.Content.SetHtmlContent(result);
        }
        
        base.Process(context, output);
    }
}