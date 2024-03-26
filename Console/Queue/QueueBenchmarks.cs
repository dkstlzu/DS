using BenchmarkDotNet.Attributes;
using CSLibrary;

namespace Benchmarks;

[MemoryDiagnoser]
public class QueueBenchmarks
{
    [Params(10000)]
    public int LoopCount;
    
    public CSLibrary.IQueue<int> intQueue;
    
    [Benchmark]
    public void Enqueue()
    {
        for (int i = 0; i < LoopCount; i++)
        {
            intQueue.Enqueue(i);
        }
    }
    
    [Benchmark]
    public void Dequeue()
    {
        for (int i = 0; i < LoopCount; i++)
        {
            int value = intQueue.Dequeue();
        }
    }

    [Benchmark]
    public void Peek()
    {
        for (int i = 0; i < LoopCount; i++)
        {
            int value = intQueue.Peek();
        }
    }
}