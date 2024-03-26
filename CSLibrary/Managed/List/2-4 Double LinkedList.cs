using System.Collections;

namespace CSLibrary;

public class DoubleLinkedListNode<T>
{
    public DoubleLinkedListNode<T>? Next;
    public DoubleLinkedListNode<T>? Previous;
    public T? Value;
}

public class DoubleLinkedList<T> : IList<T> where T : IEquatable<T>
{
    private DoubleLinkedListNode<T>? _root;
    private DoubleLinkedListNode<T>? _tail;
    private int _size;
    
    private void AddRootNode(T? t)
    {
        _root = new DoubleLinkedListNode<T>() { Value = t };
        _tail = _root;
        _size = 1;
    }

    private void MoveForwardRoot()
    {
        if (IsEmpty()) return;

        if (_root!.Next == null)
        {
            Clear();
        }
        else
        {
            _root = _root.Next;
            _root.Previous = null;
        }
    }
    
    private void MoveBackwardTail()
    {
        if (IsEmpty()) return;

        if (_tail!.Previous == null)
        {
            Clear();
        }
        else
        {
            _tail = _tail.Previous;
            _tail.Next = null;
        }
    }
    
    #region List Interface

    public void Clear()
    {
        _root = null;
        _tail = null;
        _size = 0;
    }

    public int Count()
    {
        return _size;
    }

    public T? At(int index)
    {
        if (index < 0 || index >= _size)
        {
            throw new IndexOutOfRangeException();
        }

        return NodeAt(index).Value;
    }

    private DoubleLinkedListNode<T> NodeAt(int index)
    {
        if (index < 0 || index >= _size)
        {
            throw new IndexOutOfRangeException();
        }
        
        DoubleLinkedListNode<T> node;
        if (index < _size / 2)
        {
            node = _root!;
            for (int i = 0; i < index; i++)
            {
                node = node.Next!;
            }
        }
        else
        {
            node = _tail!;
            for (int i = _size; i > index + 1; i--)
            {
                node = node.Previous!;
            }
        }

        return node;
    }

    public void Add(T? t)
    {
        if (IsEmpty())
        {
            AddRootNode(t);
            return;
        }
        
        var newNode = new DoubleLinkedListNode<T>() { Value = t };

        _tail!.Next = newNode;
        newNode.Previous = _tail;
        _tail = newNode;
        _size++;
    }

    public void Insert(T? t, int index)
    {
        if (index < 0 || index > _size)
        {
            throw new IndexOutOfRangeException();
        }
        
        if (IsEmpty())
        {
            AddRootNode(t);
            return;
        }
        
        var newNode = new DoubleLinkedListNode<T>(){Value = t};

        if (index == 0)
        {
            newNode.Next = _root;
            _root!.Previous = newNode;
            _root = newNode;
        }
        else
        {
            var prevNode = NodeAt(index - 1);

            if (prevNode == _tail)
            {
                _tail = newNode;
            }

            prevNode.Next = newNode;
        }

        _size++;
    }

    public void Remove(T t)
    {
        if (IsEmpty()) return;

        DoubleLinkedListNode<T>? prev = null;
        DoubleLinkedListNode<T>? node = _root!;

        do
        {
            if (t.Equals(node.Value))
            {
                if (prev == null)
                {
                    _size--;
                    MoveForwardRoot();
                }
                else
                {
                    if (node == _tail)
                    {
                        _tail = prev;
                    }

                    prev.Next = node.Next;
                    _size--;
                }
                break;
            }

            prev = node;
            node = node.Next;
        } while (node != null);
    }

    public void RemoveAt(int index)
    {
        if (index < 0 || index >= _size)
        {
            throw new IndexOutOfRangeException();
        }
        
        if (IsEmpty()) return;

        var targetNode = NodeAt(index);

        if (targetNode == _root)
        {
            _size--;
            MoveForwardRoot();
        }
        else if (targetNode == _tail)
        {
            _size--;
            MoveBackwardTail();
        }
        else
        {
            var prevNode = targetNode.Previous!;
            var nextNode = targetNode.Next!;

            prevNode.Next = nextNode;
            nextNode.Previous = prevNode;

            targetNode.Next = null;
            targetNode.Previous = null;
            _size--;
        }
    }

    public int IndexOf(T t)
    {
        if (IsEmpty()) return -1;
        
        DoubleLinkedListNode<T> node = _root!;
        for (int i = 0; i < _size; i++)
        {
            if (t.Equals(node!.Value)) return i;
            node = node.Next!;
        }

        return -1;
    }

    public bool Contains(T t)
    {
        if (IsEmpty()) return false;
        
        DoubleLinkedListNode<T> node = _root!;
        for (int i = 0; i < _size; i++)
        {
            if (t.Equals(node!.Value)) return true;
            node = node.Next!;
        }

        return false;
    }

    public bool IsEmpty() => _root == null;

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
        private DoubleLinkedListNode<T> _head;
        private DoubleLinkedListNode<T>? _node;
        
        public Enumerator(DoubleLinkedList<T> list)
        {
            if (list.IsEmpty())
            {
                return;
            }
            
            _head = new DoubleLinkedListNode<T>() { Next = list._root };
            _node = _head;
        }
        
        public bool MoveNext()
        {
            _node = _node?.Next;

            return _node != null;
        }

        public void Reset()
        {
            _node = _head;
        }

        public T? Current
        {
#pragma warning disable CS8766 // 반환 형식에서 참조 형식의 null 허용 여부가 암시적으로 구현된 멤버와 일치하지 않음(null 허용 여부 특성 때문일 수 있음)
            get
#pragma warning restore CS8766 // 반환 형식에서 참조 형식의 null 허용 여부가 암시적으로 구현된 멤버와 일치하지 않음(null 허용 여부 특성 때문일 수 있음)
            {
                if (_node == null)
                {
                    throw new InvalidOperationException();
                }
                
                return _node!.Value;
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