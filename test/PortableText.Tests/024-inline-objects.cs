using System;
using FluentAssertions;
using Xunit;

namespace PortableText;

public partial class Tests
{
    private class Currency : PortableTextChild
    {
        public string SourceCurrency { get; set; }
        public decimal SourceAmount { get; set; }
    }
    
    [Fact]
    public void InlineObjects()
    {
        var serializers = new PortableTextSerializers
        {
            TypeSerializers = new()
            {
                {
                    "localCurrency",
                    new()
                    {
                        Type = typeof(Currency),
                        Serialize = (value, _, _, _) =>
                        {
                            var currency = value as Currency;
                            var rates = new System.Collections.Generic.Dictionary<string, decimal>
                            {
                                { "USD", 8.82m },
                                { "DKK", 1.35m },
                                { "EUR", 10.04m }
                            };
                            var rate = rates[currency.SourceCurrency] == default
                                ? 1m
                                : rates[currency.SourceCurrency];
                            return $@"<span class=""currency"">~{Math.Round(currency.SourceAmount * rate)} NOK</span>";
                        }
                    }
                }
            }
        };
        var result = PortableTextToHtml.Render(@"
[
    {
        ""_key"": ""08707ed2945b"",
        ""_type"": ""block"",
        ""style"": ""normal"",
        ""children"": [
            {
                ""_key"": ""08707ed2945b0"",
                ""text"": ""Foo! Bar!"",
                ""_type"": ""span"",
                ""marks"": [
                    ""code""
                ]
            },
            {
                ""_key"": ""a862cadb584f"",
                ""_type"": ""localCurrency"",
                ""sourceCurrency"": ""USD"",
                ""sourceAmount"": 13.5
            },
            {
                ""_key"": ""08707ed2945b1"",
                ""text"": ""Neat"",
                ""_type"": ""span"",
                ""marks"": []
            }
        ],
        ""markDefs"": []
    },
    {
        ""_key"": ""abc"",
        ""_type"": ""block"",
        ""style"": ""normal"",
        ""children"": [
            {
                ""_key"": ""08707ed2945b0"",
                ""text"": ""Foo! Bar! "",
                ""_type"": ""span"",
                ""marks"": [
                    ""code""
                ]
            },
            {
                ""_key"": ""a862cadb584f"",
                ""_type"": ""localCurrency"",
                ""sourceCurrency"": ""DKK"",
                ""sourceAmount"": 200
            },
            {
                ""_key"": ""08707ed2945b1"",
                ""text"": "" Baz!"",
                ""_type"": ""span"",
                ""marks"": [
                    ""code""
                ]
            }
        ],
        ""markDefs"": []
    },
    {
        ""_key"": ""def"",
        ""_type"": ""block"",
        ""style"": ""normal"",
        ""children"": [
            {
                ""_key"": ""08707ed2945b0"",
                ""text"": ""Foo! Bar! "",
                ""_type"": ""span"",
                ""marks"": []
            },
            {
                ""_key"": ""a862cadb584f"",
                ""_type"": ""localCurrency"",
                ""sourceCurrency"": ""EUR"",
                ""sourceAmount"": 25
            },
            {
                ""_key"": ""08707ed2945b1"",
                ""text"": "" Baz!"",
                ""_type"": ""span"",
                ""marks"": [
                    ""code""
                ]
            }
        ],
        ""markDefs"": []
    }
]
", serializers);
        
        result.Should().Be(@"<p><code>Foo! Bar!</code><span class=""currency"">~119 NOK</span>Neat</p><p><code>Foo! Bar! </code><span class=""currency"">~270 NOK</span><code> Baz!</code></p><p>Foo! Bar! <span class=""currency"">~251 NOK</span><code> Baz!</code></p>");
    }
}