namespace CSLibrary;

public class FixedSizeQueue<T> : IQueue<T> where T : IEquatable<T>
{
    private T?[] _elements;
    private int _size;
    private int _headIndex;

    public FixedSizeQueue(int capacity)
    {
        _elements = new T[capacity];
    }

    private int AddIndex(int a, int b)
    {
        return (a + b) % _elements.Length;
    }
    
    #region Queue Interface

    public void Clear()
    {
        _size = 0;
    }

    public int Count()
    {
        return _size;
    }

    public bool IsEmpty() => Count() == 0;

    public void Enqueue(T? t)
    {
        if (_size >= _elements.Length)
        {
            throw new InvalidOperationException();
        }
        
        _elements[AddIndex(_headIndex, _size)] = t;
        _size++;
    }

    public T? Dequeue()
    {
        if (IsEmpty())
        {
            throw new InvalidOperationException();
        }

        int headIndex = _headIndex;
        _headIndex = AddIndex(_headIndex, 1);
        _size--;
        return _elements[headIndex];
    }

    public T? Peek()
    {
        if (IsEmpty())
        {
            throw new InvalidOperationException();
        }
        
        return _elements[_headIndex];
    }

    #endregion

}