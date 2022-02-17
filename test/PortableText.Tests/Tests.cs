using Xunit;
using FluentAssertions;
using System.IO;

namespace PortableText
{
    public class Test
    {
        [Fact]
        public void HandlesNull()
        {
            var result = PortableTextToHtml.Render(null);
            result.Should().BeNull();
        }

        [Fact]
        public void AcceptsEmptyJson()
        {
            var result = PortableTextToHtml.Render(string.Empty);
            result.Should().BeNull();
        }

        [Fact]
        public void ThrowsOnInvalidJson()
        {
            bool success = false;
            try
            {
                PortableTextToHtml.Render("[{]");
                success = true;
            }
            catch
            {
            }

            Assert.False(success);
        }

        [Fact]
        public void HandlesEmptyJsonArray()
        {
            var result = PortableTextToHtml.Render("[]");

            result.Should().Be(null);
        }

        [Fact]
        public void HandlesJsonArrayWithEmptyObject()
        {
            var result = PortableTextToHtml.Render("[{}]");

            result.Should().Be(null);
        }

        [Fact]
        public void GivenNoCustomSerializers_AndCustomObjectsArePresent_ShouldNotCrash()
        {
            var json = @"
[
    {
        ""_key"": ""88778f6b7024"",
        ""_type"": ""youtubeEmbed"",
        ""title"": ""Top 10 goals Jon Dahl Tomasson"",
        ""url"": ""https://youtu.be/8d9vXiGrYck""
    }
]
";
            var result = PortableTextToHtml.Render(json);

            result.Should().Be(null);
        }
    }
}