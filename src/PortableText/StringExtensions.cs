using System.Web;

namespace PortableText;

public static class StringExtensions
{
    /// <summary>
    /// HTML-encodes (decimal) a string where also "\n" is replaced with &lt;br/&gt;.
    /// </summary>
    /// <param name="value">The string value to change. This is usually raw text from a Portable Text child with _type = span.</param>
    /// <returns>A HTML-encoded (decimal) string where also "\n" are replaced with &lt;br/&gt;</returns>
    public static string PortableTextSerialize(this string value)
    {
        return HttpUtility.HtmlEncode(value).Replace("\n", "<br/>");
    }
}