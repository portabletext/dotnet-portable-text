using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using System.IO;

namespace PortableText
{
    [SimpleJob(RuntimeMoniker.Net472, baseline: true)]
    [SimpleJob(RuntimeMoniker.NetCoreApp31)]
    [SimpleJob(RuntimeMoniker.NetCoreApp50)]
    [RPlotExporter]
    public class Benchmarks
    {
        private string json;

        [GlobalSetup]
        public void Setup()
        {
            json = File.ReadAllText("C:/code/dotnet-portable-text-to-html/test/Tests/data/bigcontent.json");
        }

        [Benchmark]
        public string HugeJson() => PortableTextToHtml.Render(json);
    }
}
