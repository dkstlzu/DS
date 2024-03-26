// #define RUN_ARRAY_SAMPLE

#define RUN_FIXEDLIST_BENCHMARK
#define RUN_RESIZABLELIST_BENCHMARK
#define RUN_SINGLE_LINKEDLIST_BENCHMARK
#define RUN_DOUBLE_LINKEDLIST_BENCHMARK
#define RUN_CIRCULARSINGLE_LINKEDLIST_BENCHMARK
#define RUN_CIRCULARDOUBLE_LINKEDLIST_BENCHMARK

// #define RUN_FIXEDSIZE_STACK_BENCHMARK
// #define RUN_RESIZABLE_STACK_BENCHMARK
// #define RUN_LINKEDLIST_STACK_BENCHMARK
//
// #define RUN_FIXEDSIZE_QUEUE_BENCHMARK
// #define RUN_RESIZABLE_QUEUE_BENCHMARK
// #define RUN_LINKEDLIST_QUEUE_BENCHMARK
//
// #define RUN_DIRECTACCESS_HASHTABLE_BENCHMARK
// #define RUN_CUSTOMHASHING_HASHTABLE_BENCHMARK
// #define RUN_CHAINING_HASHTABLE_BENCHMARK
// #define RUN_COALESCED_HASHTABLE_BENCHMARK
// #define RUN_LINEARPROBING_HASHTABLE_BENCHMARK
// #define RUN_QUADRATICPROBING_HASHTABLE_BENCHMARK
// #define RUN_DOUBLEHASHING_HASHTABLE_BENCHMARK
// #define RUN_CUCKOO_HASHTABLE_BENCHMARK
// #define RUN_HOPSCOTCH_HASHTABLE_BENCHMARK
// #define RUN_ROBINHOOD_HASHTABLE_BENCHMARK

// #define RUN_UNMANAGED_FIXEDLIST_BENCHMARK
// #define RUN_UNMANAGED_RESIZABLELIST_BENCHMARK

using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using Benchmarks;
using CSLibrary;

Console.WriteLine("Hello, World!");

#if RUN_ARRAY_SAMPLE
CSLibrary.Array.Sample();
#endif

#if Test
Console.WriteLine("Test hi");
#endif

List<Summary> summariesToLog = new List<Summary>();

#region Managed

#region List

#if RUN_FIXEDLIST_BENCHMARK
summariesToLog.Add(BenchmarkRunner.Run<FixedSizeListBenchmarks>());
#endif

#if RUN_RESIZABLELIST_BENCHMARK
summariesToLog.Add(BenchmarkRunner.Run<ResizableListAddBenchmarks>());
#endif

#if RUN_SINGLE_LINKEDLIST_BENCHMARK
summariesToLog.Add(BenchmarkRunner.Run<SingleLinkedListBenchmarks>());
#endif

#if RUN_DOUBLE_LINKEDLIST_BENCHMARK
summariesToLog.Add(BenchmarkRunner.Run<DoubleLinkedListBenchmarks>());
#endif

#if RUN_CIRCULARSINGLE_LINKEDLIST_BENCHMARK
summariesToLog.Add(BenchmarkRunner.Run<CircularSingleLinkedListBenchmarks>());
#endif

#if RUN_CIRCULARDOUBLE_LINKEDLIST_BENCHMARK
summariesToLog.Add(BenchmarkRunner.Run<CircularDoubleLinkedListBenchmarks>());
#endif

#endregion

#region Stack



#endregion

#region Queue



#endregion

#region Hash Table



#endregion

#region Tree



#endregion

#endregion


#region Unmanaged

#region List

#if RUN_UNMANAGED_FIXEDLIST_BENCHMARK
summariesToLog.Add(BenchmarkRunner.Run<ManualFixedListAddBenchmark>());
summariesToLog.Add(BenchmarkRunner.Run<ManualFixedListBenchmarks>());
summariesToLog.Add(BenchmarkRunner.Run<ManualFixedListDeleteBenchmark>());
#endif

#if RUN_UNMANAGED_RESIZABLELIST_BENCHMARK
summariesToLog.Add(BenchmarkRunner.Run<ManualResizableListAddBenchmark>());
#endif

#endregion

#region Stack



#endregion

#region Queue



#endregion

#region Hash Table



#endregion

#region Tree



#endregion

#endregion


var logger = ConsoleLogger.Default;
foreach (var summary in summariesToLog)
{
    MarkdownExporter.Console.ExportToLog(summary, logger);
}