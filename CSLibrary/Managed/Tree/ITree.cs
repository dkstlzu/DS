using System.Collections;

namespace CSLibrary;

public interface ITree<TOrder, TValue> : ICollection where TOrder : IComparable<TOrder> where TValue : IEquatable<TValue>
{
    public TOrder? Order { get; protected internal set; }
    public TValue? Value { get; protected internal set; }

    public ITree<TOrder, TValue>? Parent { get; protected internal set; }

    public int ChildrenCount { get; }
    ITree<TOrder, TValue>? GetChild(int index);
    void SetChild(int index, ITree<TOrder, TValue> child);
    
    ITree<TOrder, TValue> Add(TOrder order, TValue? value);
    ITree<TOrder, TValue> Remove(TOrder order);
    ITree<TOrder, TValue>? GetTree(TOrder order);
    TValue? GetValue(TOrder order);
    bool Contains(TOrder order);
    
    /// <summary>
    /// Max distance to leaf node
    /// </summary>
    int Height();
    
    /// <summary>
    /// Distance from root node
    /// </summary>
    int Depth();
    
    bool Preorder(Func<ITree<TOrder, TValue>, bool> action);
    bool Postorder(Func<ITree<TOrder, TValue>, bool> action);
}

public interface ISortedTree<TOrder, TValue> : ITree<TOrder, TValue>, IEnumerable<ISortedTree<TOrder, TValue>>
    where TOrder : IComparable<TOrder> where TValue : IEquatable<TValue>
{
    bool IsValidSorted();
}

public abstract class TreeBase<TOrder, TValue> : ITree<TOrder, TValue>
    where TOrder : IComparable<TOrder> where TValue : IEquatable<TValue>
{
    public TOrder? Order { get; set; }
    public TValue? Value { get; set; }
    public ITree<TOrder, TValue>? Parent { get; set; }
    public abstract int ChildrenCount { get; }
    
    public ITree<TOrder, TValue>? this[int index]
    {
        get
        {
            if (index < 0 || index >= ChildrenCount)
            {
                throw new IndexOutOfRangeException();
            }
            
            return GetChild(index);
        }
        set
        {
            if (index < 0 || index >= ChildrenCount)
            {
                throw new IndexOutOfRangeException();
            }
            
            SetChild(index, value);
        }
    }

    public abstract ITree<TOrder, TValue>? GetChild(int index);
    public abstract void SetChild(int index, ITree<TOrder, TValue>? child);
    public abstract ITree<TOrder, TValue> Add(TOrder order, TValue? value);
    public abstract ITree<TOrder, TValue> Remove(TOrder order);
    public abstract ITree<TOrder, TValue>? GetTree(TOrder order);

    public virtual TValue? GetValue(TOrder order)
    {
        var node = GetTree(order);

        if (node == null)
        {
            return default;
        }
        
        return node.Value;
    }

    public bool Contains(TOrder order) => GetTree(order) != null;

    public virtual int Height()
    {
        int height = 1;

        for (int i = 0; i < ChildrenCount; i++)
        {
            var child = this[i];

            if (child != null)
            {
                height = Math.Max(height, child.Height() + 1);
            }
        }

        return height;
    }

    public int Depth()
    {
        int depth = 0;

        ITree<TOrder, TValue> node = this;

        while (node.Parent != null)
        {
            node = node.Parent;
            depth++;
        }

        return depth;
    }
    
    public virtual void Clear()
    {
        Parent = null;
        
        for (int i = 0; i < ChildrenCount; i++)
        {
            this[i] = null;
        }

        Count = 0;
    }

    public virtual int Count { get; protected internal set; }
    public bool IsEmpty() => Count == 0;
    

    public bool Preorder(Func<ITree<TOrder, TValue>, bool> action)
    {
        if (action(this))
        {
            return true;
        }

        for (int i = 0; i < ChildrenCount; i++)
        {
            var child = (TreeBase<TOrder, TValue>?)this[i];

            if (child != null && child.Preorder(action))
            {
                return true;
            }
        }

        return false;
    }

    public bool Postorder(Func<ITree<TOrder, TValue>, bool> action)
    {
        for (int i = 0; i < ChildrenCount; i++)
        {
            var child = (TreeBase<TOrder, TValue>?)this[i];

            if (child != null && child.Postorder(action))
            {
                return true;
            }
        }
        
        if (action(this))
        {
            return true;
        }

        return false;
    }
    
    protected internal void SwapWith(ITree<TOrder, TValue> other)
    {
        (Value, other.Value) = (other.Value, Value);
        (Order, other.Order) = (other.Order, Order);
    }
}

