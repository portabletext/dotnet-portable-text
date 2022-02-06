using System;

namespace PortableText;

public class TypeSerializer
{
    public Type Type { get; set; }
    public Func<object, PortableTextSerializers, string> Serialize { get; set; }
}