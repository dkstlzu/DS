using System.Collections;
using System.Runtime.InteropServices;

namespace CSLibrary.Unmanaged;

public unsafe class ManualFixedList<T> : IList<T>, IDisposable where T : IEquatable<T>
{
    private T* _elements;
    private int _size;
    private bool _disposed;

    public ManualFixedList(uint capacity)
    {
        var allocSize = new UIntPtr(capacity * (uint)sizeof(T));
        _elements = (T*)NativeMemory.Alloc(allocSize);
    }

    ~ManualFixedList()
    {
        if (!_disposed) Dispose();
    }
    
    public void Dispose()
    {
        NativeMemory.Free(_elements);
        _disposed = true;
    }

    #region List Interface

    public T At(int index)
    {
        if (index < 0 || index >= _size)
        {
            throw new IndexOutOfRangeException();
        }
        
        return _elements[index];
    }

    public void Add(T t)
    {
        Insert(t, _size);
    }

    public void Insert(T t, int index)
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

    public int Count()
    {
        return _size;
    }

    public bool IsEmpty() => Count() == 0;

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
        private ManualFixedList<T> _list;
        private int _index;
        private int _size;
        private T _current;
        
        public Enumerator(ManualFixedList<T> list)
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

        public T Current
        {
            get
            {
                if (_index == 0 || _index > _size)
                {
                    throw new InvalidOperationException();
                }
                
                return _current;
            }
        }

        object IEnumerator.Current => Current;

        public void Dispose()
        {
            // Dispose 할거 없음
        }
    }
    
    #endregion
}        