public abstract class BinaryTreeBase<TOrder, TValue> : TreeBase<TOrder, TValue>
    where TOrder : IComparable<TOrder> where TValue : IEquatable<TValue>
{
    public override int ChildrenCount { get; } = 2;
    
    public ITree<TOrder, TValue>? Left { get; protected internal set; }
    public ITree<TOrder, TValue>? Right { get; protected internal set; }

    public override ITree<TOrder, TValue>? GetChild(int index)
    {
        if (index < 0 || index >= ChildrenCount)
        {
            throw new IndexOutOfRangeException();
        }
        
        if (index == 0)
        {
            return Left;
        }
        else
        {
            return Right;
        }
    }

    public override void SetChild(int index, ITree<TOrder, TValue>? child)
    {
        if (index < 0 || index >= ChildrenCount)
        {
            throw new IndexOutOfRangeException();
        }
        
        if (index == 0)
        {
            Left = (BinaryTreeBase<TOrder, TValue>?)child;
        }
        else
        {
            Right = (BinaryTreeBase<TOrder, TValue>?)child;
        }
    }

    public bool Inorder(Func<BinaryTreeBase<TOrder, TValue>, bool> action)
    {
        if (Left != null && ((BinaryTreeBase<TOrder, TValue>)Left).Inorder(action))
        {
            return true;
        }
        
        if (action(this))
        {
            return true;
        }
        
        if (Right != null && ((BinaryTreeBase<TOrder, TValue>)Right).Inorder(action))
        {
            return true;
        }
        
        return false;
    }

    protected internal void AddChild(BinaryTreeBase<TOrder, TValue> child)
    {
        if (Left == null)
        {
            Left = child;
            child.Parent = this;
        } else if (Right == null)
        {
            Right = child;
            child.Parent = this;
        }
        else
        {
            throw new InvalidOperationException();
        }
    }
    
    protected internal void RemoveChild(TOrder order)
    {
        if (Left != null && order.Equals(Left.Order))
        {
            Left.Parent = null;
            Left = null;
        } else if (Right != null && (order.Equals(Right.Order)))
        {
            Right.Parent = null;
            Right = null;
        }

        throw new InvalidOperationException();
    }

    protected internal void RemoveFromParent()
    {
        if (Parent == null)
        {
            return;
        }

        var parent = (BinaryTreeBase<TOrder, TValue>)Parent;
        
        if (parent.Left == this)
        {
            parent.Left = null;
            Parent = null;
        } else if (parent.Right == this)
        {
            parent.Right = null;
            Parent = null;
        }
    }
    
    protected internal BinaryTreeBase<TOrder, TValue>? GetTree(int elementNumber)
    {
        if (IsEmpty())
        {
            return null;
        }
        
        var node = this;
        
        foreach (var right in FindWayTo(elementNumber))
        {
            node = right ? (BinaryTreeBase<TOrder, TValue>)node.Right! : (BinaryTreeBase<TOrder, TValue>)node.Left!;
        }

        return node;
    }

    protected void SwapWith(int elementNumber)
    {
        var target = GetTree(elementNumber);
        if (target != null)
        {
            SwapWith(target);
        }
    }

    protected static bool[] FindWayTo(int elementNumber)
    {
        Stack<bool> toRight = new Stack<bool>();

        while (elementNumber > 1)
        {
            toRight.Push(elementNumber % 2 == 1);
            elementNumber /= 2;
        }

        return toRight.ToArray();
    }
    
    protected static bool? RightToFind(int elementNumber)
    {
        if (elementNumber <= 1)
        {
            return null;
        }
        
        bool toRight = false;
        
        while (elementNumber > 1)
        {
            toRight = elementNumber % 2 == 1;
            elementNumber /= 2;
        }

        return toRight;
    }
}

