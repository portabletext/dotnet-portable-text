# .NET Portable Text

This repo contains tools for working with [Portable Text](https://portabletext.org) in .NET.

## Installation

TBD

## Basic Usage

```cs
using PortableText;

var result = PortableTextToHtml.Render(
    json,       // A string value representing your Portable Text
    serializers // Optional. Specifies how to render a certain type, mark, list etc.
);
```

## Customizing rendering

You can pass custom serializers in the `serializers` parameter. A serializer takes in a JSON de-serialized type representing your data.

This example shows how to render a in-line link in your text:

```cs
var serializers = new PortableTextSerializers
{
    MarkSerializers = new Dictionary<string, Func<PortableTextBlock, PortableTextChild, string, (string startTag, string endTag)>>
    {
        {
            "link", (block, blockChild, mark) =>
            {
                var link = block.MarkDefinitions.First(x => x.Key == mark);
                return ($"<a href=\"{link.Href}\">", "</a>");
            }
        }
    }
};

var result = PortableTextToHtml.Render(
    json,
    serializers
);
```

This example shows how to render a custom type:

```cs
var serializers = new PortableTextSerializers
{
    TypeSerializers = new Dictionary<string, TypeSerializer>
    {
        {
            "youtubeEmbed", new TypeSerializer
            {
                Type = typeof(YoutubeEmbed),
                Serialize = (block, serializers) =>
                {
                    // We are specifying which type we want the JSON serialized to, so this is safe.
                    var typedBlock = block as YoutubeEmbed;
                    return $@"<iframe title=""{typedBlock.Title}"" href=""{typedBlock.Url}""></iframe>";
                }
            }
        }
    }
};

var result = PortableTextToHtml.Render(
    json,
    serializers
);
```

## Unknown types

When this library encounters unknown types, they are ignored and you get no warnings. If your type isn't outputted, you are probably missing a serializer for it.