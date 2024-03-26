using BenchmarkDotNet.Attributes;
using CSLibrary.Unmanaged;

namespace Benchmarks;

[MemoryDiagnoser]
public class ManualFixedListAddBenchmark
{
    ManualFixedList<int> intList = new ManualFixedList<int>(ManualFixedListBenchmarks.ELEMENT_CAPACITY);

    [Benchmark]
    public void AddManualFixedList()
    {
        for (int i = 0; i < ManualFixedListBenchmarks.LOOP_COUNT; i++)
        {
            intList.Add(i);
        }
    }
    
    [Benchmark]
    public void InsertManualFixedList()
    {
        for (int i = 0; i < ManualFixedListBenchmarks.LOOP_COUNT; i++)
        {
            intList.Insert(i, 0);
        }
    }
    
    [IterationCleanup]
    public void Clear()
    {
        intList.Clear();
    }
}

[MemoryDiagnoser]
public class ManualFixedListBenchmarks
{
    public const int ELEMENT_CAPACITY = 100000;
    public const int LOOP_COUNT = 100000;
    
    ManualFixedList<int> intList = new ManualFixedList<int>(ELEMENT_CAPACITY);
    
    [GlobalSetup]
    public void AddManualFixedList()
    {
        for (int i = 0; i < LOOP_COUNT; i++)
        {
            intList.Insert(i, 0);
        }
    }
    
    [Benchmark]
    public void AccessManualFixedList()
    {
        for (int i = 0; i < LOOP_COUNT; i++)
        {
            int value = intList.At(i);
        }
    }
    
    [Benchmark]
    public void SearchManualFixedList()
    {
        for (int i = 0; i < LOOP_COUNT; i++)
        {
            int index = intList.IndexOf(i);
        }
    }
}

[MemoryDiagnoser]
public class ManualFixedListDeleteBenchmark
{
    ManualFixedList<int> intList = new ManualFixedList<int>(ManualFixedListBenchmarks.ELEMENT_CAPACITY);

    [Benchmark]
    public void RemoveManualFixedList()
    {
        for (int i = 0; i < ManualFixedListBenchmarks.LOOP_COUNT; i++)
        {
            intList.RemoveAt(0);
        }
    }
    
    [IterationSetup]
    public void AddManualFixedList()
    {
        for (int i = 0; i < ManualFixedListBenchmarks.LOOP_COUNT; i++)
        {
            intList.Add(i);
        }
    }
}