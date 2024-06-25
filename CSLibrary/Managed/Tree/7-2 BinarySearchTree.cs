using System.Collections;

namespace CSLibrary;

public class BinarySearchTree<TOrder, TValue> : SortedBinaryTreeBase<TOrder, TValue> 
    where TOrder : IComparable<TOrder> where TValue : IEquatable<TValue>
{
    public override ITree<TOrder, TValue> Add(TOrder order, TValue? value)
    {
        if (IsEmpty())
        {
            Order = order;
            Value = value;
            Count = 1;
            return this;
        }

        int compare = order.CompareTo(Order);
        
        if (compare == 0)
        {
            throw new InvalidOperationException();
        }

        Count++;
        
        if (compare < 0)
        {
            if (Left == null)
            {
                Left = new BinarySearchTree<TOrder, TValue>();
                Left.Parent = this;
            }
            
            return Left.Add(order, value);
        }
        else
        {
            if (Right == null)
            {
                Right = new BinarySearchTree<TOrder, TValue>();
                Right.Parent = this;
            }
            
            return Right.Add(order, value);
        }
    }

    public override ITree<TOrder, TValue> Remove(TOrder order)
    {
        TryRemove(order);
        return this;
    }

    private bool TryRemove(TOrder order)
    {
        int compare = order.CompareTo(Order);

        if (compare == 0)
        {
            Count--;
            
            if (Right != null)
            {
                SwapWithNextBigNode();
            }
            else if (Left != null)
            {
                SwapWithPreviousSmallNode();
            }
            else
            {
                RemoveFromParent();
            }
            
            return true;
        }
        
        if (compare < 0)
        {
            var left = (BinarySearchTree<TOrder, TValue>?)Left;
            if (left != null && left.TryRemove(order))
            {
                Count--;
                return true;
            }
        }
        
        if (compare > 0)
        {
            var right = (BinarySearchTree<TOrder, TValue>?)Right;
            if (right != null && right.TryRemove(order))
            {
                Count--;
                return true;
            }
        }

        return false;
    }

    private void SwapWithNextBigNode()
    {
        BinaryTreeBase<TOrder, TValue> replace = (BinaryTreeBase<TOrder, TValue>)Right!; 
        replace.Count--;

        while (replace.Left != null)
        {
            replace = (BinarySearchTree<TOrder, TValue>)replace.Left;
            replace.Count--;
        }
        
        replace.RemoveFromParent();
        Order = replace.Order;
        Value = replace.Value;
    }

    private void SwapWithPreviousSmallNode()
    {
        BinarySearchTree<TOrder, TValue>? replace = (BinarySearchTree<TOrder, TValue>)Left!; 
        replace.Count--;

        while (replace.Right != null)
        {
            replace = (BinarySearchTree<TOrder, TValue>)replace.Right;
            replace.Count--;
        }
            
        replace.RemoveFromParent();
        Order = replace.Order;
        Value = replace.Value;
    }

    public override ITree<TOrder, TValue>? GetTree(TOrder order)
    {
        int compare = order.CompareTo(Order);
        
        if (compare == 0)
        {
            return this;
        }

        return compare < 0 ? Left?.GetTree(order) : Right?.GetTree(order);
    }

    public override bool IsValidSorted()
    {
        TOrder? order = default;

        return Inorder((tree) =>
        {
            if (order == null)
            {
                order = tree.Order;
            }
            else
            {
                if (order.CompareTo(tree.Order) >= 0)
                {
                    return true;
                }
            }

            return false;
        });
    }

    public override IEnumerator<ISortedTree<TOrder, TValue>> GetEnumerator()
    {
        return new Enumerator().Init(this);
    }

    class Enumerator : TreeEnumerator
    {
        public override TreeEnumerator Init(ISortedTree<TOrder, TValue> tree)
        {
            _current = tree;
            _nodeList = new List<ISortedTree<TOrder, TValue>>();
            
            ((SortedBinaryTreeBase<TOrder, TValue>)tree).Inorder((node) =>
            {
                _nodeList.Add((SortedBinaryTreeBase<TOrder, TValue>)node);
                return false;
            });
            return this;
        }
    }
}