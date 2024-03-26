
using BenchmarkDotNet.Attributes;
using CSLibrary;

namespace Benchmarks;

public class LinkedListQueueBenchmarks : QueueBenchmarks
{
    [GlobalSetup(Targets = new []{nameof(Enqueue)})]
    public void SetUp()
    {
        intQueue = new LinkedListBasedQueue<int>();
    }

    [GlobalSetup(Targets = new []{nameof(Peek)})]
    [IterationSetup(Targets = new []{nameof(Dequeue)})]
    public void AddSetUp()
    {
        intQueue = new LinkedListBasedQueue<int>();
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
