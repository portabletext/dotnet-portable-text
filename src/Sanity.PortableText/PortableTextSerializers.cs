using System;
using System.Collections.Generic;

namespace PortableText;

public class PortableTextSerializers
{
    public PortableTextSerializers()
    {
        TypeSerializers = new Dictionary<string, TypeSerializer>();
        MarkSerializers = new Dictionary<string, Func<PortableTextBlock, PortableTextChild, string, (string, string)>>();
        BlockStyleSerializers = new Dictionary<string, Func<IEnumerable<string>, string>>();
    }
    public Dictionary<string, TypeSerializer> TypeSerializers { get; set; }
    public Dictionary<string, Func<PortableTextBlock, PortableTextChild, string, (string, string)>> MarkSerializers { get; set; }
    public Dictionary<string, Func<IEnumerable<string>, string>> BlockStyleSerializers { get; set; }
}