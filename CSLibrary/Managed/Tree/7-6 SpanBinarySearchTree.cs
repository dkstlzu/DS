namespace CSLibrary;

public class SpanTreeNode<T> : IEquatable<SpanTreeNode<T>>
{
    public int Index = -1;
    public T? Value;

    public SpanTreeNode(int index, T? value)
    {
        Index = index;
        Value = value;
    }
    
    public bool Equals(SpanTreeNode<T>? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return EqualityComparer<T>.Default.Equals(Value, other.Value);
    }
}

/// <summary>
/// PreOrder로 Node를 배치합니다. 그로인해서 특정 인덱스부터 메모리배열을 부분으로 때내었을때 해당 배열 값들이 항상 subtree로 사용될 수 있게합니다.
/// Span을 이용한 메모리 활용법입니다.
/// </summary>
public static class SpanTreeMemory<T>
{
    public static SpanTreeNode<T>[] Nodes;
    public static int Capacity => Nodes.Length;
    
    /// <summary>
    /// Root노드만 존재하면 0입니다.
    /// </summary>
    public static int TreeHeight;
    
    public static void Init(int capacity)
    {
        Nodes = new SpanTreeNode<T>[capacity];

        for (int i = 0; i < capacity; i++)
        {
            Nodes[i] = new SpanTreeNode<T>(i, default);
        }

        TreeHeight = GetTreeHeightOfCapacity(capacity);
    }
    //
    // public static SpanTreeNode<T>? GetLeftChild(SpanTreeNode<T> node) => GetLeftChild(node.Index);
    // public static SpanTreeNode<T>? GetLeftChild(int index)
    // {
    //     if (index < 0 || index >= Capacity)
    //     {
    //         return null;
    //     }
    //     
    //     int height = GetHeight(Capacity, index);
    //
    //     if (height < TreeHeight)
    //     {
    //         return Nodes[index + 1];
    //     }
    //     
    // }
    //
    // public static int GetLeftChildIndex(SpanTreeNode<T> node) => GetLeftChildIndex(node.Index);
    // public static int GetLeftChildIndex(int index)
    // {
    //     return index + 1;
    // }
    //
    // public static SpanTreeNode<T> GetRightChild(SpanTreeNode<T> node) => GetRightChild(node.Index);
    //
    // public static SpanTreeNode<T> GetRightChild(int index)
    // {
    //     
    // }

    private static int GetTreeHeightOfCapacity(int capacity)
    {
        int powerOfTwo = capacity + 1;
        
        int height = -1;

        while (powerOfTwo > 0)
        {
            powerOfTwo >>= 1;
            if (powerOfTwo > 0)
            {
                height++;
            }
        }

        return height;
    }

    public static int GetHeight(int treeHeight, int index)
    {
        Queue<int> q = new Queue<int>();

        int height = treeHeight;

        while (treeHeight >= 0)
        {
            q.Enqueue(treeHeight);
            treeHeight--;
        }

        while (index > 0)
        {
            int subTreeHeight = q.Dequeue() - 1;

            while (subTreeHeight >= 0)
            {
                q.Enqueue(subTreeHeight);
                subTreeHeight--;
            }
            
            index--;
        }

        
        
        return height;
    }
    
    public static int GetDepth(int treeHeight, int index) => treeHeight - GetHeight(treeHeight, index);
}

public class SpanBinarySearchTree<T>
{
    public SpanTreeNode<T> Root;

    // private Span<TreeNode<T>> _nodes;
}