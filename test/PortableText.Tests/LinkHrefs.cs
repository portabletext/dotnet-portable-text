using Shouldly;
using Xunit;

namespace PortableText;

public partial class Tests
{
    [Theory]
    [InlineData("https://sanity.io/")]
    [InlineData("HTTP://sanity.io/")]
    [InlineData("mailto:hello@sanity.io")]
    [InlineData("tel:+4712345678")]
    [InlineData("#section")]
    [InlineData("/docs")]
    [InlineData("//sanity.io/docs")]
    [InlineData("docs/getting-started")]
    [InlineData("  https://sanity.io/  ")]
    [InlineData("site/search?query=author:espen")]
    [InlineData("site/search#my:encoded:data")]
    public void LinkMark_RendersSafeHrefAsLink(string href)
    {
        var result = PortableTextToHtml.Render(LinkJson(href));

        result.ShouldBe($@"<p><a href=""{href}"">Test</a></p>");
    }

    [Theory]
    [InlineData("javascript:alert(1)")]
    [InlineData("JavaScript:alert(1)")]
    [InlineData("  javascript:alert(1)")]
    [InlineData("data:text/html,<script>alert(1)</script>")]
    [InlineData("vbscript:msgbox(1)")]
    [InlineData("ftp://sanity.io/")]
    [InlineData("java\tscript:alert(1)")]
    [InlineData("java\nscript:alert(1)")]
    [InlineData("\u0001javascript:alert(1)")]
    public void LinkMark_RendersUnsafeHrefAsPlainText(string href)
    {
        var result = PortableTextToHtml.Render(LinkJson(href));

        result.ShouldBe("<p>Test</p>");
    }

    [Fact]
    public void LinkMark_EscapesHrefAttribute()
    {
        var result = PortableTextToHtml.Render(LinkJson(@"https://sanity.io/?a=1&b=2"" onclick=""alert(1)"));

        result.ShouldBe(@"<p><a href=""https://sanity.io/?a=1&amp;b=2&quot; onclick=&quot;alert(1)"">Test</a></p>");
    }

    private static string LinkJson(string href)
    {
        var escapedHref = System.Text.Json.JsonSerializer.Serialize(href);
        return $@"
[
    {{
        ""_type"": ""block"",
        ""children"": [
            {{ ""_type"": ""span"", ""text"": ""Test"", ""marks"": [""linkKey""] }}
        ],
        ""markDefs"": [
            {{ ""_type"": ""link"", ""_key"": ""linkKey"", ""href"": {escapedHref} }}
        ]
    }}
]
";
    }
}
