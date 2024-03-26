
using BenchmarkDotNet.Attributes;
using CSLibrary;

namespace Benchmarks;

public class LinkedListStackBenchmarks : StackBenchmarks
{
    [GlobalSetup(Targets = new []{nameof(Push)})]
    public void SetUp()
    {
        intStack = new LinkedListBasedStack<int>();
    }

    [GlobalSetup(Targets = new []{nameof(Peek)})]
    [IterationSetup(Targets = new []{nameof(Pop)})]
    public void AddSetUp()
    {
        intStack = new LinkedListBasedStack<int>();
        for (int i = 0; i < LoopCount; i++)
        {
            intStack.Push(i);
        }
    }

    [GlobalCleanup]
    [IterationCleanup(Targets = new []{nameof(Push)})]
    public void Clear()
    {
        intStack.Clear();
    }
}
