using BenchmarkDotNet.Attributes;
using CSLibrary;

namespace Benchmarks;

public class SingleLinkedListBenchmarks : ListBenchmarks
{
    [GlobalSetup(Targets = new []{nameof(AddBenchmark), nameof(InsertBenchmark), nameof(RemoveBenchmark)})]
    public void SetUp()
    {
        intList = new SingleLinkedList<int>();
    }

    [GlobalSetup(Targets = new []{nameof(AccessBenchmark), nameof(SearchBenchmark)})]
    [IterationSetup(Targets = new []{nameof(RemoveBenchmark)})]
    public void AddSetUp()
    {
        intList = new SingleLinkedList<int>();
        for (int i = 0; i < LoopCount; i++)
        {
            intList.Add(i);
        }
    }

    [GlobalCleanup]
    [IterationCleanup(Targets = new []{nameof(AddBenchmark), nameof(InsertBenchmark)})]
    public void Clear()
    {
        intList.Clear();
    }
}