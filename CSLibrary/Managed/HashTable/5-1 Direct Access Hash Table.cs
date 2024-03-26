#nullable enable
using System.Collections;

namespace CSLibrary;

/// <summary>
/// Hash 값의 Collision을 처리하지 않는 단순한 형태입니다.
/// </summary>
public class DirectAccessHashTable<T> : IHashTable<T>
{
    private T?[] _elements;
    private bool[] _setted;
    private int _lowerKeyBound;
    private int _upperKeyBound;
    private int _size;

    public DirectAccessHashTable(int lowerKeyBound, int upperKeyBound)
    {
        if (lowerKeyBound > upperKeyBound)
        {
            throw new InvalidOperationException();
        }
        
        _lowerKeyBound = lowerKeyBound;
        _upperKeyBound = upperKeyBound;

        _elements = new T[upperKeyBound - lowerKeyBound + 1];
        _setted = new bool[_elements.Length];
    }

    private void ValidateKey(int key)
    {
        if (key < _lowerKeyBound || key > _upperKeyBound)
        {
            throw new IndexOutOfRangeException();
        }
    }

    private int GetIndex(int key)
    {
        ValidateKey(key);

        return key - _lowerKeyBound;
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

    public int Count()
    {
        return _size;
    }

    public bool IsEmpty() => Count() == 0;

    public T? Get(int key)
    {
        int realKey = GetIndex(key);
        
        if (!_setted[realKey])
        {
            throw new InvalidOperationException();
        }
        
        return _elements[realKey];
    }

    public void Set(int key, T? t)
    {
        int realKey = GetIndex(key);
        
        if (!_setted[realKey])
        {
            _setted[realKey] = true;
            _size++;
        }
        
        _elements[realKey] = t;
    }

    public void Remove(int key)
    {
        int realKey = GetIndex(key);

        _setted[realKey] = false;
        _size--;
    }

    public bool Contains(int key)
    {
        int realKey = GetIndex(key);

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
        private DirectAccessHashTable<T> _hashTable;
        private int _index;
        private T? _current;
        
        public Enumerator(DirectAccessHashTable<T> hashTable)
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