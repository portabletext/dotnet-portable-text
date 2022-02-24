[![test](https://github.com/saasen/dotnet-portable-text/actions/workflows/test.yml/badge.svg)](https://github.com/saasen/dotnet-portable-text/actions/workflows/test.yml)
![Nuget](https://img.shields.io/nuget/v/PortableText?color=blue&logo=nuget)

# .NET Portable Text

This repo contains tools for working with [Portable Text](https://portabletext.org) in .NET.

## Installation

```
dotnet add package PortableText
```

## Basic Usage

### Rendering

```cs
using PortableText;

var result = PortableTextToHtml.Render(
    json,       // A string value representing your Portable Text
    serializers // Optional. Specifies how to render a certain type, mark, list etc.
);
```

## Customizing rendering

You can pass custom serializers in the `serializers` parameter.

### Types

A type serializer takes in a JSON de-serialized type representing your data.

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
                    // We are specifying which type we want the JSON de-serialized to, so this is safe.
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

### Block styles

Block styles typically describes a visual property for the whole block.

You can customize rendering of block styles like this:

```cs
var serializers = new PortableTextSerializers
{
    BlockStyleSerializers = new Dictionary<string, Func<IEnumerable<string>, string>>
    {
        { "h1", blocks => $"<h1 className="text-2xl">{string.Join(string.Empty, blocks)}</h1>" }
    }
};

var result = PortableTextToHtml.Render(
    json,
    serializers
);
```

### Marks

This library separates annotation marks and decorator marks.

#### Decorator marks

Decorator marks are marks that mean something by themselves.
Examples of decorator marks would be "em" or "strong" which could mean to emphasize or bold text.

This example shows how to render a in-line link in your text:

```cs
var serializers = new PortableTextSerializers
{
    MarkSerializers = new Dictionary<string, Func<PortableTextBlock, PortableTextChild, string, (string startTag, string endTag)>>
    {
        Decorators = new Dictionary<string, Func<(string, string)>>
        {
            {
                "highlight", () =>
                {
                    return ("<em>", "</em>>");
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

#### Annotation marks

Annotation marks are marks that needs additional information other than the name of the mark itself for it to be useful.
Examples of this would be a "link". A link needs a URL to be meaningful.

```cs
public class LinkPortableTextMarkAnnotation
{
    public string Href { get; set; }
}

var serializers = new PortableTextSerializers
{
    MarkSerializers = new Dictionary<string, Func<PortableTextBlock, PortableTextChild, string, (string startTag, string endTag)>>
    {
        Annotations = new Dictionary<string, AnnotatedMarkSerializer>
        {
            {
                "link",
                new AnnotatedMarkSerializer
                {
                    Type = typeof(LinkPortableTextMarkAnnotation),
                    Serialize = (value, rawValue) =>
                    {
                        var typed = value as LinkPortableTextMarkAnnotation;

                        return ($@"<a href=""{typed.Href}"">", "</a>");
                    }
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

### Lists

This example shows how to customize rendering of a list. It uses the thumbs up sign as the list style type:

```cs
var serializers = new PortableTextSerializers
{
    ListSerializers = new Dictionary<string, Func<IEnumerable<string>, string>>()
    {
        { "bullet", listItems => @$"<ul style=""list-style-type: ""\1F44D"""">{string.Join(string.Empty, listItems)}</ul>" }
    }
};

var result = PortableTextToHtml.Render(
    json,
    serializers
);
```

### List items

This example shows how to customize rendering of a list item:

```cs
var serializers = new PortableTextSerializers
{
    ListItemSerializers = new Dictionary<string, Func<(string, string)>>
    {
         { "bullet", () => (@"<li style=""list-style-type: circle;"">🎱 ", "</li>") }
    }
};

var result = PortableTextToHtml.Render(
    json,
    serializers
);
```

## Unknown types

When this library encounters unknown types, they are ignored and you get no warnings.
If your type isn't outputted, you are probably missing a serializer for it.
