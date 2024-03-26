using BenchmarkDotNet.Attributes;
using CSLibrary;

namespace Benchmarks;

public class ResizableQueueBenchmarks : QueueBenchmarks
{
    private const int INITIAL_ELEMENTS = 10;

    [GlobalSetup(Targets = new []{nameof(Enqueue)})]
    public void SetUp()
    {
        intQueue = new ResizableQueue<int>(INITIAL_ELEMENTS);
    }

    [GlobalSetup(Targets = new []{nameof(Peek)})]
    [IterationSetup(Targets = new []{nameof(Dequeue)})]
    public void AddSetUp()
    {
        intQueue = new ResizableQueue<int>(LoopCount);
        for (int i = 0; i < LoopCount; i++)
        {
            intQueue.Enqueue(i);
        }
    }

    [GlobalCleanup]
    [IterationCleanup(Targets = new []{nameof(Enqueue)})]
    public void Clear()
    {
        intQueue.Clear();
    }
}
