using FluentAssertions;
using Xunit;

namespace PortableText;

public partial class Tests
{
    [Fact]
    public void NestedLists()
    {
        var result = PortableTextToHtml.Render(@"
[
    {
        ""_key"": ""a"",
        ""_type"": ""block"",
        ""children"": [
            {
                ""_type"": ""span"",
                ""marks"": [],
                ""text"": ""Span""
            }
        ],
        ""markDefs"": [],
        ""style"": ""normal""
    },
    {
        ""_key"": ""b"",
        ""_type"": ""block"",
        ""listItem"": ""bullet"",
        ""level"": 1,
        ""children"": [
            {
                ""_type"": ""span"",
                ""marks"": [],
                ""text"": ""Item 1, level 1""
            }
        ],
        ""markDefs"": [],
        ""style"": ""normal""
    },
    {
        ""_key"": ""c"",
        ""_type"": ""block"",
        ""listItem"": ""bullet"",
        ""level"": 1,
        ""children"": [
            {
                ""_type"": ""span"",
                ""marks"": [],
                ""text"": ""Item 2, level 1""
            }
        ],
        ""markDefs"": [],
        ""style"": ""normal""
    },
    {
        ""_key"": ""d"",
        ""_type"": ""block"",
        ""listItem"": ""bullet"",
        ""level"": 2,
        ""children"": [
            {
                ""_type"": ""span"",
                ""marks"": [],
                ""text"": ""Item 3, level 2""
            }
        ],
        ""markDefs"": [],
        ""style"": ""normal""
    },
    {
        ""_key"": ""e"",
        ""_type"": ""block"",
        ""listItem"": ""number"",
        ""level"": 3,
        ""children"": [
            {
                ""_type"": ""span"",
                ""marks"": [],
                ""text"": ""Item 4, level 3""
            }
        ],
        ""markDefs"": [],
        ""style"": ""normal""
    },
    {
        ""_key"": ""f"",
        ""_type"": ""block"",
        ""listItem"": ""number"",
        ""level"": 2,
        ""children"": [
            {
                ""_type"": ""span"",
                ""marks"": [],
                ""text"": ""Item 5, level 2""
            }
        ],
        ""markDefs"": [],
        ""style"": ""normal""
    },
    {
        ""_key"": ""g"",
        ""_type"": ""block"",
        ""listItem"": ""number"",
        ""level"": 2,
        ""children"": [
            {
                ""_type"": ""span"",
                ""marks"": [],
                ""text"": ""Item 6, level 2""
            }
        ],
        ""markDefs"": [],
        ""style"": ""normal""
    },
    {
        ""_key"": ""h"",
        ""_type"": ""block"",
        ""listItem"": ""bullet"",
        ""level"": 1,
        ""children"": [
            {
                ""_type"": ""span"",
                ""marks"": [],
                ""text"": ""Item 7, level 1""
            }
        ],
        ""markDefs"": [],
        ""style"": ""normal""
    },
    {
        ""_key"": ""i"",
        ""_type"": ""block"",
        ""listItem"": ""bullet"",
        ""level"": 1,
        ""children"": [
            {
                ""_type"": ""span"",
                ""marks"": [],
                ""text"": ""Item 8, level 1""
            }
        ],
        ""markDefs"": [],
        ""style"": ""normal""
    },
    {
        ""_key"": ""j"",
        ""_type"": ""block"",
        ""listItem"": ""number"",
        ""level"": 1,
        ""children"": [
            {
                ""_type"": ""span"",
                ""marks"": [],
                ""text"": ""Item 1, list 2""
            }
        ],
        ""markDefs"": [],
        ""style"": ""normal""
    },
    {
        ""_key"": ""k"",
        ""_type"": ""block"",
        ""listItem"": ""number"",
        ""level"": 1,
        ""children"": [
            {
                ""_type"": ""span"",
                ""marks"": [],
                ""text"": ""Item 2, list 2""
            }
        ],
        ""markDefs"": [],
        ""style"": ""normal""
    },
    {
        ""_key"": ""l"",
        ""_type"": ""block"",
        ""listItem"": ""number"",
        ""level"": 2,
        ""children"": [
            {
                ""_type"": ""span"",
                ""marks"": [],
                ""text"": ""Item 3, list 2, level 2""
            }
        ],
        ""markDefs"": [],
        ""style"": ""normal""
    },
    {
        ""_key"": ""m"",
        ""_type"": ""block"",
        ""children"": [
            {
                ""_type"": ""span"",
                ""marks"": [],
                ""text"": ""Just a block""
            }
        ],
        ""markDefs"": [],
        ""style"": ""normal""
    }
]
");
        // TODO: Two bugs here:
        // TODO:    * When you suddenly see another type of list deep in a level, it starts on level 1
        // TODO:    * We are using the block serializer when serializing the list value, so we get a p-tag since it's a "normal" block.
        // TODO:        It should probably solely be the listItem type that decides that, but what about marks?
        result.Should().Be(string.Join("",
            "<p>Span</p>",
            "<ul>",
                "<li>Item 1, level 1</li>",
                "<li>",
                    "Item 2, level 1",
                    "<ol>",
                        "<li>",
                            "Item 3, level 2",
                            "<ol>",
                                "<li>Item 4, level 3</li>",
                            "</ol>",
                        "</li>",
                        "<li>Item 5, level 2</li>",
                        "<li>Item 6, level 2</li>",
                    "</ol>",
                "</li>",
                "<li>Item 7, level 1</li>",
                "<li>Item 8, level 1</li>",
            "</ul>",
            "<ol>",
                "<li>Item 1 of list 2</li>",
                "<li>",
                    "Item 2 of list 2",
                    "<ol>",
                        "<li>Item 3 of list 2, level 2</li>",
                    "</ol>",
                "</li>",
            "</ol>",
            "<p>Just a block</p>"
        ));
    }
}