using System.Collections;

namespace CSLibrary;

public class FixedSizeList<T> : IList<T> where T : IEquatable<T>
{
    private T?[] _elements;
    private int _size;

    public FixedSizeList(int capacity)
    {
        _elements = new T[capacity];
    }

    #region List Interface

    public T? At(int index)
    {
        if (index < 0 || index >= _size)
        {
            throw new IndexOutOfRangeException();
        }
        
        return _elements[index];
    }

    public void Add(T? t)
    {
        Insert(t, _size);
    }

    public void Insert(T? t, int index)
    {
        if (index < 0 || index > _size)
        {
            throw new IndexOutOfRangeException();
        }

        for (int i = _size; i > index; i--)
        {
            _elements[i] = _elements[i - 1];
        }

        _elements[index] = t;

        _size++;
    }

    public void Remove(T t)
    {
        int index = IndexOf(t);

        if (index >= 0)
        {
            RemoveAt(index);
        }
    }

    public void RemoveAt(int index)
    {
        if (index < 0 || index >= _size)
        {
            throw new IndexOutOfRangeException();
        }

        _size--;

        for (int i = index; i < _size; i++)
        {
            _elements[i] = _elements[i + 1];
        }
    }

    public int IndexOf(T t)
    {
        int index = -1;
        for (int i = 0; i < _size; i++)
        {
            if (t.Equals(_elements[i]))
            {
                index = i;
            }
        }
        return index;
    }

    public bool Contains(T t)
    {
        return IndexOf(t) >= 0;
    }

    public void Clear()
    {
        _size = 0;
    }

    public int Count => _size;

    public bool IsEmpty() => Count == 0;
    
    #endregion

    #region IEnumerable

    public IEnumerator<T> GetEnumerator()
    {
        return new Enumerator(this);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable<T>)this).GetEnumerator();
    }

    class Enumerator : IEnumerator<T>
    {
        private FixedSizeList<T> _list;
        private int _index;
        private int _size;
        private T? _current;
        
        public Enumerator(FixedSizeList<T> list)
        {
            _list = list;
            _size = list.Count();
        }
        
        public bool MoveNext()
        {
            if (_index < _size)
            {
                _current = _list.At(_index++);
                return true;
            }

            return false;
        }

        public void Reset()
        {
            _index = 0;
            _current = default;
        }

        public T? Current
        {
#pragma warning disable CS8766 // 반환 형식에서 참조 형식의 null 허용 여부가 암시적으로 구현된 멤버와 일치하지 않음(null 허용 여부 특성 때문일 수 있음)
            get
#pragma warning restore CS8766 // 반환 형식에서 참조 형식의 null 허용 여부가 암시적으로 구현된 멤버와 일치하지 않음(null 허용 여부 특성 때문일 수 있음)
            {
                if (_index == 0 || _index > _size)
                {
                    throw new InvalidOperationException();
                }
                
                return _current;
            }
        }

        object? IEnumerator.Current => Current;

        public void Dispose()
        {
            // Dispose 할거 없음
        }
    }
    
    #endregion
}