using Shouldly;
using Xunit;

namespace PortableText;

public partial class Tests
{
    [Theory]
    [InlineData("5")]
    [InlineData("true")]
    [InlineData("[]")]
    public void GivenStyleOfWrongType_RendersNormalBlock(string style)
    {
        var result = PortableTextToHtml.Render(@"
[
    {
        ""_type"": ""block"",
        ""style"": " + style + @",
        ""children"": [
            { ""_type"": ""span"", ""text"": ""Test"" }
        ]
    }
]
");

        result.ShouldBe("<p>Test</p>");
    }

    [Theory]
    [InlineData("5")]
    [InlineData("null")]
    [InlineData("true")]
    public void GivenListItemOfWrongType_RendersBlockOutsideList(string listItem)
    {
        var result = PortableTextToHtml.Render(@"
[
    {
        ""_type"": ""block"",
        ""listItem"": " + listItem + @",
        ""children"": [
            { ""_type"": ""span"", ""text"": ""Test"" }
        ]
    }
]
");

        result.ShouldBe("<p>Test</p>");
    }

    [Theory]
    [InlineData(@"""2""")]
    [InlineData("true")]
    [InlineData("2147483648")]
    public void GivenInvalidListLevel_RendersBlockOutsideList(string level)
    {
        var result = RenderListWithSecondLevel(level);

        result.ShouldBe("<ul><li>First</li></ul><p>Second</p>");
    }

    [Fact]
    public void GivenNullListLevel_RendersItemOnFirstLevel()
    {
        var result = RenderListWithSecondLevel("null");

        result.ShouldBe("<ul><li>First</li><li>Second</li></ul>");
    }

    [Theory]
    [InlineData("1.9", "<ul><li>First</li><li>Second</li></ul>")]
    [InlineData("2.5", "<ul><li>First<ul><li>Second</li></ul></li></ul>")]
    public void GivenListLevelWithDecimals_RoundsDown(string level, string expected)
    {
        var result = RenderListWithSecondLevel(level);

        result.ShouldBe(expected);
    }

    private static string RenderListWithSecondLevel(string level)
    {
        return PortableTextToHtml.Render(@"
[
    {
        ""_type"": ""block"",
        ""listItem"": ""bullet"",
        ""level"": 1,
        ""children"": [
            { ""_type"": ""span"", ""text"": ""First"" }
        ]
    },
    {
        ""_type"": ""block"",
        ""listItem"": ""bullet"",
        ""level"": " + level + @",
        ""children"": [
            { ""_type"": ""span"", ""text"": ""Second"" }
        ]
    }
]
");
    }

    [Theory]
    [InlineData(@"""2""")]
    [InlineData("true")]
    public void GivenLevelOfWrongTypeOutsideList_RendersNormalBlock(string level)
    {
        var result = PortableTextToHtml.Render(@"
[
    {
        ""_type"": ""block"",
        ""level"": " + level + @",
        ""children"": [
            { ""_type"": ""span"", ""text"": ""Test"" }
        ]
    }
]
");

        result.ShouldBe("<p>Test</p>");
    }
}
