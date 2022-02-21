using System;
using System.Collections.Generic;

namespace PortableText;

public class PortableTextSerializers
{
    public PortableTextSerializers()
    {
        TypeSerializers = new Dictionary<string, TypeSerializer>();
        MarkSerializers = new MarkSerializers();
        BlockStyleSerializers = new Dictionary<string, Func<IEnumerable<string>, string>>();
        ListSerializers = new Dictionary<string, Func<IEnumerable<string>, string>>();
        ListItemSerializers = new Dictionary<string, Func<(string, string)>>();
    }

    public Dictionary<string, TypeSerializer> TypeSerializers { get; set; }
    public MarkSerializers MarkSerializers { get; set; }
    public Dictionary<string, Func<IEnumerable<string>, string>> BlockStyleSerializers { get; set; }
    public Dictionary<string, Func<IEnumerable<string>, string>> ListSerializers { get; set; }
    public Dictionary<string, Func<(string, string)>> ListItemSerializers { get; set; }
}

public class MarkSerializers
{
    public MarkSerializers()
    {
        Annotations = new Dictionary<string, AnnotatedMarkSerializer>();
        Decorators = new Dictionary<string, Func<(string, string)>>();
    }
    
    public Dictionary<string, AnnotatedMarkSerializer> Annotations { get; set; }
    public Dictionary<string, Func<(string, string)>> Decorators { get; set; }
}

public class AnnotatedMarkSerializer
{
    public Type Type { get; set; }
    public Func<object, string, (string, string)> Serialize { get; set; }
}

public class TypeSerializer
{
    public Type Type { get; set; }
    public Func<object, string, PortableTextSerializers, bool, string> Serialize { get; set; }
}
