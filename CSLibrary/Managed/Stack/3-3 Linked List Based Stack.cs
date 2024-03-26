namespace CSLibrary;

public class LinkedListBasedStack<T> : IStack<T> where T : IEquatable<T>
{
    private SingleLinkedListNode<T>? _topNode;
    private int _size;

    #region Stack Interface

    public void Clear()
    {
        _topNode = null;
        _size = 0;
    }

    public int Count()
    {
        return _size;
    }

    public T? Peek()
    {
        if (IsEmpty())
        {
            throw new InvalidOperationException();
        }
        
        return _topNode!.Value;
    }

    public T? Pop()
    {
        if (IsEmpty())
        {
            throw new InvalidOperationException();
        }

        var value = _topNode!.Value;
        _topNode = _topNode.Next;
        _size--;

        return value;
    }

    public void Push(T? t)
    {
        var newTopNode = new SingleLinkedListNode<T>() { Value = t };
        newTopNode.Next = _topNode;
        _topNode = newTopNode;
        _size++;
    }

    public bool IsEmpty() => _topNode == null;

    #endregion

}