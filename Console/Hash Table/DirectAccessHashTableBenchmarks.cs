using BenchmarkDotNet.Attributes;
using CSLibrary;

namespace Benchmarks;

[MemoryDiagnoser]
[RPlotExporter]
public class DirectAccessHashTableBenchmarks
{
    [Params(100000)]
    public int LoopCount;
    
    DirectAccessHashTable<string> stringHashTable;
    
    [Benchmark]
    public void Set()
    {
        for (int i = 0; i < LoopCount; i++)
        {
            stringHashTable.Set(i, "Hash");
        }
    }
    
    [Benchmark]
    public void Get()
    {
        for (int i = 0; i < LoopCount; i++)
        {
            string? value = stringHashTable.Get(i);
        }
    }

    [Benchmark]
    public void Contains()
    {
        for (int i = 0; i < LoopCount; i++)
        {
            bool value = stringHashTable.Contains(i);
        }
    }
    
    [Benchmark]
    public void Remove()
    {
        for (int i = 0; i < LoopCount; i++)
        {
            stringHashTable.Remove(i);
        }
    }
    //
    // [GlobalSetup(Targets = new []{nameof(Set)})]
    // public void SetUp()
    // {
    //     stringHashTable = new DirectAccessHashTable<string>(LoopCount);
    // }
    //
    // [GlobalSetup(Targets = new []{nameof(Remove)})]
    // [IterationSetup(Targets = new []{nameof(Get)})]
    // public void AddSetUp()
    // {
    //     stringHashTable = new DirectAccessHashTable<string>(LoopCount);
    //     for (int i = 0; i < LoopCount; i++)
    //     {
    //         stringHashTable.Enqueue(i);
    //     }
    // }
    //
    // [GlobalCleanup]
    // [IterationCleanup(Targets = new []{nameof(Set)})]
    // public void Clear()
    // {
    //     stringHashTable.Clear();
    // }
}
