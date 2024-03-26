using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Exporters;

namespace Benchmarks;

[MemoryDiagnoser]
[Config(typeof(Config))]
public class ListBenchmarks
{
    class Config : ManualConfig
    {
        public Config()
        {
            AddDiagnoser(new MemoryDiagnoser(new MemoryDiagnoserConfig(true)));
            AddExporter(MarkdownExporter.GitHub);
        }
    }
    
    [Params(10000)]
    public int LoopCount;
    
    public CSLibrary.IList<int> intList;
    
    [Benchmark]
    public void AddBenchmark()
    {
        for (int i = 0; i < LoopCount; i++)
        {
            intList.Add(i);
        }
    }
    
    [Benchmark]
    public void InsertBenchmark()
    {
        for (int i = 0; i < LoopCount; i++)
        {
            intList.Insert(i, 0);
        }
    }
    
    [Benchmark]
    public void RemoveBenchmark()
    {
        for (int i = 0; i < LoopCount; i++)
        {
            intList.RemoveAt(0);
        }
    }
    
    [Benchmark]
    public void AccessBenchmark()
    {
        for (int i = 0; i < LoopCount; i++)
        {
            intList.At(i);
        }
    }
        
    [Benchmark]
    public void SearchBenchmark()
    {
        for (int i = 0; i < LoopCount; i++)
        {
            intList.IndexOf(i);
        }
    }
}