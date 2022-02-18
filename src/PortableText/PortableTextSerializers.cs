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
        ListSerializers = new Dictionary<string, Func<IEnumerable<string>, string>>();
        ListItemSerializers = new Dictionary<string, Func<(string, string)>>();
    }
    public Dictionary<string, TypeSerializer> TypeSerializers { get; set; }
    public Dictionary<string, Func<PortableTextBlock, PortableTextChild, string, (string, string)>> MarkSerializers { get; set; }
    public Dictionary<string, Func<IEnumerable<string>, string>> BlockStyleSerializers { get; set; }
    public Dictionary<string, Func<IEnumerable<string>, string>> ListSerializers { get; set; }
    public Dictionary<string, Func<(string, string)>> ListItemSerializers { get; set; }
}

public class TypeSerializer
{
    public Type Type { get; set; }
    public Func<object, string, PortableTextSerializers, bool, string> Serialize { get; set; }
}
