using BenchmarkDotNet.Attributes;
using CSLibrary.Unmanaged;

namespace Benchmarks;

[MemoryDiagnoser]
public class ManualResizableListAddBenchmark
{
    public const int INITIAL_CAPACITY = 100;
    public const int LOOP_COUNT = 10000;
    
    ManualResizableList<int> intList = new ManualResizableList<int>(INITIAL_CAPACITY);

    [Benchmark]
    public void ResizeManualResizableList()
    {
        for (int i = INITIAL_CAPACITY; i < LOOP_COUNT; i++)
        {
            intList.Resize(i);
        }
    }
}