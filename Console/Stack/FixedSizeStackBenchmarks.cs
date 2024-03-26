using BenchmarkDotNet.Attributes;
using CSLibrary;

namespace Benchmarks;

public class FixedSizeStackBenchmarks : StackBenchmarks
{
    [GlobalSetup(Targets = new []{nameof(Push)})]
    public void SetUp()
    {
        intStack = new FixedSizeStack<int>(LoopCount);
    }

    [GlobalSetup(Targets = new []{nameof(Peek)})]
    [IterationSetup(Targets = new []{nameof(Pop)})]
    public void AddSetUp()
    {
        intStack = new FixedSizeStack<int>(LoopCount);
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
