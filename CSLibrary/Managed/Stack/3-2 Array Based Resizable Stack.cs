namespace CSLibrary;

public class ResizableStack<T> : IStack<T> where T : IEquatable<T>
{
    private T?[] _elements;
    private int _top = -1;
    private int _size => _top+1;

    public ResizableStack(int capacity)
    {
        _elements = new T[capacity];
    }
    
    public void Resize(int newCapacity)
    {
        if (_elements.Length == newCapacity)
        {
            return;
        }

        int realNewCapacity = 1;
        while (realNewCapacity < newCapacity)
        {
            realNewCapacity <<= 1;
        }
        
        var previousElementsArr = _elements;
        _elements = new T[realNewCapacity];
        var newSize = Math.Min(_size, realNewCapacity);
        System.Array.Copy(previousElementsArr, _elements, newSize);
    }
    
    #region Stack Interface

    public void Clear()
    {
        _top = -1;
    }

    public int Count => _size;
    public bool IsEmpty() => Count == 0;

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

        if (_size < _elements.Length / 4)
        {
            Resize(_size/2);
        }
        
        return _elements[_top--];
    }

    public void Push(T? t)
    {
        if (_size >= _elements.Length)
        {
            Resize(Count * 2);
        }
        
        _elements[++_top] = t;
    }

    #endregion

}