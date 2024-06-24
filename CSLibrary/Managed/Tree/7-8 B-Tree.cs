namespace CSLibrary;

public class BTree<TOrder, TValue> where TOrder : IComparable<TOrder>
{
    public int Degree { get; private set; }
    public int Capacity => Degree - 1;
    public bool  IsLeafNode { get; private set; }

    private (TOrder, TValue?)?[] _keys;
    public BTree<TOrder, TValue>? Parent { get; private set; }
    private BTree<TOrder, TValue>?[] _children;

    public BTree(int degree, bool isLeafNode = true)
    {
        if (degree < 2)
        {
            throw new InvalidOperationException();
        }
        
        Degree = degree;

        _keys = new (TOrder, TValue?)?[degree - 1];
        _children = new BTree<TOrder, TValue>[degree ];
        
        IsLeafNode = isLeafNode;
    }

    public BTree<TOrder, TValue> Add(TOrder order, TValue? value)
    {
        BTree<TOrder, TValue> node = GetAddableNodeLocationOf(order);
        node.AddFromLeaf(order, value, null, null);

        while (node.Parent != null)
        {
            node = node.Parent;
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

    public BTree<TOrder, TValue> Remove(TOrder order, TValue? value)
    {
        var node = GetNodeWith(order);
        node.RemoveOrder(order);

        while (node.Parent != null && node != this)
        {
            node = node.Parent;
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

    public TValue? GetValue(TOrder order)
    {
        if (TryGetSelfValue(order, out TValue? value))
        {
            return value;
        }

        return GetValidChild(order).GetValue(order);
    }

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

    public bool Contains(TOrder order)
    {
        if (SelfContains(order))
        {
            return true;
        }

        return GetValidChild(order).Contains(order);
    }

    private bool SelfContains(TOrder order)
    {
        foreach (var key in _keys)
        {
            if (!key.HasValue) break;
            
            if (order.CompareTo(key.Value.Item1) == 0)
            {
                return true;
            }
        }

        return false;
    }

    private BTree<TOrder, TValue> GetNodeWith(TOrder order)
    {
        if (SelfContains(order))
        {
            return this;
        }

        return GetValidChild(order).GetNodeWith(order);
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
        
        for (int i = 0; i < Capacity; i++)
        {
            if (!_keys[i].HasValue)
            {
                throw new InvalidOperationException();
            }

            if (order.CompareTo(_keys[i]!.Value.Item1) < 0)
            {
                return _children[i]!;
            }
        }

        return _children[^1]!;
    }

    public int SelfCount()
    {
        int count = 0;

        foreach (var key in _keys)
        {
            if (!key.HasValue) break;

            count++;
        }

        return count;
    }

    public int Count()
    {
        int count = 0;

        count += SelfCount();

        foreach (BTree<TOrder,TValue>? child in _children)
        {
            if (child == null) continue;
            
            count += child.Count();
        }

        return count;
    }
}