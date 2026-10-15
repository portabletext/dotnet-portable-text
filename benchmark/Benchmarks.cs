using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;

namespace PortableText
{
    [MemoryDiagnoser]
    [ExceptionDiagnoser]
    public class Benchmarks
    {
        private string json;
        private string article;
        private PortableTextSerializers customSerializers;

        [GlobalSetup]
        public void Setup()
        {
            json = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "data", "bigcontent.json"));

            // A typical article: the first 25 elements of the large document (lists, marks and links included)
            using var document = JsonDocument.Parse(json);
            article = "[" + string.Join(",", document.RootElement.EnumerateArray().Take(25).Select(x => x.GetRawText())) + "]";

            customSerializers = new PortableTextSerializers
            {
                BlockStyleSerializers = new Dictionary<string, Func<IEnumerable<string>, string>>
                {
                    { "h3", blocks => $@"<h3 class=""title"">{string.Join(string.Empty, blocks)}</h3>" }
                },
                MarkSerializers = new MarkSerializer
                {
                    Decorators = new Dictionary<string, Func<(string, string)>>
                    {
                        { "strong", () => ("<b>", "</b>") }
                    }
                }
            };
        }

        [Benchmark]
        public string HugeJson() => PortableTextToHtml.Render(json);

        [Benchmark]
        public string Article() => PortableTextToHtml.Render(article);

        [Benchmark]
        public string ArticleWithCustomSerializers() => PortableTextToHtml.Render(article, customSerializers);
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var config = DefaultConfig.Instance
                .AddJob(Job.Default.WithRuntime(CoreRuntime.Core80))
                .AddJob(Job.Default.WithRuntime(CoreRuntime.Core10_0));

            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, config);
        }
    }
}
