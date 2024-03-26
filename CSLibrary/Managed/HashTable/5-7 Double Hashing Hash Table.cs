#nullable enable
using System.Collections;

namespace CSLibrary;

public class DoubleHashingHashTable<T> : IHashTable<T> where T : IEquatable<T>
{
    private const int DEFAULT_CAPACITY = 32;
    private const float LAZY_DELETION_THRESHOLD_RATIO = 0.2f;

    private HashTableNode<T>?[] _elements;
    private int _minimumCapacity;
    private int m_size;
    private int _size
    {
        get => m_size;
        set
        {
            m_size = value;
            CheckResize();
        }
    }

    private int _deletedNodeNumber;

    public DoubleHashingHashTable() : this(DEFAULT_CAPACITY)
    {
        
    }
    
    public DoubleHashingHashTable(int minimumCapacity)
    {
        _elements = new HashTableNode<T>[minimumCapacity];
        _minimumCapacity = minimumCapacity;
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

    private int SecondHash(int firstHash, object obj)
    {
        int seed = obj.GetHashCode() * (_elements.GetHashCode() % _elements.Length) % _elements.Length;

        /*
        862559
        564257
        253537
        826549
        535099
        다섯개 숫자는 모두 소수입니다.
        */

        int hash = seed * 862559 % _elements.Length;
        hash = hash * 564257 % _elements.Length;
        hash = hash * 253537 % _elements.Length;
        hash = hash * 826549 % _elements.Length;
        hash = hash * 535099 % (_elements.Length - 1) + 1;
        
        return (firstHash + hash) % _elements.Length;
    }
    
    private void Resize()
    {
        int newCapacity = 1;
        while (newCapacity < 3*_size)
        {
            newCapacity <<= 1;
        }

        var previousElementsArr = _elements; 
        _elements = new HashTableNode<T>?[Math.Max(newCapacity, _minimumCapacity)];

        foreach (HashTableNode<T>? node in previousElementsArr)
        {
            if (IHashTable<T>.IsValidNode(node))
            {
                TrySet(node!.Key, node.Value);
            }
        }
        
        _deletedNodeNumber = 0;
    }

    private void CheckResize()
    {
        if (_elements.Length <= _minimumCapacity && _size < _elements.Length * 0.125) return;
        
        if (_size * 2 + 1 > _elements.Length || _size < _elements.Length * 0.125)
        {
            Resize();
        }
    }
        
    private void CleanDeletedNodes()
    {
        if (_deletedNodeNumber > _elements.Length * LAZY_DELETION_THRESHOLD_RATIO)
        {
            Resize();
        }
    }
    
    private HashTableNode<T>? Find(int probingStartIndex, int key, out int lastProbedIndex)
    {
        int probingIndex = probingStartIndex;
        for (int i = 0; i < _elements.Length; i++, probingIndex = SecondHash(probingIndex, key))
        {
            if (_elements[probingIndex] == IHashTable<T>.Deleted) continue;
            if (_elements[probingIndex] == null)
            {
                lastProbedIndex = probingIndex;
                return null;
            }
            
            if (_elements[probingIndex]!.Key == key)
            {
                lastProbedIndex = probingIndex;
                return _elements[probingIndex]!;
            }
        }

        lastProbedIndex = -1;
        return null;
    }
    
    #region HashTable Interface

    public void Clear()
    {
        for (int i = 0; i < _elements.Length; i++)
        {
            _elements[i] = default;
        }

        _size = 0;
        _deletedNodeNumber = 0;
    }

    public int Count()
    {
        return _size;
    }

    public bool IsEmpty() => Count() == 0;

    public T? Get(int key)
    {
        int realKey = GetHash(key);

        var targetNode = Find(realKey, key, out _);

        if (targetNode != null)
        {
            return targetNode.Value;
        }
        
        throw new InvalidOperationException();
    }

    public void Set(int key, T? t)
    {
        if (TrySet(key, t))
        {
            _size++;
        }
    }

    private bool TrySet(int key, T? t)
    {
        int realKey = GetHash(key);
        
        if (Find(realKey, key, out int lastProbedIndex) != null)
        {
            _elements[lastProbedIndex]!.Value = t;
            return false;
        }

        if (lastProbedIndex >= 0)
        {
            _elements[lastProbedIndex] = new HashTableNode<T>() { Value = t, Key = key };
            return true;
        }

        throw new InvalidOperationException();
    }

    public void Remove(int key)
    {
        if (!TryRemove(key))
        {
            throw new InvalidOperationException();
        }
        
        _size--;
    }

    private bool TryRemove(int key)
    {
        int realKey = GetHash(key);

        if (Find(realKey, key, out int foundIndex) != null)
        {
            _elements[foundIndex] = IHashTable<T>.Deleted;
            _deletedNodeNumber++;
            CleanDeletedNodes();
            return true;
        }

        return false;
    }

    public bool Contains(int key)
    {
        int realKey = GetHash(key);

        return null != Find(realKey, key, out _);
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
        private DoubleHashingHashTable<T> _hashTable;
        private int _index;
        private T? _current;
        
        public Enumerator(DoubleHashingHashTable<T> hashTable)
        {
            _hashTable = hashTable;
        }

        public bool MoveNext()
        {
            if (_index < _hashTable._elements.Length)
            {
                if (IHashTable<T>.IsValidNode(_hashTable._elements[_index]))
                {
                    _current = _hashTable._elements[_index]!.Value;
                }
                else
                {
                    _current = default;
                }
                _index++;
                return true;
            }

            return false;
        }

        public void Reset()
        {
            _index = 0;
        }

#pragma warning disable CS8766 // 반환 형식에서 참조 형식의 null 허용 여부가 암시적으로 구현된 멤버와 일치하지 않음(null 허용 여부 특성 때문일 수 있음)
        public T? Current => _current;
#pragma warning restore CS8766 // 반환 형식에서 참조 형식의 null 허용 여부가 암시적으로 구현된 멤버와 일치하지 않음(null 허용 여부 특성 때문일 수 있음)

        object? IEnumerator.Current => Current;
        
        public void Dispose()
        {
            // 할거 없음
        }
    }

    #endregion
}