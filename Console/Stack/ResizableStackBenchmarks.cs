using BenchmarkDotNet.Attributes;
using CSLibrary;

namespace Benchmarks;

public class ResizableStackBenchmarks : StackBenchmarks
{
    private const int INITIAL_ELEMENTS = 10;

    [GlobalSetup(Targets = new []{nameof(Push)})]
    public void SetUp()
    {
        intStack = new ResizableStack<int>(INITIAL_ELEMENTS);
    }

    [GlobalSetup(Targets = new []{nameof(Peek)})]
    [IterationSetup(Targets = new []{nameof(Pop)})]
    public void AddSetUp()
    {
        intStack = new ResizableStack<int>(LoopCount);
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
