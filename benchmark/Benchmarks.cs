using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using System.IO;
using BenchmarkDotNet.Running;

namespace PortableText
{
    [SimpleJob(RuntimeMoniker.NetCoreApp31)]
    [SimpleJob(RuntimeMoniker.Net50)]
    [SimpleJob(RuntimeMoniker.Net60)]
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
    
    public class Program
    {
        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<Benchmarks>();
        }
    }
}
