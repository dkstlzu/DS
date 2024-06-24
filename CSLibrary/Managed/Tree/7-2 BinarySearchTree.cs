namespace CSLibrary;

public class BinarySearchTree<TOrder, TValue> where TOrder : IComparable<TOrder>
{
    public TOrder? Order { get; private set; }
    public TValue? Value { get; private set; }
    
    public BinarySearchTree<TOrder, TValue>? Left { get; private set; }
    public BinarySearchTree<TOrder, TValue>? Right { get; private set; }

    private int _count;

    public BinarySearchTree()
    {
        
    }
    
    public BinarySearchTree(TOrder order, TValue? value)
    {
        Add(order, value);
    }

    public void Add(TOrder order, TValue? value)
    {
        if (IsEmpty())
        {
            Order = order;
            Value = value;
            _count = 1;
            return;
        }

        if (order.Equals(Order))
        {
            throw new InvalidOperationException();
        }

        _count++;
        
        if (order.CompareTo(Order) < 0)
        {
            if (Left == null)
            {
                Left = new BinarySearchTree<TOrder, TValue>(order, value);
            }
            
            Left.Add(order, value);
        } else if (order.CompareTo(Order) > 0)
        {
            if (Right == null)
            {
                Right = new BinarySearchTree<TOrder, TValue>(order, value);
            }
            
            Right.Add(order, value);
        }
    }

    public void Remove(TOrder order)
    {
        Remove(order, null);
    }

    private void Remove(TOrder order, BinarySearchTree<TOrder, TValue>? parent)
    {
        int compare = order.CompareTo(Order);
        _count--;
        
        if (compare < 0)
        {
            Left.Remove(order, this);
            return;
        }
        
        if (compare > 0)
        {
            Right.Remove(order, this);
            return;
        }
        
        BinarySearchTree<TOrder, TValue>? replace = null; 
        
        if (Right != null)
        {
            // find the smallest bigger node
            parent = this;
            replace = Right;
            replace._count--;

            while (replace.Left != null)
            {
                parent = replace;
                replace = replace.Left;
                replace._count--;
            }
            
            parent.RemoveChild(replace.Order!);
            Order = replace.Order;
            Value = replace.Value;
        }
        else if (Left != null)
        {
            // if does not have bigger node, then find the biggest smaller node
            parent = this;
            replace = Left;
            replace._count--;

            while (replace.Right != null)
            {
                parent = replace;
                replace = replace.Right;
                replace._count--;
            }
            
            parent.RemoveChild(replace.Order!);
            Order = replace.Order;
            Value = replace.Value;
        }
        else
        {
            // it is leaf node and delete target node
            if (parent != null)
            {
                parent.RemoveChild(order);
            }
        }
    }

    private void RemoveChild(TOrder order)
    {
        if (Left != null)
        {
            if (order.Equals(Left.Order))
            {
                Left = null;
                return;
            }
        }

        if (Right != null)
        {
            if (order.Equals(Right.Order))
            {
                Right = null;
                return;
            }
        }

        throw new InvalidOperationException();
    }

    public bool Contains(TOrder order)
    {
        return GetTree(order) != null;
    }

    public TValue? GetValue(TOrder order)
    {
        return GetTree(order)!.Value;
    }

    public BinarySearchTree<TOrder, TValue>? GetTree(TOrder order)
    {
        int compare = order.CompareTo(Order);
        
        if (compare == 0)
        {
            return this;
        }

        if (compare < 0)
        {
            if (Left == null)
            {
                return null;
            }
            
            return Left.GetTree(order);
        } 
        
        // compare > 0
        if (Right == null)
        {
            return null;
        }

        return Right.GetTree(order);
    }

    public int Height()
    {
        int height = 1;

        if (Left != null)
        {
            height = Math.Max(height, Left.Height() + 1);
        }

        if (Right != null)
        {
            height = Math.Max(height, Right.Height() + 1);
        }

        return height;
    }
    
    public void Clear()
    {
        Left = null;
        Right = null;
        _count = 0;
    }

    public int Count()
    {
        return _count;
    }

    public bool IsEmpty()
    {
        return _count == 0;
    }
}