public abstract class SortedTreeBase<TOrder, TValue> : TreeBase<TOrder, TValue>, ISortedTree<TOrder, TValue>
    where TOrder : IComparable<TOrder> where TValue : IEquatable<TValue>
{
    public abstract bool IsValidSorted();

    public virtual IEnumerator<ISortedTree<TOrder, TValue>> GetEnumerator()
    {
        return new TreeEnumerator().Init(this);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
    
    protected class TreeEnumerator : IEnumerator<ISortedTree<TOrder, TValue>>
    {
        protected List<ISortedTree<TOrder, TValue>> _nodeList;
        protected int _index;
        protected ISortedTree<TOrder, TValue> _current;

        public virtual TreeEnumerator Init(ISortedTree<TOrder, TValue> tree)
        {
            throw new NotImplementedException();
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

        public ISortedTree<TOrder, TValue> Current => _current;

        object? IEnumerator.Current => Current;
        
        public void Dispose()
        {
            // 할거 없음
        }
    }
}

public abstract class SortedBinaryTreeBase<TOrder, TValue> : BinaryTreeBase<TOrder, TValue>, ISortedTree<TOrder, TValue>
    where TOrder : IComparable<TOrder> where TValue : IEquatable<TValue>
{
    public abstract bool IsValidSorted();

    public virtual IEnumerator<ISortedTree<TOrder, TValue>> GetEnumerator()
    {
        return new TreeEnumerator().Init(this);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
    
    protected class TreeEnumerator : IEnumerator<ISortedTree<TOrder, TValue>>
    {
        protected List<ISortedTree<TOrder, TValue>> _nodeList;
        protected int _index;
        protected ISortedTree<TOrder, TValue> _current;

        public virtual TreeEnumerator Init(ISortedTree<TOrder, TValue> tree)
        {
            throw new NotImplementedException();
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

        public ISortedTree<TOrder, TValue> Current => _current;

        object? IEnumerator.Current => Current;
        
        public void Dispose()
        {
            // 할거 없음
        }
    }
}

public abstract class SortedBTreeBase<TOrder, TValue> : SortedTreeBase<TOrder, TValue>
    where TOrder : IComparable<TOrder> where TValue : IEquatable<TValue>
{
    public abstract int SelfCount();

    public abstract (TOrder, TValue?)? GetKey(int index);
    public abstract void SetKey(int index, (TOrder, TValue?) key);
    
    public bool Inorder(Func<TOrder, TValue?, bool> action)
    {
        int count = SelfCount();
        
        for (int i = 0; i < count; i++)
        {
            if (((SortedBTreeBase<TOrder, TValue>?)GetChild(i)!).Inorder(action))
            {
                return true;
            }
            
            var key = GetKey(i)!;
            if (action(key.Value.Item1, key.Value.Item2))
            {
                return true;
            }
        }
        
        if (((SortedBTreeBase<TOrder, TValue>?)GetChild(count)!).Inorder(action))
        {
            return true;
        }

        return false;
    }

    public override bool IsValidSorted()
    {
        TOrder? order = default;

        return !Inorder((treeOrder, treeValue) =>
        {
            if (order == null)
            {
                order = treeOrder;
            }
            else
            {
                if (order.CompareTo(treeOrder) >= 0)
                {
                    return true;
                }
            }

            return false;
        });
    }
}