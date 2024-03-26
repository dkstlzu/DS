#nullable enable
using System.Collections;

namespace CSLibrary;

public class CuckooHashTable<T> : IHashTable<T> where T : IEquatable<T>
{
    private const int DEFAULT_CAPACITY = 32;
    private const int CUCKOO_LOOP_SIZE = 16;

    private HashTableNode<T>?[] _elements;
    private int _minimumCapacity;
    private int _tableCapacity => _elements.Length / 2;
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

    private int _hashSeed1;
    private int _hashSeed2;
    
    public CuckooHashTable() : this(DEFAULT_CAPACITY)
    {
        
    }
    
    public CuckooHashTable(int minimumCapacity)
    {
        if (minimumCapacity % 2 > 0)
        {
            minimumCapacity++;
        }
        
        _minimumCapacity = minimumCapacity;
        _elements = new HashTableNode<T>[minimumCapacity];
    }
    
    public int Hash1(object obj)
    {
        int seed = obj.GetHashCode() * (_elements.GetHashCode() % _elements.Length) % _elements.Length;
        seed = (seed + _hashSeed1) % _elements.Length;
        
        /*
        885919
        252869
        133201
        667531
        491357
        다섯개 숫자는 모두 소수입니다.
        */

        int hash = seed * 885919 % _tableCapacity;
        hash = hash * 252869 % _tableCapacity;
        hash = hash * 133201 % _tableCapacity;
        hash = hash * 667531 % _tableCapacity;
        hash = hash * 491357 % _tableCapacity;
        
        return hash;
    }
    
    public int Hash2(object obj)
    {
        int seed = obj.GetHashCode() * (_elements.GetHashCode() % _elements.Length) % _elements.Length;
        seed = (seed + _hashSeed2) % _elements.Length;

        /*
        927491
        897191
        755329
        586589
        544001
        다섯개 숫자는 모두 소수입니다.
        */

        int hash = seed * 927491 % _tableCapacity;
        hash = hash * 897191 % _tableCapacity;
        hash = hash * 755329 % _tableCapacity;
        hash = hash * 586589 % _tableCapacity;
        hash = hash * 544001 % _tableCapacity;
        
        return hash + _tableCapacity;
    }

    private void Rehash()
    {
        _hashSeed1 = Random.Shared.Next() % _elements.Length;
        _hashSeed2 = Random.Shared.Next() % _elements.Length;
        Resize();
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
    }

    private void CheckResize()
    {
        if (_elements.Length <= _minimumCapacity && _size < _elements.Length * 0.125) return;
        
        if (_size * 2 + 1 > _elements.Length || _size < _elements.Length * 0.125)
        {
            Resize();
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

    public int Count()
    {
        return _size;
    }

    public bool IsEmpty() => Count() == 0;

    public T? Get(int key)
    {
        int index = Hash1(key);

        if (_elements[index] != null && _elements[index]!.Key == key)
        {
            return _elements[index]!.Value;
        }
        
        index = Hash2(key);
        
        if (_elements[index] != null && _elements[index]!.Key == key)
        {
            return _elements[index]!.Value;
        }

        throw new InvalidOperationException();
    }

    public void Set(int key, T? t)
    {
        if (!SetValueIfContains(key, t) && TrySet(key, t))
        {
            _size++;
        }
    }

    private bool SetValueIfContains(int key, T? t)
    {
        int index = Hash1(key);

        if (_elements[index] != null && _elements[index]!.Key == key)
        {
            _elements[index]!.Value = t;
            return true;
        }
        
        index = Hash2(key);
        
        if (_elements[index] != null && _elements[index]!.Key == key)
        {
            _elements[index]!.Value = t;
            return true;
        }

        return false;
    }

    private bool TrySet(int key, T? t)
    {
        var cuckooNode = new HashTableNode<T>(){Value = t, Key = key};
        for (int i = 0; i < CUCKOO_LOOP_SIZE; i++)
        {
            int index = Hash1(key);
            
            if (_elements[index] == null)
            {
                _elements[index] = cuckooNode;
                return true;
            }

            (_elements[index], cuckooNode) = (cuckooNode, _elements[index]!);

            index = Hash2(cuckooNode.Key);
            
            if (_elements[index] == null)
            {
                _elements[index] = cuckooNode;
                return true;
            }
            
            (_elements[index], cuckooNode) = (cuckooNode, _elements[index]!);
        }

        // Cuckoo 실패시 Rehash
        Rehash();
        return TrySet(cuckooNode.Key, cuckooNode.Value);
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
        int index = Hash1(key);

        if (_elements[index] != null && _elements[index]!.Key == key)
        {
            _elements[index] = null;
            return true;
        }
        
        index = Hash2(key);
        
        if (_elements[index] != null && _elements[index]!.Key == key)
        {
            _elements[index] = null;
            return true;
        }

        return false;
    }

    public bool Contains(int key)
    {
        int index = Hash1(key);

        if (_elements[index] != null && _elements[index]!.Key == key)
        {
            return true;
        }
        
        index = Hash2(key);
        
        if (_elements[index] != null && _elements[index]!.Key == key)
        {
            return true;
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
        private CuckooHashTable<T> _hashTable;
        private int _index;
        private T? _current;
        
        public Enumerator(CuckooHashTable<T> hashTable)
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