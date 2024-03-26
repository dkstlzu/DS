#nullable enable
namespace CSLibrary;

public interface IList<T> : ICollection, IEnumerable<T> where T : IEquatable<T>
{
    T? At(int index);
    void Add(T? t);
    void Insert(T? t, int index);
    void Remove(T t);
    void RemoveAt(int index);
    int IndexOf(T t);
    bool Contains(T t);
}