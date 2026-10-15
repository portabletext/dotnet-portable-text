using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using System.IO;
using BenchmarkDotNet.Running;

namespace PortableText
{
    [SimpleJob(RuntimeMoniker.Net80)]
    [SimpleJob(RuntimeMoniker.Net10_0)]
    [RPlotExporter]
    public class Benchmarks
    {
        private string json;

        [GlobalSetup]
        public void Setup()
        {
            json = File.ReadAllText("../../../../../../../data/bigcontent.json");
        }

        [Benchmark]
        public string HugeJson() => PortableTextToHtml.Render(json);
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<Benchmarks>();
        }
    }
}
