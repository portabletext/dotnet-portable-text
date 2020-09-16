using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Sanity;
using System.IO;

namespace Benchmark
{
    [SimpleJob(RuntimeMoniker.Net472, baseline: true)]
    [SimpleJob(RuntimeMoniker.NetCoreApp30)]
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
        public string HugeJson() => BlockContentToHtml.Render(json);
    }
}
