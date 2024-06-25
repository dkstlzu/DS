namespace CSLibrary;

public class BTree<TOrder, TValue> : SortedBTreeBase<TOrder, TValue> 
    where TOrder : IComparable<TOrder> where TValue : IEquatable<TValue>
{
    public int Degree { get; private set; }
    public override int ChildrenCount => Degree;
    public int Capacity => Degree - 1;
    
    public bool  IsLeafNode { get; private set; }

    private (TOrder, TValue?)?[] _keys;
    private ITree<TOrder, TValue>?[] _children;

    public BTree(int degree, bool isLeafNode = true)
    {
        if (degree < 2)
        {
            throw new InvalidOperationException();
        }
        
        Degree = degree;

        _keys = new (TOrder, TValue?)?[degree - 1];
        _children = new ITree<TOrder, TValue>[degree ];
        
        IsLeafNode = isLeafNode;
    }
    
    public override (TOrder, TValue?)? GetKey(int index)
    {
        if (index < 0 || index >= Capacity)
        {
            throw new IndexOutOfRangeException();
        }

        return _keys[index];
    }

    public override void SetKey(int index, (TOrder, TValue?) key)
    {
        if (index < 0 || index >= Capacity)
        {
            throw new IndexOutOfRangeException();
        }

        _keys[index] = key;
    }

    public override ITree<TOrder, TValue>? GetChild(int index)
    {
        if (index < 0 || index >= ChildrenCount)
        {
            throw new IndexOutOfRangeException();
        }

        return _children[index];
    }

    public override void SetChild(int index, ITree<TOrder, TValue>? child)
    {
        if (index < 0 || index >= ChildrenCount)
        {
            throw new IndexOutOfRangeException();
        }

        _children[index] = child;
    }
    
    public override BTree<TOrder, TValue> Add(TOrder order, TValue? value)
    {
        BTree<TOrder, TValue> node = GetAddableNodeLocationOf(order);
        node.AddFromLeaf(order, value, null, null);

        while (node.Parent != null)
        {
            node = (BTree<TOrder, TValue>)node.Parent;
        }

        return node;
    }

    private void AddFromLeaf(TOrder order, TValue? value, 
        BTree<TOrder, TValue>? childNode, BTree<TOrder, TValue>? splittedNode)
    {
        var upRiseObj = Insert(order, value);
    }

    private (TOrder, TValue?)? Insert(TOrder order, TValue? value)
    {
        int count = SelfCount();
        
        int index = 0;
        
        for (; index < count; index++)
        {
            if (order.CompareTo(_keys[index]!.Value.Item1) < 0)
            {
                break;
            }
        }

        (TOrder, TValue?)? result = null;
        
        if (count == Capacity)
        {
            result = _keys[index]!.Value;
        }
        else
        {
            for (int i = count; i > index; i--)
            {
                _keys[i] = _keys[i - 1];
            }
        }

        _keys[index] = (order, value);
        return result;
    }

    public override ITree<TOrder, TValue> Remove(TOrder order)
    {
        BTree<TOrder, TValue>? node = (BTree<TOrder, TValue>?)GetTree(order);
        node.RemoveOrder(order);

        while (node.Parent != null && node != this)
        {
            node = (BTree<TOrder, TValue>)node.Parent;
        }

        return node;
    }

    private void RemoveOrder(TOrder order)
    {
        if (!IsLeafNode)
        {
            // get node which has the smallest but bigger key than order
            
        }
        
        for (int i = 0; i < Capacity; i++)
        {
            
        }
    }

    public override ITree<TOrder, TValue>? GetTree(TOrder order)
    {
        if (TryGetSelfValue(order, out TValue? value))
        {
            return this;
        }

        return GetValidChild(order).GetTree(order);
    }

    private bool SelfContains(TOrder order) => TryGetSelfValue(order, out var value);
    private bool TryGetSelfValue(TOrder order, out TValue? value)
    {
        foreach (var key in _keys)
        {
            if (!key.HasValue) break;
            
            if (order.CompareTo(key.Value.Item1) == 0)
            {
                value = key.Value.Item2;
                return true;
            }
        }

        value = default;
        return false;
    }

    private BTree<TOrder, TValue> GetAddableNodeLocationOf(TOrder order)
    {
        if (SelfContains(order))
        {
            throw new InvalidOperationException();
        }
        
        if (IsLeafNode)
        {
            return this;
        }
        
        return GetValidChild(order).GetAddableNodeLocationOf(order);
    }

    private BTree<TOrder, TValue> GetValidChild(TOrder order)
    {
        if (SelfContains(order))
        {
            throw new InvalidOperationException();
        }

        int i = 0;
        for (; i < Capacity; i++)
        {
            if (!_keys[i].HasValue)
            {
                throw new InvalidOperationException();
            }

            if (order.CompareTo(_keys[i]!.Value.Item1) < 0)
            {
                break;
            }
        }

        return (BTree<TOrder, TValue>)_children[i]!;
    }

    public override int SelfCount()
    {
        int count = 0;

        foreach (var key in _keys)
        {
            if (!key.HasValue) break;

            count++;
        }

        return count;
    }

    public override int Count
    {
        get
        {
            int count = 0;

            count += SelfCount();

            foreach (var child in _children)
            {
                if (child == null) continue;
            
                count += child.Count;
            }

            return count;
        }
    }
}