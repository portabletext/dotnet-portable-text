using System;

namespace Sanity;

public class TypeSerializer
{
    public Type Type { get; set; }
    public Func<object, PortableTextSerializers, string> Serialize { get; set; }
}