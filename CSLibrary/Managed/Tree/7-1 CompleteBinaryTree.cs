namespace CSLibrary;

public class CompleteBinaryTree<T> where T : IEquatable<T>
{
    public T? Value;
    private int _count;
    public CompleteBinaryTree<T>? Left { get; private set; }
    public CompleteBinaryTree<T>? Right { get; private set; }

    public CompleteBinaryTree()
    {
        
    }
    
    public CompleteBinaryTree(T value)
    {
        Add(value);
    }

    public CompleteBinaryTree<T>? GetRemoveTargetLeafNode()
    {
        return GetTree(_count);
    }
    
    private CompleteBinaryTree<T>? GetTree(int elementNumber)
    {
        CompleteBinaryTree<T> tree = this;

        foreach (var right in FindWayTo(elementNumber))
        {
            if (right)
            {
                if (tree.Right == null) return null;
                tree = tree.Right;
            }
            else
            {
                if (tree.Left == null) return null;
                tree = tree.Left;
            }
        }

        return tree;
    }

    private bool[] FindWayTo(int elementNumber)
    {
        Stack<bool> toRight = new Stack<bool>();

        while (elementNumber > 1)
        {
            toRight.Push(elementNumber % 2 == 1);
            elementNumber /= 2;
        }

        return toRight.ToArray();
    }

    private bool RightToFind(int elementNumber)
    {
        bool toRight = false;
        
        while (elementNumber > 1)
        {
            toRight = elementNumber % 2 == 1;
            elementNumber /= 2;
        }

        return toRight;
    }

    public CompleteBinaryTree<T> Add(T value)
    {
        if (IsEmpty())
        {
            Value = value;
            _count = 1;
            Left = new CompleteBinaryTree<T>();
            Right = new CompleteBinaryTree<T>();
            return this;
        }

        _count++;

        return RightToFind(_count) ? Right!.Add(value) : Left!.Add(value);
    }

    public void Remove(T value)
    {
        if (IsEmpty())
        {
            throw new InvalidOperationException();
        }

        var leaf = GetTree(_count);
        
        if (leaf == null)
        {
            throw new InvalidOperationException();
        }
        
        Replace(value, this, leaf.Value!);

        RemoveLastLeaf();

        _removeFound = false;
    }

    private void RemoveLastLeaf()
    {
        var wayToLeaf = FindWayTo(_count);
        var leafParent = GetTree(_count / 2);
        var leaf = GetTree(_count);
        var target = this;
        _count--;

        foreach (var right in wayToLeaf)
        {
            target = right ? target.Right : target.Left;

            if (!target.IsEmpty())
            {
                target._count--;
            }
        }
        
        if (leafParent != null)
        {
            if (leaf == leafParent.Right)
            {
                leafParent.Right = null;
            }
            else
            {
                leafParent.Left = null;
            }
        }
    }

    private static bool _removeFound = false;
    
    private static void Replace(T value, CompleteBinaryTree<T> tree, T replaceValue)
    {
        if (value.Equals(tree.Value))
        {
            _removeFound = true;
            tree.Value = replaceValue;
            return;
        }
        
        if (tree.Left != null && !tree.Left.IsEmpty())
            Replace(value, tree.Left, replaceValue);
        
        if (_removeFound) return;
        
        if (tree.Right != null && !tree.Right.IsEmpty())
            Replace(value, tree.Right, replaceValue);
    }
    
    public bool Contains(T value)
    {
        return TryFind(value, out CompleteBinaryTree<T>? node);
    }

    private bool TryFind(T value, out CompleteBinaryTree<T>? node)
    {
        if (IsEmpty())
        {
            node = null;
            return false;
        }
        
        if (value.Equals(Value))
        {
            node = this;
            return true;
        }

        if (Left!= null && !Left.IsEmpty() && Left!.TryFind(value, out node))
        {
            return true;
        }

        if (Right!= null && !Right.IsEmpty() && Right!.TryFind(value, out node))
        {
            return true;
        }

        node = null;
        return false;
    }

    public int Height()
    {
        int i = _count;
        int height = 0;
        
        // Complete tree라는 가정
        while (i > 0)
        {
            i >>= 1;
            height++;
        }

        return height;
    }

    public void Clear()
    {
        _count = 0;
        Left = null;
        Right = null;
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