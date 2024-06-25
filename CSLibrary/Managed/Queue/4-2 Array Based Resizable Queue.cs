namespace CSLibrary;

public class ResizableQueue<T> : IQueue<T> where T : IEquatable<T>
{
    private T?[] _elements;
    private int _size;
    private int _headIndex;

    public ResizableQueue(int capacity)
    {
        _elements = new T[capacity];
    }

    public void Resize(int newCapacity)
    {
        if (_elements.Length == newCapacity)
        {
            return;
        }
        
        var previousElementsArr = _elements;
        int tailIndex = AddIndex(_headIndex, _size);

        _elements = new T[newCapacity];
    
        int equalOrRightFragmentSize = (tailIndex >= _headIndex) ? tailIndex - _headIndex : previousElementsArr.Length - _headIndex;
        int leftFragmentSize = (tailIndex < _headIndex) ? tailIndex + 1 : 0;

        int rightFragmentCopyLength = Math.Min(newCapacity, equalOrRightFragmentSize);
        int remainedCapacity = newCapacity - equalOrRightFragmentSize < 0 ? 0 : newCapacity - equalOrRightFragmentSize;
        int leftFragmentCopyLength = Math.Min(remainedCapacity, leftFragmentSize);
    
        System.Array.Copy(previousElementsArr, _headIndex, _elements, 0, rightFragmentCopyLength);
        System.Array.Copy(previousElementsArr, 0, _elements, equalOrRightFragmentSize, leftFragmentCopyLength);
        _headIndex = 0;
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

    public int Count => _size;
    public bool IsEmpty() => Count == 0;

    public void Enqueue(T? t)
    {
        if (_size >= _elements.Length)
        {
            Resize(_elements.Length * 2);
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