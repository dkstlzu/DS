#nullable enable
using System.Collections;

namespace CSLibrary;

class CoalescedHashTableNode<T> : HashTableNode<T>
{
    public CoalescedHashTableNode<T>? Next;
    public int Index;
}

public class CoalescedHashTable<T> : IHashTable<T> where T : IEquatable<T>
{
    private const int DEFAULT_CAPACITY = 32;
    // https://dl.acm.org/doi/pdf/10.1145/358728.358745 에서 언급하는 일반적인 상황에서 적합할 것으로 기대되는 비율
    private const float CELLAR_RATIO = 0.14f;

    private CoalescedHashTableNode<T>?[] _elements;
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

    private int _cellarNumber;
    private int _occupiedCellarNumber;
    
    public CoalescedHashTable() : this(DEFAULT_CAPACITY)
    {
        
    }
    
    public CoalescedHashTable(int minimumCapacity)
    {
        _elements = new CoalescedHashTableNode<T>[minimumCapacity];
        _cellarNumber = (int)(minimumCapacity * CELLAR_RATIO);
        _minimumCapacity = minimumCapacity;
    }
    
    public int GetHash(object obj)
    {
        int modulo = _elements.Length - _cellarNumber;
        int seed = obj.GetHashCode() * (_elements.GetHashCode() % modulo) % modulo;

        /*
        885919
        252869
        133201
        667531
        491357
        다섯개 숫자는 모두 소수입니다.
        */

        int hash = seed * 885919 % modulo;
        hash = hash * 252869 % modulo;
        hash = hash * 133201 % modulo;
        hash = hash * 667531 % modulo;
        hash = hash * 491357 % modulo;
        
        return hash;
    }
    
    private void Resize()
    {
        int newCapacity = 1;
        while (newCapacity < 3*_size)
        {
            newCapacity <<= 1;
        }

        var previousElementsArr = _elements; 
        _elements = new CoalescedHashTableNode<T>?[Math.Max(newCapacity, _minimumCapacity)];
        _cellarNumber = (int)(_elements.Length * CELLAR_RATIO);
        _occupiedCellarNumber = 0;

        foreach (CoalescedHashTableNode<T>? node in previousElementsArr)
        {
            if (IHashTable<T>.IsValidNode(node))
            {
                TrySet(node!.Key, node.Value);
            }
        }
    }
    
    private void CheckResize()
    {
        if (_elements.Length <= _minimumCapacity && _size < _elements.Length * 0.125) return;
        
        if (_size * 2 + 1 > _elements.Length || _size < _elements.Length * 0.125)
        {
            Resize();
        }
    }

    private void ResizeCellar()
    {
        int newCapacity = _elements.Length * 2;

        var previousElementsArr = _elements; 
        _elements = new CoalescedHashTableNode<T>?[Math.Max(newCapacity, _minimumCapacity)];
        _cellarNumber = (int)(_elements.Length * CELLAR_RATIO);
        _occupiedCellarNumber = 0;

        foreach (CoalescedHashTableNode<T>? node in previousElementsArr)
        {
            if (IHashTable<T>.IsValidNode(node))
            {
                TrySet(node!.Key, node.Value);
            }
        }
    }

    private void CheckResizeCellar()
    {
        if (_occupiedCellarNumber >= _cellarNumber)
        {
            ResizeCellar();
        }
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

        if (_elements[realKey] == null)
        {
            throw new InvalidOperationException();
        }
        
        if (_elements[realKey]!.Key == key)
        {
            return _elements[realKey]!.Value;
        }
        
        var node = _elements[realKey];

        while (node != null)
        {
            if (node.Key == key)
            {
                return node.Value;
            }

            node = node.Next;
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
        
        if (_elements[realKey] == null)
        {
            _elements[realKey] = new CoalescedHashTableNode<T>(){Value = t, Key = key, Index = realKey};
            return true;
        }
        
        if (_elements[realKey]!.Key == key)
        {
            _elements[realKey]!.Value = t;
            return false;
        }
        
        var prevNode = _elements[realKey];
        var node = prevNode!.Next;

        while (node != null)
        {
            if (node.Key == key)
            {
                node.Value = t;
                return false;
            }

            node = node.Next;
        }

        for (int i = _elements.Length - 1; i >= _elements.Length - _cellarNumber; i--)
        {
            if (_elements[i] == null)
            {
                var newNode = new CoalescedHashTableNode<T>() { Value = t, Key = key, Index = i };
                prevNode.Next = newNode;
                _elements[i] = newNode;
                _occupiedCellarNumber++;
                CheckResizeCellar();
                return true;
            }
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

        if (_elements[realKey] == null)
        {
            return false;
        }
        
        var prevNode = _elements[realKey]!;
        var node = prevNode.Next;

        if (prevNode.Key == key)
        {
            if (node != null)
            {
                _elements[node.Index] = null;
                node.Index = prevNode.Index;
            }
            
            _elements[prevNode.Index] = node;
            return true;
        }

        while (node != null)
        {
            if (node.Key == key)
            {
                prevNode.Next = node.Next;
                _elements[node.Index] = null;
                return true;
            }

            node = node.Next;
        }
        
        return false;
    }

    public bool Contains(int key)
    {
        int realKey = GetHash(key);

        if (_elements[realKey] == null)
        {
            return false;
        }
        
        if (_elements[realKey]!.Key == key)
        {
            return true;
        }
        
        var node = _elements[realKey];

        while (node != null)
        {
            if (node.Key == key)
            {
                return true;
            }

            node = node.Next;
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
        private CoalescedHashTable<T> _hashTable;
        private int _index;
        private T? _current;
        
        public Enumerator(CoalescedHashTable<T> hashTable)
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