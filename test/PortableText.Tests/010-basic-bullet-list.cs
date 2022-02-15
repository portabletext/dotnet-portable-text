using FluentAssertions;
using Xunit;

namespace PortableText;

public partial class Tests
{
    [Fact]
    public void BasicBulletList()
    {
        var result = PortableTextToHtml.Render(@"
[
    {
        ""_key"": ""f94596b05b41"",
        ""_type"": ""block"",
        ""children"": [
            {
                ""_type"": ""span"",
                ""marks"": [],
                ""text"": ""Let's test some of these lists!""
            }
        ],
        ""markDefs"": [],
        ""style"": ""normal""
    },
    {
        ""_key"": ""937effb1cd06"",
        ""_type"": ""block"",
        ""level"": 1,
        ""listItem"": ""bullet"",
        ""children"": [
            {
                ""_type"": ""span"",
                ""marks"": [],
                ""text"": ""Bullet 1""
            }
        ],
        ""markDefs"": [],
        ""style"": ""normal""
    },
    {
        ""_key"": ""bd2d22278b88"",
        ""_type"": ""block"",
        ""level"": 1,
        ""listItem"": ""bullet"",
        ""children"": [
            {
                ""_type"": ""span"",
                ""marks"": [],
                ""text"": ""Bullet 2""
            }
        ],
        ""markDefs"": [],
        ""style"": ""normal""
    },
    {
        ""_key"": ""a97d32e9f747"",
        ""_type"": ""block"",
        ""level"": 1,
        ""listItem"": ""bullet"",
        ""children"": [
            {
                ""_type"": ""span"",
                ""marks"": [],
                ""text"": ""Bullet 3""
            }
        ],
        ""markDefs"": [],
        ""style"": ""normal""
    }
]
");
        // TODO: We have to HTML encode the value (!)
        result.Should().Be(string.Join("",
            "<p>Let&#x27;s test some of these lists!</p>",
            "<ul>",
            "<li>Bullet 1</li>",
            "<li>Bullet 2</li>",
            "<li>Bullet 3</li>",
            "</ul>"
        ));
    }
}