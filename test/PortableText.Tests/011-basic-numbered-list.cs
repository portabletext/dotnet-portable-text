using FluentAssertions;
using Xunit;

namespace PortableText;

public partial class Tests
{
    [Fact]
    public void BasicNumberedList()
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
        ""listItem"": ""number"",
        ""children"": [
            {
                ""_type"": ""span"",
                ""marks"": [],
                ""text"": ""Number 1""
            }
        ],
        ""markDefs"": [],
        ""style"": ""normal""
    },
    {
        ""_key"": ""bd2d22278b88"",
        ""_type"": ""block"",
        ""level"": 1,
        ""listItem"": ""number"",
        ""children"": [
            {
                ""_type"": ""span"",
                ""marks"": [],
                ""text"": ""Number 2""
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
                ""text"": ""Number 3""
            }
        ],
        ""markDefs"": [],
        ""style"": ""normal""
    }
]
");
        // TODO: This test fails because we add a p-tag to all li-s
        result.Should().Be(string.Join("",
            "<p>Let&#x27;s test some of these lists!</p>",
            "<ul>",
                "<li>Number 1</li>",
                "<li>Number 2</li>",
                "<li>Number 3</li>",
            "</ul>"
        ));
    }
}