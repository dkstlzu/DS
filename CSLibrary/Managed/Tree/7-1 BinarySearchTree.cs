using System.Collections;

namespace CSLibrary;

public class BinarySearchTreeNode<T> : IComparable<BinarySearchTreeNode<T>>, IComparable<T> where T : IComparable<T>
{
    public BinarySearchTreeNode<T>? Left;
    public BinarySearchTreeNode<T>? Right;
    public T? Value;

    public BinarySearchTreeNode(T? value)
    {
        Value = value;
    }

    public int CompareTo(BinarySearchTreeNode<T>? other)
    {
        if (other == null || other.Value == null)
        {
            return 1;
        }

        if (Value == null)
        {
            return -1;
        }

        return Value.CompareTo(other.Value);
    }

    public int CompareTo(T? other)
    {
        if (other == null)
        {
            return 1;
        }

        if (Value == null)
        {
            return -1;
        }
        
        return Value.CompareTo(other);
    }
}

public class BinarySearchTree<T> : ITree<T>, ITreeTraversal<T>, IEnumerable<T> where T : IComparable<T>
{
    public BinarySearchTreeNode<T>? Root;
    private int _count;

    #region ITree

    public void Clear()
    {
        Root = null;
        _count = 0;
    }
    public int Count() => _count;
    public bool IsEmpty() => Count() == 0;

    public void Insert(T item)
    {
        if (IsEmpty())
        {
            return;
        }
        
        BinarySearchTreeNode<T>? node = Root!;

        int compare = node.CompareTo(item);
        
        while (compare != 0)
        {
            if (compare > 0)
            {
                if (node.Left == null)
                {
                    node.Left = new BinarySearchTreeNode<T>(item);
                    _count++;
                    return;
                }
                
                node = node.Left;
            }
            else
            {
                if (node.Right == null)
                {
                    node.Right = new BinarySearchTreeNode<T>(item);
                    _count++;
                    return;
                }
                
                node = node.Right;
            }
            
            compare = node.CompareTo(item);
        }

        throw new InvalidOperationException("Tree Insert 이미 있음");
    }

    public void Remove(T item)
    {
        if (IsEmpty())
        {
            return;
        }
        
        BinarySearchTreeNode<T>? node = Root!;

        int compare = node.CompareTo(item);
        
        while (compare != 0)
        {
            BinarySearchTreeNode<T>? parentNode = null;
            if (compare > 0)
            {
                parentNode = node;
                node = node.Left;
                
                if (node == null)
                {
                    parentNode.Left = null;
                    _count--;
                    return;
                }
            }
            else
            {
                parentNode = node;
                node = node.Right;
                
                if (node == null)
                {
                    parentNode.Right = null;
                    _count--;
                    return;
                }
            }
            
            compare = node.CompareTo(item);
        }

        throw new InvalidOperationException("Tree Remove 해당 아이템 없음");
    }
    
    public bool Contains(T item)
    {
        if (IsEmpty())
        {
            return false;
        }
        
        BinarySearchTreeNode<T>? node = Root!;

        int compare = node.CompareTo(item);
        
        while (compare != 0)
        {
            if (compare > 0)
            {
                node = node.Left;
            }
            else
            {
                node = node.Right;
            }

            if (node == null)
            {
                return false;
            }
            
            compare = node.CompareTo(item);
        }

        return true;
    }

    #endregion ITree

    #region ITreeTraversal

    public void PreorderTraversal(Func<T?, bool> action)
    {
        if (IsEmpty())
        {
            return;
        }

        PreorderTraversalInternal(action, Root!);
    }

    private bool PreorderTraversalInternal(Func<T?, bool> action, BinarySearchTreeNode<T> node)
    {
        bool success = action.Invoke(node.Value);

        if (success)
        {
            return true;
        }
        
        if (node.Left != null)
        {
            success = PreorderTraversalInternal(action, node.Left);

            if (success)
            {
                return true;
            }
        }

        if (node.Right != null)
        {
            success = PreorderTraversalInternal(action, node.Right);
            
            if (success)
            {
                return true;
            }
        }

        return false;
    }

    public void InorderTraversal(Func<T?, bool> action)
    {
        if (IsEmpty())
        {
            return;
        }
        
        InorderTraversalInternal(action, Root!);
    }

    private bool InorderTraversalInternal(Func<T?, bool> action, BinarySearchTreeNode<T> node)
    {
        bool success = false;
        
        if (node.Left != null)
        {
            success = InorderTraversalInternal(action, node.Left);

            if (success)
            {
                return true;
            }
        }
        
        success = action.Invoke(node.Value);

        if (success)
        {
            return true;
        }

        if (node.Right != null)
        {
            success = InorderTraversalInternal(action, node.Right);

            if (success)
            {
                return true;
            }
        }

        return false;
    }

    public void PostorderTraversal(Func<T?, bool> action)
    {
        if (IsEmpty())
        {
            return;
        }
        
        PostorderTraversalInternal(action, Root!);
    }

    private bool PostorderTraversalInternal(Func<T?, bool> action, BinarySearchTreeNode<T> node)
    {
        bool success = false;
        
        if (node.Left != null)
        {
            success = PostorderTraversalInternal(action, node.Left);
            
            if (success)
            {
                return true;
            }
        }

        if (node.Right != null)
        {
            success = PostorderTraversalInternal(action, node.Right);
            
            if (success)
            {
                return true;
            }
        }
        
        success = action.Invoke(node.Value);

        if (success)
        {
            return true;
        }

        return false;
    }

    #endregion ITreeTraversal

    #region IEnumerable

    public IEnumerator<T> GetEnumerator()
    {
        return new Enumerator(this);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    class Enumerator : IEnumerator<T>
    {
        private List<T?> _nodeList;
        private int _index;
        private T? _current;
        
        public Enumerator(BinarySearchTree<T> tree)
        {
            _nodeList = new List<T?>();
            
            tree.PreorderTraversal((t) =>
            {
                _nodeList.Add(t);
                return false;
            });
        }

        public bool MoveNext()
        {
            if (_index < _nodeList.Count)
            {
                _current = _nodeList[_index++];
            }

            return false;
        }

        public void Reset()
        {
            _index = 0;
        }

#pragma warning disable CS8766 // 반환 형식에서 참조 형식의 null 허용 여부가 암시적으로 구현된 멤버와 일치하지 않음(null 허용 여부 특성 때문일 수 있음)
        public T? Current => _current;
#pragma warning restore CS8766 // 반환 형식에서 참조 형식의 null 허용 여부가 암시적으로 구현된 멤버와 일치하지 않음(null 허용 여부 특성 때문일 수 있음)

        object? IEnumerator.Current => Current;
        
        public void Dispose()
        {
            // 할거 없음
        }
    }
    
    #endregion IEnumerable
}