using System.Web;

namespace PortableText;

public static class StringExtensions
{
    public static string PortableTextSerialize(this string value)
    {
        return HttpUtility.HtmlEncode(value).Replace("\n", "<br/>");
    }
}