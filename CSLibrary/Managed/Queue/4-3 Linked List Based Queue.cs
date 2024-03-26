namespace CSLibrary;

public class LinkedListBasedQueue<T> : IQueue<T> where T : IEquatable<T>
{
    private SingleLinkedListNode<T>? _frontNode = null;
    private SingleLinkedListNode<T>? _backNode = null;
    private int _size = 0;

    private void MoveForwardFrontNode()
    {
        if (IsEmpty()) return;

        if (_frontNode!.Next == null)
        {
            Clear();
        }
        else
        {
            _frontNode = _frontNode.Next;
        }
    }
    
    #region Queue Interface

    public void Clear()
    {
        _frontNode = null;
        _backNode = null;
        _size = 0;
    }

    public int Count()
    {
        return _size;
    }

    public bool IsEmpty() => _frontNode == null;

    public void Enqueue(T? t)
    {
        if (IsEmpty())
        {
            AddRootNode(t);
            return;
        }
        
        var newNode = new SingleLinkedListNode<T>() { Value = t };

        _backNode!.Next = newNode;
        _backNode = newNode;
        _size++;
    }

    private void AddRootNode(T? t)
    {
        _frontNode = new SingleLinkedListNode<T>() { Value = t };
        _backNode = _frontNode;
        _size = 1;
    }

    public T? Dequeue()
    {
        if (IsEmpty())
        {
            throw new InvalidOperationException();
        }

        var value = _frontNode!.Value;

        _size--;
        MoveForwardFrontNode();
        return value;
    }

    public T? Peek()
    {
        if (IsEmpty())
        {
            throw new InvalidOperationException();
        }

        return _frontNode!.Value;
    }

    #endregion
}