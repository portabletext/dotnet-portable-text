using Microsoft.AspNetCore.Razor.TagHelpers;
using PortableText;

namespace Sanity.PortableText.AspNetCore;

public class PortableTextTagHelper : TagHelper
{
    [HtmlAttributeName("value")]
    public string Value { get; set; }
    
    [HtmlAttributeName("serializers")]
    public PortableTextSerializers Serializers { get; set; }
    
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