namespace CSLibrary;


public class Heap<TOrder, TValue> : BinaryTreeBase<TOrder, TValue> 
    where TOrder : IComparable<TOrder> where TValue : IEquatable<TValue>
{
    public bool IsMaxHeap { get; private set; }
    
    public Heap(bool maxHeap)
    {
        IsMaxHeap = maxHeap;
    }

    public Heap(bool maxHeap, TOrder order, TValue? value) : this(maxHeap)
    {
        Add(order, value);
    }

    public override ITree<TOrder, TValue> Add(TOrder order, TValue? value)
    {
        if (IsEmpty())
        {
            Order = order;
            Value = value;
            return this;
        }

        BinaryTreeBase<TOrder, TValue> parent = GetTree((Count + 1) / 2)!;
        BinaryTreeBase<TOrder, TValue> newHeap = new Heap<TOrder, TValue>(IsMaxHeap, order, value);

        parent.AddChild(newHeap);

        while (newHeap.Parent != null)
        {
            if ((newHeap.Order!.CompareTo(newHeap.Parent.Order) > 0 && IsMaxHeap) 
                || (newHeap.Order.CompareTo(newHeap.Parent.Order) < 0 && !IsMaxHeap))
            {
                newHeap.SwapWith(newHeap.Parent);
                newHeap = (BinaryTreeBase<TOrder, TValue>)newHeap.Parent;
            }
            else
            {
                break;
            }
        }

        return this;
    }

    public override ITree<TOrder, TValue> Remove(TOrder order)
    {
        Heap<TOrder, TValue>? target = (Heap<TOrder, TValue>?)GetTree(order);

        if (target == null)
        {
            throw new InvalidOperationException();
        }

        target.Pop();
        return this;
    }

    public override ITree<TOrder, TValue>? GetTree(TOrder order)
    {
        ITree<TOrder, TValue>? result = null;

        Preorder((tree) =>
        {
            if (order.Equals(tree.Order))
            {
                result = tree;
                return true;
            }

            return false;
        });

        return result;
    }

    public TValue? Pop()
    {
        if (IsEmpty())
        {
            throw new InvalidOperationException();
        }

        var value = Value;

        if (Count == 1)
        {
            Clear();
            return value;
        }
        
        BinaryTreeBase<TOrder, TValue> parent = GetTree(Count / 2)!;
        
        if (RightToFind(parent.Count)!.Value)
        {
            SwapWith(parent.Right!);
            parent.Right = null;
        }
        else
        {
            SwapWith(parent.Left!);
            parent.Left = null;
        }

        var heap = this;

        while (heap.Left != null || heap.Right != null)
        {
            if (heap.Left != null && heap.Order!.CompareTo(heap.Left.Order) < 0 && IsMaxHeap)
            {
                heap.SwapWith(heap.Left);
            } else if (heap.Right != null && heap.Order!.CompareTo(heap.Right.Order) < 0 && IsMaxHeap)
            {
                heap.SwapWith(heap.Right);
            } 
        }
        
        return value;
    }
}

public class PriorityQueue<TOrder, TValue> : ICollection
    where TOrder : IComparable<TOrder> where TValue : IEquatable<TValue>
{
    private Heap<TOrder, TValue> _heap;

    public int Count => _heap.Count;
    public bool IsEmpty() => _heap.IsEmpty();
    public void Clear() => _heap.Clear();

    public PriorityQueue(bool maxPriority)
    {
        _heap = new Heap<TOrder, TValue>(maxPriority);
    }

    public void Enqueue(TOrder order, TValue? value)
    {
        _heap.Add(order, value);
    }

    public TValue? Dequeue()
    {
        return _heap.Pop();
    }
}
