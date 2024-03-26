using BenchmarkDotNet.Attributes;
using CSLibrary;

namespace Benchmarks;

[MemoryDiagnoser]
public class StackBenchmarks
{
    [Params(10000)]
    public int LoopCount;
    
    public CSLibrary.IStack<int> intStack;
    
    [Benchmark]
    public void Push()
    {
        for (int i = 0; i < LoopCount; i++)
        {
            intStack.Push(i);
        }
    }
    
    [Benchmark]
    public void Pop()
    {
        for (int i = 0; i < LoopCount; i++)
        {
            int value = intStack.Pop();
        }
    }

    [Benchmark]
    public void Peek()
    {
        for (int i = 0; i < LoopCount; i++)
        {
            int value = intStack.Peek();
        }
    }

}