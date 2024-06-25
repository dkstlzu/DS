using System.Diagnostics;

namespace CSLibrary;

public class Tree23<TOrder, TValue> : SortedTreeBase<TOrder, TValue> 
    where TOrder : IComparable<TOrder> where TValue : IEquatable<TValue>
{
    public const int MAX_ELEMENT = 2;
    
    public (TOrder, TValue?)? SmallKey;
    public (TOrder, TValue?)? LargeKey;

    public new Tree23<TOrder, TValue>? Parent { get; protected set; }
    
    public override int ChildrenCount => 3;

    public Tree23<TOrder, TValue>? Left;
    public Tree23<TOrder, TValue>? Middle;
    /// <summary>
    /// Use only when two elements are used
    /// </summary>
    public Tree23<TOrder, TValue>? Right;

    public bool IsLeafNode;
    
    public Tree23()
    {
        IsLeafNode = true;
    }

    public Tree23(bool isLeafNode)
    {
        IsLeafNode = isLeafNode;
    }

    public override ITree<TOrder, TValue>? GetChild(int index)
    {
        if (index < 0 || index >= ChildrenCount)
        {
            throw new IndexOutOfRangeException();
        }

        if (index == 0)
        {
            return Left;
        } else if (index == 1)
        {
            return Middle;
        }
        else
        {
            return Right;
        }
    }

    public override void SetChild(int index, ITree<TOrder, TValue>? child)
    {
        if (index < 0 || index >= ChildrenCount)
        {
            throw new IndexOutOfRangeException();
        }
        
        if (index == 0)
        {
            Left = (Tree23<TOrder, TValue>?)child;
        } else if (index == 1)
        {
            Middle = (Tree23<TOrder, TValue>?)child;
        }
        else
        {
            Right = (Tree23<TOrder, TValue>?)child;
        }
    }

    public override Tree23<TOrder, TValue> Add(TOrder order, TValue? value)
    {
        FindLeafNodeWith(order).AddFromLeaf(order, value, null, null);

        var node = this;

        while (node.Parent != null)
        {
            node = node.Parent;
        }

        return node;
    }

    private void AddFromLeaf(TOrder order, TValue? value, 
        Tree23<TOrder, TValue>? childNode, Tree23<TOrder, TValue>? splittedChildNode)
    {
        switch (SelfCount())
        {
            default:
                throw new InvalidOperationException();
            case 0:
                SmallKey = (order, value);
                return;
            case 1:
                if (order.CompareTo(SmallKey!.Value.Item1) < 0)
                {
                    LargeKey = SmallKey;
                    SmallKey = (order, value);
                }
                else
                {
                    LargeKey = (order, value);
                }

                if (IsLeafNode)
                {
                    return;
                }
                
                if (childNode == Left)
                {
                    Right = Middle;
                    Middle = splittedChildNode;

                } else if (childNode == Middle)
                {
                    Right = splittedChildNode;
                }
                else
                {
                    throw new InvalidOperationException();
                }
                
                splittedChildNode!.Parent = this;
                
                return;
            case 2:
                (order, value) = Insert(order, value);

                if (IsLeafNode)
                {
                    break;
                }
                
                Debug.Assert(splittedChildNode != null);
                
                var splittedNode = new Tree23<TOrder, TValue>(false);
                splittedNode.SmallKey = LargeKey;
                LargeKey = null;
            
                if (childNode == Left)
                {
                    splittedNode.Left = Middle;
                    splittedNode.Middle = Right;

                    Middle = splittedChildNode;
                    Right = null;
                    splittedChildNode.Parent = this;
                } else if (childNode == Middle)
                {
                    splittedNode.Left = splittedChildNode;
                    splittedNode.Middle = Right;
                    splittedChildNode.Parent = splittedNode;

                    Right = null;
                } else if (childNode == Right)
                {
                    splittedNode.Left = Right;
                    splittedNode.Middle = splittedChildNode;
                    splittedChildNode.Parent = splittedNode;

                    Right = null;
                }

                if (Parent == null)
                {
                    Parent = new Tree23<TOrder, TValue>(false);

                    Parent.SmallKey = (order, value);

                    Parent.Left = this;
                    Parent.Middle = splittedNode;
                    splittedNode.Parent = Parent;
                }
                else
                {
                    Parent.AddFromLeaf(order, value, this, splittedNode);
                }
                
                break;
        }
    }
    
        
    /// <returns>Key value that should go up node</returns>
    private (TOrder, TValue?) Insert(TOrder order, TValue? value)
    {
        if (SelfCount() < MAX_ELEMENT)
        {
            throw new InvalidOperationException();
        }
        
        if (order.CompareTo(SmallKey!.Value.Item1) < 0)
        {
            var temp = SmallKey;
            SmallKey = (order, value);
            return temp.Value;
        }
        
        if (order.CompareTo(LargeKey!.Value.Item1) < 0)
        {
            return (order, value);
        } 
        
        if (order.CompareTo(LargeKey.Value.Item1) > 0)
        {
            var temp = LargeKey;
            LargeKey = (order, value);
            return temp.Value;
        }
        
        throw new InvalidOperationException();
    }

    public override Tree23<TOrder, TValue> Remove(TOrder order)
    {
        Tree23<TOrder, TValue> node = (Tree23<TOrder, TValue>)GetTree(order);

        if (node.RemoveAndEmpty(order))
        {
            RearrangeTree(node);
        }

        while (node.Parent != null)
        {
            if (node == this)
            {
                return this;
            }
            
            node = node.Parent;
        }

        return node;
    }

    public override ITree<TOrder, TValue> GetTree(TOrder order)
    {
        int compare1 = SmallKey != null ? order.CompareTo(SmallKey.Value.Item1) : -1;
        int compare2 = LargeKey != null ? order.CompareTo(LargeKey.Value.Item1) : -1;
        
        if (compare1 == 0 || compare2 == 0)
        {
            return this;
        } else if (IsLeafNode)
        {
            throw new InvalidOperationException();
        }
        
        if (compare1 < 0)
        {
            return Left!.GetTree(order);
        } else if (compare2 < 0)
        {
            return Middle!.GetTree(order);
        } else
        {
            return Right!.GetTree(order);
        }
    }

    /// <returns>true when removed all elements</returns>
    public bool RemoveAndEmpty(TOrder order)
    {
        if (SmallKey != null && order.CompareTo(SmallKey.Value.Item1) == 0)
        {
            SmallKey = LargeKey;
            return LargeKey == null;
        }
        
        if (LargeKey != null && order.CompareTo(LargeKey.Value.Item1) == 0)
        {
            LargeKey = null;
            return false;
        }

        throw new InvalidOperationException();
    }

    private ChildPosition GetSibling(out Tree23<TOrder, TValue> sibling)
    {
        Debug.Assert(Parent != null);
        
        if (this == Parent.Left)
        {
            sibling = Parent.Middle!;
            return ChildPosition.MIDDLE;
        } else if (this == Parent.Middle)
        {
            if (Parent.Right != null)
            {
                if (Parent.Left!.SelfCount() >= Parent.Right.SelfCount())
                {
                    sibling = Parent.Left;
                    return ChildPosition.LEFT;
                }
                else
                {
                    sibling = Parent.Right;
                    return ChildPosition.RIGHT;
                }
            }

            sibling = Parent.Left!;
            return ChildPosition.LEFT;
        } else /*if (this == Parent.Right)*/
        {
            sibling = Parent.Middle!;
            return ChildPosition.MIDDLE;
        }
    }

    private static void RearrangeTree(Tree23<TOrder, TValue> tree)
    {
        if (tree.Parent == null || tree.SelfCount() > 0)
        {
            return;
        }

        ChildPosition selfPosition = tree.GetPosition();
        var parent = tree.Parent;
        int parentSelfCount = parent.SelfCount();
        ChildPosition siblingPosition = tree.GetSibling(out var sibling);
        int siblingSelfCount = sibling.SelfCount();

        // tree.Parent has one key
        if (parentSelfCount == 1)
        {
            switch (siblingPosition)
            {
                default:
                case ChildPosition.NOPARENT:
                case ChildPosition.RIGHT:
                    throw new InvalidOperationException();
                case ChildPosition.LEFT:
                    if (siblingSelfCount == 1)
                    {
                        sibling.LargeKey = tree.Parent.SmallKey;
                        tree.Parent.SmallKey = null;
                        tree.Parent.Middle = null;
                    }
                    else
                    {
                        tree.SmallKey = tree.Parent.SmallKey;
                        tree.Parent.SmallKey = sibling.LargeKey;
                        sibling.LargeKey = null;
                    }
                    break;
                case ChildPosition.MIDDLE:
                    if (siblingSelfCount == 1)
                    {
                        tree.SmallKey = tree.Parent.SmallKey;
                        tree.LargeKey = sibling.SmallKey;
                        tree.Parent.SmallKey = null;
                        tree.Parent.Middle = null;
                    }
                    else
                    {
                        tree.SmallKey = tree.Parent.SmallKey;
                        tree.Parent.SmallKey = sibling.SmallKey;
                        sibling.SmallKey = sibling.LargeKey;
                        sibling.LargeKey = null;
                    }
                    break;
            }
        }
        else
        {
            // Parent has two key
            switch (siblingPosition)
            {
                default:
                case ChildPosition.NOPARENT:
                    throw new InvalidOperationException();
                case ChildPosition.LEFT:
                    if (siblingSelfCount == 1)
                    {
                        tree.SmallKey = parent.LargeKey;
                        tree.LargeKey = parent.Right!.SmallKey;
                        
                        parent.LargeKey = null;
                        parent.Right = null;
                    }
                    else
                    {
                        tree.SmallKey = parent.SmallKey;
                        parent.SmallKey = sibling.LargeKey;

                        sibling.LargeKey = null;
                    }
                    break;
                case ChildPosition.MIDDLE:
                    if (selfPosition == ChildPosition.LEFT)
                    {
                        if (siblingSelfCount == 1)
                        {
                            tree.SmallKey = parent.SmallKey;
                            parent.SmallKey = sibling.SmallKey;
                            sibling.SmallKey = parent.LargeKey;
                            sibling.LargeKey = parent.Right!.SmallKey;

                            parent.LargeKey = null;
                            parent.Right = null;
                        }
                        else
                        {
                            tree.SmallKey = parent.SmallKey;
                            parent.SmallKey = sibling.SmallKey;
                            sibling.SmallKey = sibling.LargeKey;
                            sibling.LargeKey = null;
                        }
                    }
                    else
                    {
                        if (siblingSelfCount == 1)
                        {
                            sibling.LargeKey = parent.LargeKey;
                            
                            parent.LargeKey = null;
                            parent.Right = null;
                        }
                        else
                        {
                            tree.SmallKey = parent.LargeKey;
                            parent.LargeKey = sibling.LargeKey;
                            
                            sibling.LargeKey = null;
                        }
                    }
                    break;
                case ChildPosition.RIGHT:
                    if (siblingSelfCount == 1)
                    {
                        tree.SmallKey = parent.LargeKey;
                        tree.LargeKey = sibling.SmallKey;

                        parent.LargeKey = null;
                        parent.Right = null;
                    }
                    else
                    {
                        tree.SmallKey = parent.LargeKey;
                        parent.LargeKey = sibling.SmallKey;
                        sibling.SmallKey = sibling.LargeKey;
                        sibling.LargeKey = null;
                    }
                    break;
            }
        }
            
        RearrangeTree(parent);
    }

    enum ChildPosition
    {
        NOPARENT,
        LEFT,
        MIDDLE,
        RIGHT
    }

    private ChildPosition GetPosition()
    {
        if (Parent == null)
        {
            return ChildPosition.NOPARENT;
        }

        if (this == Parent.Left)
        {
            return ChildPosition.LEFT;
        }

        if (this == Parent.Middle)
        {
            return ChildPosition.MIDDLE;
        }

        if (this == Parent.Right)
        {
            return ChildPosition.RIGHT;
        }

        throw new InvalidOperationException();
    }
    
    public Tree23<TOrder, TValue> FindLeafNodeWith(TOrder order)
    {
        int compare1 = SmallKey != null ? order.CompareTo(SmallKey!.Value.Item1) : -1;
        int compare2 = LargeKey != null ? order.CompareTo(LargeKey.Value.Item1) : -1;

        if (compare1 == 0 || compare2 == 0)
        {
            if (IsLeafNode)
            {
                return this;
            }
            
            throw new InvalidOperationException();
        } else if (compare1 < 0)
        {
            return Left!.FindLeafNodeWith(order);
        } else if (compare2 < 0)
        {
            return Middle!.FindLeafNodeWith(order);
        } else
        {
            return Right!.FindLeafNodeWith(order);
        }
    }
    public override TValue? GetValue(TOrder order)
    {
        return ((Tree23<TOrder, TValue>)GetTree(order)).GetSelfValue(order);
    }
    
    public TValue? GetSelfValue(TOrder order)
    {
        int compare1 = SmallKey != null ? order.CompareTo(SmallKey.Value.Item1) : -1;
        int compare2 = LargeKey != null ? order.CompareTo(LargeKey.Value.Item1) : -1;
        
        if (compare1 == 0)
        {
            return SmallKey!.Value.Item2;
        } else if (compare2 == 0)
        {
            return LargeKey!.Value.Item2;
        }

        throw new InvalidOperationException();
    }

    public override void Clear()
    {
        SmallKey = null;
        LargeKey = null;

        Parent = null;
        Left = null;
        Middle = null;
        Right = null;
    }

    public int SelfCount()
    {
        int count = 0;

        count += SmallKey != null ? 1 : 0;
        count += LargeKey != null ? 1 : 0;

        return count;
    }
    
    public override int Count
    {
        get
        {
            int count = 0;

            count += SelfCount();
            count += Left?.Count() ?? 0;
            count += Middle?.Count() ?? 0;
            count += Right?.Count() ?? 0;

            return count;
        }
    }

    public override bool IsValidSorted()
    {
        TOrder? order = default;

        return !Inorder((treeOrder, treeValue) =>
        {
            if (order == null)
            {
                order = treeOrder;
            }
            else
            {
                if (order.CompareTo(treeOrder) >= 0)
                {
                    return true;
                }
            }

            return false;
        });
    }

    public bool Inorder(Func<TOrder, TValue?, bool> action)
    {
        if (Left != null && Left.Inorder(action))
        {
            return true;
        }

        if (SmallKey != null && action(SmallKey.Value.Item1, SmallKey.Value.Item2))
        {
            return true;
        }
        
        if (Middle != null && Middle.Inorder(action))
        {
            return true;
        }
        
        if (LargeKey != null && action(LargeKey.Value.Item1, LargeKey.Value.Item2))
        {
            return true;
        }
        
        if (Right != null && Right.Inorder(action))
        {
            return true;
        }

        return false;
    }
}