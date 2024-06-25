#nullable enable
using System.Collections;

namespace CSLibrary;

/// <summary>
/// Hash 값의 Collision을 처리하지 않는 단순한 형태입니다.
/// </summary>
public class CustomHashingHashTable<T> : IHashTable<T> where T : IEquatable<T>
{
    private const int DEFAULT_CAPACITY = 32;
    
    private T?[] _elements;
    private bool[] _setted;
    private int _size;

    public CustomHashingHashTable() : this(DEFAULT_CAPACITY)
    {
        
    }
    
    public CustomHashingHashTable(int capacity)
    {
        _elements = new T[capacity];
        _setted = new bool[_elements.Length];
    }
    
    public int GetHash(object obj)
    {
        int seed = obj.GetHashCode() * (_elements.GetHashCode() % _elements.Length) % _elements.Length;

        /*
        117269
        946579
        180773
        165541
        118583
        다섯개 숫자는 모두 소수입니다.
        */

        int hash = seed * 117269 % _elements.Length;
        hash = hash * 946579 % _elements.Length;
        hash = hash * 180773 % _elements.Length;
        hash = hash * 165541 % _elements.Length;
        hash = hash * 118583 % _elements.Length;
        
        return hash;
    }
    
    #region HashTable Interface

    public void Clear()
    {
        for (int i = 0; i < _elements.Length; i++)
        {
            _elements[i] = default;
            _setted[i] = false;
        }

        _size = 0;
    }

    public int Count => _size;
    public bool IsEmpty() => Count == 0;

    public T? Get(int key)
    {
        int realKey = GetHash(key);
        
        if (!_setted[realKey])
        {
            throw new InvalidOperationException();
        }
        
        return _elements[realKey];
    }

    public void Set(int key, T? t)
    {
        int realKey = GetHash(key);
        
        if (!_setted[realKey])
        {
            _setted[realKey] = true;
            _size++;
        }
        
        _elements[realKey] = t;
    }

    public void Remove(int key)
    {
        int realKey = GetHash(key);

        _setted[realKey] = false;
        _size--;
    }

    public bool Contains(int key)
    {
        int realKey = GetHash(key);

        return _setted[realKey];
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
        private CustomHashingHashTable<T> _hashTable;
        private int _index;
        private T? _current;
        
        public Enumerator(CustomHashingHashTable<T> hashTable)
        {
            _hashTable = hashTable;
        }

        public bool MoveNext()
        {
            if (_index < _hashTable._elements.Length)
            {
                _current = _hashTable._elements[_index++];
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