#nullable enable
namespace CSLibrary;

public interface IStack<T> : ICollection where T : IEquatable<T>
{
    T? Peek();
    T? Pop();
    void Push(T? t);
}