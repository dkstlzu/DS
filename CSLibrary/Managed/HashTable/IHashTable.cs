#nullable enable
namespace CSLibrary;

internal class HashTableNode<T> : IEquatable<HashTableNode<T>>
{
    public T? Value;
    public int Key;
    
    public bool Equals(HashTableNode<T>? other)
    {
        if (other == null) return false;
        if (ReferenceEquals(this, other)) return true;
        if (GetType() != other.GetType()) return false;
        
        if (Key == other.Key) return true;
        return false;
    }
}

public interface IHashTable<T> : ICollection, IEnumerable<T>
{
    T? Get(int key);
    void Set(int key, T? t);
    void Remove(int key);
    bool Contains(int key);

    internal static HashTableNode<T> Deleted = new HashTableNode<T>();

    internal static bool IsValidNode(HashTableNode<T>? node) => node != null && node != Deleted;
}