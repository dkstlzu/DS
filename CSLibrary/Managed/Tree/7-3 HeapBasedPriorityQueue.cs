namespace CSLibrary;


public class Heap<TOrder, TValue> where TOrder : IComparable<TOrder>
{
    public TOrder? Order { get; private set; }
    public TValue? Value { get; private set; }
    public bool IsMaxHeap { get; private set; }
    private bool _isInitialized = false;
    
    public Heap<TOrder, TValue>? Parent { get; private set; }
    public Heap<TOrder, TValue>? Left { get; private set; }
    public Heap<TOrder, TValue>? Right { get; private set; }
    

    public Heap(bool maxHeap)
    {
        IsMaxHeap = maxHeap;
    }

    public Heap(bool maxHeap, TOrder order, TValue? value) : this(maxHeap)
    {
        Add(order, value);
    }

    #region Utility

    private static Heap<TOrder, TValue> GetHeap(Heap<TOrder, TValue> heap, int elementNumber)
    {
        foreach (var right in FindWayTo(elementNumber))
        {
            heap = right ? heap.Right! : heap.Left!;
        }

        return heap;
    }

    private static bool[] FindWayTo(int elementNumber)
    {
        Stack<bool> toRight = new Stack<bool>();

        while (elementNumber > 1)
        {
            toRight.Push(elementNumber % 2 == 1);
            elementNumber /= 2;
        }

        return toRight.ToArray();
    }
    
    private static bool RightToFind(int elementNumber)
    {
        bool toRight = false;
        
        while (elementNumber > 1)
        {
            toRight = elementNumber % 2 == 1;
            elementNumber /= 2;
        }

        return toRight;
    }

    private void Swap(Heap<TOrder, TValue> other)
    {
        (Value, other.Value) = (other.Value, Value);
        (Order, other.Order) = (other.Order, Order);
    }

    #endregion

    
    public void Add(TOrder order, TValue? value)
    {
        if (IsEmpty())
        {
            Order = order;
            Value = value;
            _isInitialized = true;
            return;
        }

        var parent = GetHeap(this, Count() / 2);
        var newHeap = new Heap<TOrder, TValue>(IsMaxHeap, order, value);
        
        if (RightToFind(parent.Count() + 1))
        {
            parent.Right = newHeap;
        }
        else
        {
            parent.Left = newHeap;
        }
        
        newHeap.Parent = parent;

        while (newHeap.Parent != null)
        {
            if ((newHeap.Order!.CompareTo(newHeap.Parent.Order) > 0 && IsMaxHeap) 
                || (newHeap.Order.CompareTo(newHeap.Parent.Order) < 0 && !IsMaxHeap))
            {
                newHeap.Swap(newHeap.Parent);
                newHeap = newHeap.Parent;
            }
            else
            {
                break;
            }
        }
    }

    public TValue? Pop()
    {
        if (IsEmpty())
        {
            throw new InvalidOperationException();
        }

        var value = Value;

        if (Count() == 1)
        {
            Clear();
            return value;
        }
        
        var parent = GetHeap(this, Count() / 2);

        if (RightToFind(parent.Count() + 1))
        {
            Swap(parent.Right!);
            parent.Right = null;
        }
        else
        {
            Swap(parent.Left!);
            parent.Left = null;
        }

        var heap = this;

        while (heap.Left != null || heap.Right != null)
        {
            if (heap.Left != null && heap.Order!.CompareTo(heap.Left.Order) < 0 && IsMaxHeap)
            {
                heap.Swap(heap.Left);
            } else if (heap.Right != null && heap.Order!.CompareTo(heap.Right.Order) < 0 && IsMaxHeap)
            {
                heap.Swap(heap.Right);
            } 
        }
        
        return value;
    }
    
    public void Clear()
    {
        Left = null;
        Right = null;
        _isInitialized = false;
    }

    public int Count()
    {
        if (!_isInitialized)
        {
            return 0;
        }

        int count = 1;
        if (Left != null)
        {
            count += Left.Count();
        }

        if (Right != null)
        {
            count += Right.Count();
        }

        return count;
    }

    public bool IsEmpty()
    {
        return Count() == 0;
    }
}

public class PriorityQueue<TOrder, TValue> where TOrder : IComparable<TOrder>
{
    private Heap<TOrder, TValue> _heap;

    public int Count() => _heap.Count();
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
