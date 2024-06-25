namespace CSLibrary;

public class CompleteBinaryTree<T> : BinaryTreeBase<T, T> where T : IEquatable<T>, IComparable<T>
{
    public override int ChildrenCount { get; } = 2;

    public CompleteBinaryTree()
    {
        
    }

    public CompleteBinaryTree(T value)
    {
        Order = value;
        Value = value;
        Count = 1;
    }
    
    public CompleteBinaryTree<T>? GetRemoveTargetLeafNode()
    {
        return (CompleteBinaryTree<T>?)GetTree(Count);
    }

    public override ITree<T, T>? GetChild(int index)
    {
        return index == 0 ? Left : Right;
    }

    public override void SetChild(int index, ITree<T, T>? child)
    {
        if (index == 0)
        {
            Left = (CompleteBinaryTree<T>?)child;
        }
        else
        {
            Right = (CompleteBinaryTree<T>?)child;
        }
    }

    public override ITree<T, T> Add(T order, T? value) => Add(order);
    public CompleteBinaryTree<T> Add(T value)
    {
        if (IsEmpty())
        {
            Order = value;
            Value = value;
            Count = 1;
            return this;
        }

        Count++;

        if (RightToFind(Count)!.Value)
        {
            if (Right == null)
            {
                Right = new CompleteBinaryTree<T>(value);
                Right.Parent = this;
                return (CompleteBinaryTree<T>)Right;
            }
            else
            {
                return ((CompleteBinaryTree<T>)Right).Add(value);
            }
        }
        else
        {
            if (Left == null)
            {
                Left = new CompleteBinaryTree<T>(value);
                Left.Parent = this;
                return (CompleteBinaryTree<T>)Left;
            }
            else
            {
                return ((CompleteBinaryTree<T>)Left).Add(value);
            }
        }
    }

    public override ITree<T, T> Remove(T value)
    {
        if (IsEmpty())
        {
            throw new InvalidOperationException();
        }

        var leaf = GetTree(Count)!;

        if (!Preorder((node) =>
            {
                if (value.Equals(node.Value))
                {
                    node.Value = leaf.Value;
                    return true;
                }

                return false;
            }))
        {
            throw new InvalidOperationException();
        }

        var parent = (CompleteBinaryTree<T>?)leaf.Parent;

        while (leaf != null)
        {
            leaf.Count--;
            leaf = (CompleteBinaryTree<T>?)leaf.Parent;
        }

        if (parent != null)
        {
            if (parent.Right != null)
            {
                parent.Right = null;
            }
            else
            {
                parent.Left = null;
            }
        }

        return this;
    }

    public override ITree<T, T>? GetTree(T value)
    {
        ITree<T, T>? result = null;
        
        Preorder((t) =>
        {
            if (!IsEmpty() && value.Equals(t.Value))
            {
                result = t;
                return true;
            }
            
            return false;
        });

        return result;
    }

    public override int Height()
    {
        int i = Count;
        int height = 0;
        
        // Complete tree라는 가정
        while (i > 0)
        {
            i >>= 1;
            height++;
        }

        return height;
    }
}