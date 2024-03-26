using System.Runtime.CompilerServices;

namespace Common;

public static class Utility
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void print(object obj) => Console.Write(obj);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void printl(object obj) => Console.WriteLine(obj);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void printl() => Console.WriteLine();
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void prints(object obj)
    {
        Console.Write(obj);
        Console.Write(" ");
    }
}