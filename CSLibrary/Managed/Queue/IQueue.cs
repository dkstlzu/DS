#nullable enable
namespace CSLibrary;

public interface IQueue<T> : ICollection where T : IEquatable<T>
{
    void Enqueue(T? t);
    T? Dequeue();
    T? Peek();
}