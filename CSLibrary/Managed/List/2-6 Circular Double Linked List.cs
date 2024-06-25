using System.Collections;

namespace CSLibrary;

public class CircularDoubleLinkedList<T> : IList<T> where T : IEquatable<T>
{
    private DoubleLinkedListNode<T>? _tail = null;
    private int _size = 0;

    #region List Interface

    public void Clear()
    {
        _tail = null;
        _size = 0;
    }

    public int Count => _size;

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
        
        DoubleLinkedListNode<T> node = _tail!;
        for (int i = 0; i <= index; i++)
        {
            node = node.Next!;
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

        var newNode = new DoubleLinkedListNode<T>(){Value = t};
        var root = _tail!.Next!;

        newNode.Next = root;
        newNode.Previous = _tail;
        root.Previous = newNode;
        _tail.Next = newNode;
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
            newNode.Next = _tail!.Next;
            _tail.Next = newNode;
        }
        else
        {
            var previousNode = NodeAt(index - 1);

            if (previousNode == _tail)
            {
                _tail = newNode;
            }
            
            newNode.Next = previousNode.Next;
            previousNode.Next = newNode;
        }
        
        _size++;
    }

    void AddRootNode(T? t)
    {
        _tail = new DoubleLinkedListNode<T>(){Value = t};
        _tail.Next = _tail;
        _tail.Previous = _tail;
        _size = 1;
    }

    public void Remove(T t)
    {
        if (IsEmpty()) return;
        
        DoubleLinkedListNode<T> prev = _tail!;
        DoubleLinkedListNode<T> node = _tail!.Next!;

        do
        {
            if (t.Equals(node.Value))
            {
                if (prev == node)
                {
                    Clear();
                } else 
                {
                    if (node == _tail)
                    {
                        _tail = prev;
                    }
                    
                    prev.Next = node.Next;
                    node.Next!.Previous = prev;
                    _size--;
                }
                break;
            }

            prev = node;
            node = node.Next!;
        } while (node != _tail.Next);
    }

    public void RemoveAt(int index)
    {
        if (index < 0 || index >= _size)
        {
            throw new IndexOutOfRangeException();
        }
        
        if (IsEmpty()) return;
        
        var targetNode = NodeAt(index);

        if (index == 0 && targetNode == _tail)
        {
            Clear();
        } else if (index == 0)
        {
            _tail!.Next = targetNode.Next;
            targetNode.Next!.Previous = _tail;
            _size--;
        }
        else
        {
            var prevNode = NodeAt(index - 1);

            prevNode.Next = targetNode.Next;
            targetNode.Next!.Previous = prevNode;
            _size--;
        }
    }
    
    public int IndexOf(T t)
    {
        if (IsEmpty()) return -1;
        
        DoubleLinkedListNode<T> node = _tail!;
        for (int i = 0; i < _size; i++)
        {
            node = node.Next!;
            if (t.Equals(node.Value)) return i;
        }

        return -1;
    }

    public bool Contains(T t)
    {
        if (IsEmpty()) return false;
        
        DoubleLinkedListNode<T> node = _tail!;
        for (int i = 0; i <= _size; i++)
        {
            if (t.Equals(node.Value)) return true;
            node = node.Next!;
        }

        return false;
    }

    public bool IsEmpty() => _tail == null;

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
        private DoubleLinkedListNode<T> _tail;
        private DoubleLinkedListNode<T>? _node;
        private int _size;
        private int _index = -1;
        
        public Enumerator(CircularDoubleLinkedList<T> list)
        {
            if (list.IsEmpty())
            {
                _index = Int32.MaxValue;
                return;
            }
            
            _tail = list._tail!;
            _size = list.Count();
            Reset();
        }
        
        public bool MoveNext()
        {
            if (_index >= _size - 1)
            {
                return false;
            }

            _node = _node!.Next;
            _index++;
            return true;
        }

        public void Reset()
        {
            _node = _tail;
            _index = -1;
        }

        public T? Current
        {
#pragma warning disable CS8766 // 반환 형식에서 참조 형식의 null 허용 여부가 암시적으로 구현된 멤버와 일치하지 않음(null 허용 여부 특성 때문일 수 있음)
            get
#pragma warning restore CS8766 // 반환 형식에서 참조 형식의 null 허용 여부가 암시적으로 구현된 멤버와 일치하지 않음(null 허용 여부 특성 때문일 수 있음)
            {
                if (_index < 0 || _index >= _size)
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