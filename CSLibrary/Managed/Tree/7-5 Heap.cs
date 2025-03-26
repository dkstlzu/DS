using System.Collections;

namespace CSLibrary;

public class HeapNode<T>
{
    public HeapNode<T>? Parent;
    public HeapNode<T>? Left;
    public HeapNode<T>? Right;
    public T Value;

    public HeapNode(T value)
    {
        Value = value;
    }
}

public class Heap<T> : ITree<T>
{
    public static Heap<T> Default => new Heap<T>(Comparer<T>.Default);
    
    public HeapNode<T>? Root;
    private int _count;

    private Comparison<T>? _comparison;
    private Comparer<T>? _comparer;
    
    public Heap(Comparison<T> comparison)
    {
        _comparison = comparison;
    }

    public Heap(Comparer<T> comparer)
    {
        _comparer = comparer;
    }

    private int Compare(T a, T b)
    {
        if (_comparer != null)
        {
            return _comparer.Compare(a, b);
        }

        if (_comparison != null)
        {
            return _comparison(a, b);
        }
        
        throw new InvalidOperationException("비교함수가 없음?");
    }

    public T Pop()
    {
        T value = Root!.Value;
        
        Remove(value);
        
        return value;
    }

    public T Peek()
    {
        return Root!.Value;
    }
    
    #region ITree

    public void Clear()
    {
        Root = null;
        _count = 0;
    }

    public int Count() => _count;
    public bool IsEmpty() => _count == 0;
    public void Insert(T item) { throw new NotImplementedException(); }

    public void Remove(T item) { throw new NotImplementedException(); }

    public bool Contains(T item)
    {
        if (Root == null)
        {
            return false;
        }
        
        bool Recursive(HeapNode<T>? node)
        {
            if (node == null)
            {
                return false;
            }
            
            int compare = Compare(item, node.Value);

            if (compare == 0)
            {
                return true;
            }

            if (compare > 0)
            {
                return false;
            }
            
            return Recursive(node.Left) || Recursive(node.Right);
        }
        
        return Recursive(Root);
    }
    #endregion ITree
}

public class PriorityQueue<T> : ICollection
{
    private Heap<T> _internalHeap;

    public PriorityQueue(Comparison<T> comparison)
    {
        _internalHeap = new Heap<T>(comparison);
    }
    
    public PriorityQueue(Comparer<T> comparer)
    {
        _internalHeap = new Heap<T>(comparer);
    }

    public void Enqueue(T item)
    {
        _internalHeap.Insert(item);
    }

    public T Dequeue()
    {
        return _internalHeap.Pop();
    }

    public void Clear()
    {
        _internalHeap.Clear();
    }

    public int Count() => _internalHeap.Count();
    public bool IsEmpty() => Count() == 0;
}