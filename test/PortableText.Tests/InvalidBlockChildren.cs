using Shouldly;
using Xunit;

namespace PortableText;

public partial class Tests
{
    [Theory]
    [InlineData("")]
    [InlineData(@"""children"": null,")]
    [InlineData(@"""children"": {},")]
    [InlineData(@"""children"": ""Hidden"",")]
    public void GivenBlockWithoutChildrenArray_IgnoresBlock(string children)
    {
        var result = PortableTextToHtml.Render(@"
[
    {
        ""_type"": ""block"",
        " + children + @"
        ""markDefs"": []
    },
    {
        ""_type"": ""block"",
        ""children"": [
            { ""_type"": ""span"", ""text"": ""After"" }
        ]
    }
]
");

        result.ShouldBe("<p>After</p>");
    }

    [Fact]
    public void GivenOnlyBlockWithoutChildrenArray_ReturnsNull()
    {
        var result = PortableTextToHtml.Render(@"[{ ""_type"": ""block"", ""children"": null }]");

        result.ShouldBeNull();
    }

    [Fact]
    public void GivenListItemWithoutChildrenArray_IgnoresItemLikeUnknownType()
    {
        var result = PortableTextToHtml.Render(@"
[
    {
        ""_type"": ""block"",
        ""listItem"": ""bullet"",
        ""children"": [
            { ""_type"": ""span"", ""text"": ""First"" }
        ]
    },
    {
        ""_type"": ""block"",
        ""listItem"": ""bullet"",
        ""children"": null
    },
    {
        ""_type"": ""block"",
        ""listItem"": ""bullet"",
        ""children"": [
            { ""_type"": ""span"", ""text"": ""Second"" }
        ]
    }
]
");

        result.ShouldBe("<ul><li>First</li></ul><ul><li>Second</li></ul>");
    }

    [Fact]
    public void GivenFirstListItemWithoutChildrenArray_IgnoresItem()
    {
        var result = PortableTextToHtml.Render(@"
[
    {
        ""_type"": ""block"",
        ""listItem"": ""bullet""
    },
    {
        ""_type"": ""block"",
        ""listItem"": ""bullet"",
        ""children"": [
            { ""_type"": ""span"", ""text"": ""Item"" }
        ]
    }
]
");

        result.ShouldBe("<ul><li>Item</li></ul>");
    }

    [Theory]
    [InlineData("5")]
    [InlineData("null")]
    [InlineData(@"""Hidden""")]
    [InlineData("[]")]
    [InlineData(@"{ ""text"": ""Hidden"" }")]
    [InlineData(@"{ ""_type"": 5, ""text"": ""Hidden"" }")]
    [InlineData(@"{ ""_type"": null, ""text"": ""Hidden"" }")]
    public void GivenChildWithoutStringType_IgnoresChild(string child)
    {
        var result = RenderBlockWithMiddleChild(child);

        result.ShouldBe("<p>AB</p>");
    }

    [Theory]
    [InlineData(@"{ ""_type"": ""span"", ""marks"": [""strong""] }")]
    [InlineData(@"{ ""_type"": ""span"", ""marks"": [""strong""], ""text"": null }")]
    [InlineData(@"{ ""_type"": ""span"", ""marks"": [""strong""], ""text"": 5 }")]
    [InlineData(@"{ ""_type"": ""span"", ""marks"": [""strong""], ""text"": true }")]
    [InlineData(@"{ ""_type"": ""span"", ""marks"": [""strong""], ""text"": [""Hidden""] }")]
    public void GivenSpanWithoutStringText_IgnoresSpan(string span)
    {
        var result = RenderBlockWithMiddleChild(span);

        result.ShouldBe("<p>AB</p>");
    }

    private static string RenderBlockWithMiddleChild(string child)
    {
        return PortableTextToHtml.Render(@"
[
    {
        ""_type"": ""block"",
        ""children"": [
            { ""_type"": ""span"", ""text"": ""A"" },
            " + child + @",
            { ""_type"": ""span"", ""text"": ""B"" }
        ]
    }
]
");
    }
}
