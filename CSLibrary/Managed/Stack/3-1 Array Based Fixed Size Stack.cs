namespace CSLibrary;

public class FixedSizeStack<T> : IStack<T> where T : IEquatable<T>
{
    private T?[] _elements;
    private int _top = -1;
    private int _size => _top+1;

    public FixedSizeStack(int capacity)
    {
        _elements = new T[capacity];
    }
    
    #region Stack Interface

    public void Clear()
    {
        _top = -1;
    }

    public int Count()
    {
        return _size;
    }

    public bool IsEmpty() => Count() == 0;

    public T? Peek()
    {
        if (_top < 0)
        {
            throw new InvalidOperationException();
        }
        
        return _elements[_top];
    }

    public T? Pop()
    {
        if (_top < 0)
        {
            throw new InvalidOperationException();
        }

        return _elements[_top--];
    }

    public void Push(T? t)
    {
        if (_size >= _elements.Length)
        {
            throw new InvalidOperationException();
        }

        _elements[++_top] = t;
    }

    #endregion
}