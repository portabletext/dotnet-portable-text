using System;
using System.Collections.Generic;

namespace PortableText;

/// <summary>
/// A class representing all the render extension points when rendering Portable Text.
/// </summary>
public class PortableTextSerializers
{
    public PortableTextSerializers()
    {
        TypeSerializers = new Dictionary<string, TypeSerializer>();
        MarkSerializers = new MarkSerializer();
        BlockStyleSerializers = new Dictionary<string, Func<IEnumerable<string>, string>>();
        ListSerializers = new Dictionary<string, Func<IEnumerable<string>, string>>();
        ListItemSerializers = new Dictionary<string, Func<(string, string)>>();
    }

    /// <summary>
    /// Gets or sets the value of the <see cref="Dictionary{TKey,TValue}"/>.
    /// See <see cref="TypeSerializer"/> for more information.
    /// </summary>
    public Dictionary<string, TypeSerializer> TypeSerializers { get; set; }
    
    /// <summary>
    /// Gets or sets the value of the <see cref="Dictionary{TKey,TValue}"/>.
    /// See <see cref="MarkSerializer"/> for more information.
    /// </summary>
    public MarkSerializer MarkSerializers { get; set; }
    
    /// <summary>
    /// Gets or sets the value of the <see cref="Dictionary{TKey,TValue}"/>.
    /// How you want to apply styles to a block. You return a string with all your generated HTML.
    /// The key of this dictionary is the _type of block you want to target
    /// The value is a method taking in all the text of the children in a block, and returning a string with all your generated HTML.
    /// </summary>
    public Dictionary<string, Func<IEnumerable<string>, string>> BlockStyleSerializers { get; set; }
    
    /// <summary>
    /// Gets or sets the value of the <see cref="Dictionary{TKey,TValue}"/>.
    /// How you want to render lists. This is only for rendering the list itself, not it's children. See <see cref="ListItemSerializers"/> for that.
    /// The key of this dictionary is the _type of list you want to target
    /// The value is a method taking in all the list children, and returning a string with all your generated HTML.
    /// </summary>
    public Dictionary<string, Func<IEnumerable<string>, string>> ListSerializers { get; set; }
    
    /// <summary>
    /// Gets or sets the value of the <see cref="Dictionary{TKey,TValue}"/>.
    /// How you want to render list items. This is only for rendering list items themselves, the list itself. See <see cref="ListSerializers"/> for that.
    /// The key of this dictionary is the _type of list item you want to target
    /// The value is a method returning a <see cref="Tuple{T1,T2}"/> representing the HTML start- and end-tags of the list item.
    /// </summary>
    public Dictionary<string, Func<(string, string)>> ListItemSerializers { get; set; }
}

/// <summary>
/// A class representing how to render annotation marks and decorator marks.
/// See the README for more information and examples. 
/// </summary>
public class MarkSerializer
{
    public MarkSerializer()
    {
        Annotations = new Dictionary<string, AnnotatedMarkSerializer>();
        Decorators = new Dictionary<string, Func<(string, string)>>();
    }
    
    /// <summary>
    /// Annotation marks are marks that needs additional information other than the name of the mark itself.
    /// Examples of this would be a "link". A link needs a URL to be meaningful.
    ///
    /// The key of this dictionary is a string representing the _type of this annotation mark
    /// The value of this dictionary is an object with a type representing your custom annotation mark type and a <see cref="AnnotatedMarkSerializer.Serialize"/> method that returns the generated HTML.
    /// See <see cref="AnnotatedMarkSerializer"/> for more information.
    /// </summary>
    public Dictionary<string, AnnotatedMarkSerializer> Annotations { get; set; }
    
    /// <summary>
    /// Decorator marks are marks that mean something in themselves just by the name.
    /// Examples of this would be "em" or "strong" which could mean to emphasize or bold text.
    ///
    /// The key of this dictionary is a string representing the _type of this decorator mark
    /// The value of this dictionary is a method that returns HTML start- and end-tag in a <see cref="Tuple{T1,T2}"/>.
    /// </summary>
    public Dictionary<string, Func<(string, string)>> Decorators { get; set; }
}

/// <summary>
/// A class representing how to render a an annotated mark in Portable Text.
/// See the README for more information and examples.
/// </summary>
public class AnnotatedMarkSerializer
{
    /// <summary>
    /// The type you want to be passed into your <see cref="Serialize"/> method.
    /// </summary>
    public Type Type { get; set; }
    
    /// <summary>
    /// How you want to serialize your custom annotated mark to HTML. You return a string with all your generated HTML.
    /// <list type="number">
    ///     <item>
    ///         <description>object: The value this library de-serializes your <see cref="Type"/> to. You can as operator you convert it to your desired type.</description>
    ///     </item>
    ///     <item>
    ///         <description>string: The raw JSON element/value of the node in question.</description>
    ///     </item>
    ///     <item>
    ///         <description>(string, string): A tuple containing the HTML start-tag as the first value, and the HTML end-tag as the second value.</description>
    ///     </item>
    /// </list>
    /// </summary>
    public Func<object, string, (string, string)> Serialize { get; set; }
}

/// <summary>
/// A class representing how to render a certain type in Portable Text.
/// See the README for more information and examples.
/// </summary>
public class TypeSerializer
{
    /// <summary>
    /// The type you want to be passed into your <see cref="Serialize"/> method.
    /// </summary>
    public Type Type { get; set; }
    
    /// <summary>
    /// How you want to serialize your custom type to HTML. You return a string with all your generated HTML.
    /// <list type="number">
    ///     <item>
    ///         <description>object: The value this library de-serializes your <see cref="Type"/> to. You can as operator you convert it to your desired type.</description>
    ///     </item>
    ///     <item>
    ///         <description>string: The raw JSON element/value of the node in question.</description>
    ///     </item>
    ///     <item>
    ///         <description>PortableTextSerializers: All the serializers gets passed in to the <see cref="Serialize"/> method in case you need it.</description>
    ///     </item>
    ///     <item>
    ///         <description>bool: If this node is inlined in a block or not..</description>
    ///     </item>
    /// </list>
    /// </summary>
    public Func<object, string, PortableTextSerializers, bool, string> Serialize { get; set; }
}
