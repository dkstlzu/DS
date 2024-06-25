#nullable enable
using System.Collections;

namespace CSLibrary;

class RobinHoodHashTableNode<T> : HashTableNode<T>
{
    public int ProbingDistance;
}

public class RobinHoodHashTable<T> : IHashTable<T> where T : IEquatable<T>
{
    private const int DEFAULT_CAPACITY = 32;

    private RobinHoodHashTableNode<T>?[] _elements;
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
    
    public RobinHoodHashTable() : this(DEFAULT_CAPACITY)
    {
        
    }
    
    public RobinHoodHashTable(int minimumCapacity)
    {
        _elements = new RobinHoodHashTableNode<T>[minimumCapacity];
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
    
    private void Resize()
    {
        int newCapacity = 1;
        while (newCapacity < 3*_size)
        {
            newCapacity <<= 1;
        }

        var previousElementsArr = _elements; 
        _elements = new RobinHoodHashTableNode<T>?[Math.Max(newCapacity, _minimumCapacity)];

        foreach (RobinHoodHashTableNode<T>? node in previousElementsArr)
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

    private void RobinHoodArrange(int index)
    {
        if (_elements[index] != null) return;

        int probingIndex = index == _elements.Length-1 ? 0 : index + 1;
        for (int i = 1; i < _elements.Length; i++, probingIndex = probingIndex == _elements.Length-1 ? 0 : probingIndex + 1)
        {
            if (_elements[probingIndex] == null) break;

            if (index + _elements[probingIndex]!.ProbingDistance >= i)
            {
                _elements[probingIndex]!.ProbingDistance -= i - index;
                _elements[index] = _elements[probingIndex];
                _elements[probingIndex] = null;
                RobinHoodArrange(probingIndex);
                return;
            }
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

        int probingIndex = realKey;
        for (int i = 0; i < _elements.Length; i++, probingIndex = probingIndex == _elements.Length-1 ? 0 : probingIndex + 1)
        {
            if (_elements[probingIndex] == null) break;

            if (_elements[probingIndex]!.Key == key)
            {
                return _elements[probingIndex]!.Value;
            }
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

        int probingIndex = realKey;
        for (int i = 0; i < _elements.Length; i++, probingIndex = probingIndex == _elements.Length-1 ? 0 : probingIndex + 1)
        {
            if (_elements[probingIndex] == null)
            {
                _elements[probingIndex] = new RobinHoodHashTableNode<T>() { Value = t, Key = key, ProbingDistance = probingIndex - realKey };
                return true;
            }
            
            if (_elements[probingIndex]!.Key == key)
            {
                _elements[probingIndex]!.Value = t;
                return false;
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

        int probingIndex = realKey;
        for (int i = 0; i < _elements.Length; i++, probingIndex = probingIndex == _elements.Length-1 ? 0 : probingIndex + 1)
        {
            if (_elements[probingIndex] == null) break;

            if (_elements[probingIndex]!.Key == key)
            {
                _elements[probingIndex] = null;

                RobinHoodArrange(probingIndex);
                return true;
            }
        }

        return false;
    }

    public bool Contains(int key)
    {
        int realKey = GetHash(key);

        int probingIndex = realKey;
        for (int i = 0; i < _elements.Length; i++, probingIndex = probingIndex == _elements.Length-1 ? 0 : probingIndex + 1)
        {
            if (_elements[probingIndex] == null) break;
            
            if (_elements[probingIndex]!.Key == key)
            {
                return true;
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
        private RobinHoodHashTable<T> _hashTable;
        private int _index;
        private T? _current;
        
        public Enumerator(RobinHoodHashTable<T> hashTable)
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