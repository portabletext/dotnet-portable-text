using System;
using System.Collections.Generic;

namespace stuff
{
    public class PortableTextBlock
    {
        public string Key { get; set; }
        // public PortableTextChild[] Children { get; set; }
        public string Type { get; set; }
        // public MarkDefinition[] MarkDefinitions { get; set; }
        // public string Style { get; set; }
    }

    public class YoutubeEmbed : PortableTextBlock
    {
        public string Title { get; set; }

        public void stuff()
        {
            var stuf = new PortableTextSerializers
            {
                TypeSerializers = new Dictionary<string, Func<object, PortableTextSerializers, string>>
                {
                    {
                        "youtubeEmbed", (block, serializers) =>
                        {
                            var typedBlock = block as YoutubeEmbed;
                            if (typedBlock == null)
                            {
                                return string.Empty;
                            }

                            return "wut";
                        }
                    }
                }
            };
        }
    }


    public class PortableTextSerializers
    {
        public Dictionary<string, Func<object, PortableTextSerializers, string>> TypeSerializers { get; set; }
        //public Dictionary<string, Func<PortableTextBlock, PortableTextChild, (string, string)>> MarkSerializers { get; set; }
    }
}