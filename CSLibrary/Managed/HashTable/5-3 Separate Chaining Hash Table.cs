#nullable enable
using System.Collections;

namespace CSLibrary;

class ChainingHashTableNode<T> : HashTableNode<T>
{
    public ChainingHashTableNode<T>? Next;
}

public class ChainingHashTable<T> : IHashTable<T> where T : IEquatable<T>
{
    private const int DEFAULT_CAPACITY = 32;

    private ChainingHashTableNode<T>?[] _elements;
    private int _size;
    
    public ChainingHashTable() : this(DEFAULT_CAPACITY)
    {
        
    }
    
    public ChainingHashTable(int capacity)
    {
        _elements = new ChainingHashTableNode<T>[capacity];
    }
    
    public int GetHash(object obj)
    {
        int seed = obj.GetHashCode() * (_elements.GetHashCode() % _elements.Length) % _elements.Length;

        /*
        885919
        252869
        133201
        667531
        491357
        다섯개 숫자는 모두 소수입니다.
        */

        int hash = seed * 885919 % _elements.Length;
        hash = hash * 252869 % _elements.Length;
        hash = hash * 133201 % _elements.Length;
        hash = hash * 667531 % _elements.Length;
        hash = hash * 491357 % _elements.Length;
        
        return hash;
    }
    
    #region HashTable Interface

    public void Clear()
    {
        for (int i = 0; i < _elements.Length; i++)
        {
            _elements[i] = default;
        }

        _size = 0;
    }

    public int Count => _size;
    public bool IsEmpty() => Count == 0;

    public T? Get(int key)
    {
        int realKey = GetHash(key);
        
        if (_elements[realKey] != null)
        {
            var node = _elements[realKey];

            while (node != null)
            {
                if (node.Key == key)
                {
                    return node.Value;
                }

                node = node.Next;
            }
        }
        
        throw new InvalidOperationException();
    }

    public void Set(int key, T? t)
    {
        int realKey = GetHash(key);

        if (_elements[realKey] == null)
        {
            _elements[realKey] = new ChainingHashTableNode<T>(){Value = t, Key = key};
            _size++;
            return;
        }
        
        var targetNode = _elements[realKey]!;

        if (targetNode.Key == key)
        {
            targetNode.Value = t;
            return;
        }
        
        var prevNode = targetNode;
        var node = prevNode.Next;
        while (node != null)
        {
            if (node.Key == key)
            {
                node.Value = t;
                return;
            }

            node = node.Next;
        }

        prevNode.Next = new ChainingHashTableNode<T>() { Value = t, Key = key };
        _size++;
    }

    public void Remove(int key)
    {
        int realKey = GetHash(key);

        if (_elements[realKey] == null)
        {
            throw new InvalidOperationException();
        }
        
        if (_elements[realKey]!.Key == key)
        {
            _elements[realKey] = _elements[realKey]!.Next;
            _size--;
            return;
        }
        
        var prevNode = _elements[realKey]!;
        var node = prevNode.Next;

        while (node != null)
        {
            if (node.Key == key)
            {
                prevNode.Next = node.Next;
                _size--;
                return;
            }

            node = node.Next;
        }

        throw new InvalidOperationException();
    }

    public bool Contains(int key)
    {
        int realKey = GetHash(key);
        
        if (_elements[realKey] != null)
        {
            var node = _elements[realKey]!;

            while (node != null)
            {
                if (node.Key == key)
                {
                    return true;
                }

                node = node.Next;
            }
        }

        return false;
    }

    #endregion
    
    #region IEnumerable

    public IEnumerator<T> GetEnumerator()
    {
        return new Enumerator(this);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    class Enumerator : IEnumerator<T>
    {
        private ChainingHashTable<T> _hashTable;
        private ChainingHashTableNode<T>? _node;
        private int _index;
        
        public Enumerator(ChainingHashTable<T> hashTable)
        {
            _hashTable = hashTable;
        }

        public bool MoveNext()
        {
            if (_index >= _hashTable._elements.Length)
            {
                return false;
            }
            
            if (_node == null || _node.Next == null)
            {
                for (int i = _index; i < _hashTable._elements.Length; i++)
                {
                    if (_hashTable._elements[i] != null)
                    {
                        _node = _hashTable._elements[i];
                        _index = i+1;
                        return true;
                    }
                }
            } else
            {
                _node = _node!.Next;
                return true;
            }

            _index = _hashTable._elements.Length;
            return false;
        }

        public void Reset()
        {
            _node = null;
            _index = 0;
        }

#pragma warning disable CS8766 // 반환 형식에서 참조 형식의 null 허용 여부가 암시적으로 구현된 멤버와 일치하지 않음(null 허용 여부 특성 때문일 수 있음)
        public T? Current => _node!.Value;
#pragma warning restore CS8766 // 반환 형식에서 참조 형식의 null 허용 여부가 암시적으로 구현된 멤버와 일치하지 않음(null 허용 여부 특성 때문일 수 있음)

        object? IEnumerator.Current => Current;
        
        public void Dispose()
        {
            // 할거 없음
        }
    }

    #endregion
}