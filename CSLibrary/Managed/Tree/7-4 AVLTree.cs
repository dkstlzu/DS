using System.Collections;

namespace CSLibrary;

public class AVLTreeNode<T> where T : IComparable<T>
{
    public AVLTreeNode<T>? Parent;
    public AVLTreeNode<T>? Left;
    public AVLTreeNode<T>? Right;
    public T? Value;
    public bool IsLeaf => Left == null && Right == null;

    public AVLTreeNode(T value)
    {
        Value = value;
    }
}

public class AVLTree<T> : ITree<T>, ITreeTraversal<T>, IEnumerable<T> where T : IComparable<T>
{
    public AVLTreeNode<T>? Root;
    private int _count;

    public AVLTree()
    {
        
    }

    public AVLTree(AVLTreeNode<T>? root)
    {
        int GetCount(AVLTreeNode<T>? node)
        {
            if (node == null)
            {
                return 0;
            }
            
            return GetCount(node.Left) + GetCount(node.Right) + 1;
        }
        
        if (Root != null)
        {
            Root = root;
            _count = GetCount(root);
        }
    }
    
        public int GetStableHeight()
    {
        int height = -1;

        int heightCount = _count + 1;

        while (heightCount > 0)
        {
            heightCount >>= 1;
            height++;
        }

        // 마지막에 한번 더 연산됨
        return height - 1;
    }

    private int GetHeight(AVLTreeNode<T>? node)
    {
        if (node == null)
        {
            return -1;
        }
        
        return Math.Max(GetHeight(node.Left), GetHeight(node.Right)) + 1;
    }

    private bool TryBalance(AVLTreeNode<T> node)
    {
        int leftHeight = GetHeight(node.Left);
        int rightHeight = GetHeight(node.Right);
        
        if (Math.Abs(leftHeight - rightHeight) < 2)
        {
            return false;
        }

        if (leftHeight > rightHeight)
        {
            if (GetHeight(node.Left.Left) > GetHeight(node.Left.Right))
            {
                TurnRight1(node);
            }
            else
            {
                TurnRight2(node);
            }
        }
        else
        {
            if (GetHeight(node.Right.Left) < GetHeight(node.Right.Right))
            {
                TurnLeft1(node);
            }
            else
            {
                TurnLeft2(node);
            }
        }

        return true;
    }
    
    /// <summary>
    /// \  모양을 /\ 로 바꿈 <br/>
    ///  \
    /// </summary>
    private void TurnLeft1(AVLTreeNode<T> node)
    {
        AVLTreeNode<T>? parent = node.Parent;
        
        AVLTreeNode<T> node1 = node;
        AVLTreeNode<T> node2 = node.Right!;
        AVLTreeNode<T> node3 = node.Right!.Right!;
        
        AVLTreeNode<T> nodeA = node.Left!;
        AVLTreeNode<T> nodeB = node.Right!.Left!;
        AVLTreeNode<T> nodeC = node.Right!.Right!.Left!;
        AVLTreeNode<T> nodeD = node.Right!.Right!.Right!;

        if (parent != null)
        {
            if (node == parent.Left)
            {
                parent.Left = node2;
            }
            else
            {
                parent.Right = node2;
            }
        }

        node2.Left = node1;
        node1.Right = nodeB;
    }
    
    /// <summary>
    /// \  모양을 /\ 로 바꿈 <br/>
    /// /
    /// </summary>
    private void TurnLeft2(AVLTreeNode<T> node)
    {
        AVLTreeNode<T>? parent = node.Parent;

        AVLTreeNode<T> node1 = node;
        AVLTreeNode<T> node2 = node.Right!;
        AVLTreeNode<T> node3 = node.Right!.Left!;
        
        AVLTreeNode<T> nodeA = node.Left!;
        AVLTreeNode<T> nodeB = node.Right!.Left!.Left!;
        AVLTreeNode<T> nodeC = node.Right!.Left!.Right!;
        AVLTreeNode<T> nodeD = node.Right!.Right!;

        if (parent != null)
        {
            if (node == parent.Left)
            {
                parent.Left = node3;
            }
            else
            {
                parent.Right = node3;
            }
        }

        node1.Right = nodeB;
        node2.Left = nodeC;
        node3.Left = node1;
        node3.Right = node2;
    }
    
    /// <summary>
    ///  / 모양을 /\ 로 바꿈 <br/>
    /// / 
    /// </summary>
    private void TurnRight1(AVLTreeNode<T> node)
    {
        AVLTreeNode<T>? parent = node.Parent;

        AVLTreeNode<T> node1 = node;
        AVLTreeNode<T> node2 = node.Left!;
        AVLTreeNode<T> node3 = node.Left!.Left!;
        
        AVLTreeNode<T> nodeA = node.Left!.Left!.Left!;
        AVLTreeNode<T> nodeB = node.Left!.Left!.Right!;
        AVLTreeNode<T> nodeC = node.Left!.Right!;
        AVLTreeNode<T> nodeD = node.Right!;

        if (parent != null)
        {
            if (node == parent.Left)
            {
                parent.Left = node2;
            }
            else
            {
                parent.Right = node2;
            }
        }

        node1.Left = nodeC;
        node2.Right = node1;
    }
    
    /// <summary>
    ///  / 모양을 /\ 로 바꿈 <br/>
    ///  \
    /// </summary>
    private void TurnRight2(AVLTreeNode<T> node)
    {
        AVLTreeNode<T>? parent = node.Parent;

        AVLTreeNode<T> node1 = node;
        AVLTreeNode<T> node2 = node.Left!;
        AVLTreeNode<T> node3 = node.Left!.Right!;
        
        AVLTreeNode<T> nodeA = node.Left!.Left!;
        AVLTreeNode<T> nodeB = node.Left!.Right!.Left!;
        AVLTreeNode<T> nodeC = node.Left!.Right!.Right!;
        AVLTreeNode<T> nodeD = node.Right!;

        if (parent != null)
        {
            if (node == parent.Left)
            {
                parent.Left = node3;
            }
            else
            {
                parent.Right = node3;
            }
        }

        node2.Right = nodeB;
        node1.Left = nodeC;
        node3.Left = node1;
        node3.Right = node2;
    }
    
    #region ITree

    public void Clear()
    {
        Root = null;
        _count = 0;
    }

    public int Count() => _count;
    public bool IsEmpty() => Count() == 0;

    public void Insert(T item)
    {
        if (Root == null)
        {
            Root = new AVLTreeNode<T>(item);
            _count++;
            return;
        }
        
        AVLTreeNode<T> node = Root;

        while (true)
        {
            int compare = item.CompareTo(node.Value);

            if (compare == 0)
            {
                throw new InvalidOperationException("이미 있는데 넣을라그랬음");
            }

            if (compare < 0)
            {
                if (node.Left == null)
                {
                    node.Left = new AVLTreeNode<T>(item);
                    _count++;
                    break;
                }
                else
                {
                    node = node.Left;
                }
            }

            if (compare > 0)
            {
                if (node.Right == null)
                {
                    node.Right = new AVLTreeNode<T>(item);
                    _count++;
                    break;
                }
                else
                {
                    node = node.Right;
                }
            }
        }

        while (TryBalance(node))
        {
            if (node.Parent == null)
            {
                break;
            }
            
            node = node.Parent;
        }
    }

    public void Remove(T item)
    {
        if (Root == null)
        {
            throw new InvalidOperationException("없는데 지울려그랬음");
        }

        if (item.CompareTo(Root!.Value) == 0)
        {
            Root = null;
            _count = 0;
            return;
        }
        
        AVLTreeNode<T> node = Root;

        while (true)
        {
            int compare = item.CompareTo(node.Value);

            if (compare == 0)
            {
                if (node.Parent.Left == node)
                {
                    node.Parent.Left = null;
                }
                else
                {
                    node.Parent.Right = null;
                }
                
                node = node.Parent;
                _count--;
                break;
            }
            
            if (compare < 0)
            {
                if (node.Left == null)
                {
                    throw new InvalidOperationException("없는데 지울려그랬음");
                }

                node = node.Left;
            }

            if (compare > 0)
            {
                if (node.Right == null)
                {
                    throw new InvalidOperationException("없는데 지울려그랬음");
                }
                
                node = node.Right;
            }
        }

        while (TryBalance(node))
        {
            if (node.Parent == null)
            {
                break;
            }
            
            node = node.Parent;
        }
    }

    public bool Contains(T item)
    {
        if (Root == null)
        {
            return false;
        }

        AVLTreeNode<T> node = Root;
        
        while (true)
        {
            int compare = item.CompareTo(node.Value);

            if (compare == 0)
            {
                return true;
            }

            if (compare < 0)
            {
                if (node.Left == null)
                {
                    return false;
                }
                else
                {
                    node = node.Left;
                }
            }

            if (compare > 0)
            {
                if (node.Right == null)
                {
                    return false;
                }
                else
                {
                    node = node.Right;
                }
            }
        }
    }
    
    #endregion ITree

    #region ITreeTraversal

    public void PreorderTraversal(Func<T?, bool> action)
    {
        if (Root == null)
        {
            return;
        }
        
        Stack<AVLTreeNode<T>> stack = new Stack<AVLTreeNode<T>>();
        stack.Push(Root);

        while (stack.Count > 0)
        {
            AVLTreeNode<T> node = stack.Pop();
            
            if (action(node.Value))
            {
                return;
            }

            if (node.Right != null)
            {
                stack.Push(node.Right);
            }
            
            if (node.Left != null)
            {
                stack.Push(node.Left);
            }
        }
    }

    public void InorderTraversal(Func<T?, bool> action)
    {
        if (Root == null)
        {
            return;
        }
        
        Stack<AVLTreeNode<T>> stack = new Stack<AVLTreeNode<T>>();
        stack.Push(Root);

        while (stack.Count > 0)
        {
            AVLTreeNode<T> node = stack.Pop();

            if (node.Right != null)
            {
                stack.Push(node.Right);
            }
            
            if (node.Left != null)
            {
                stack.Push(node);
                stack.Push(node.Left);
            }
            else if (action(node.Value))
            {
                return;
            }
        }
    }

    public void PostorderTraversal(Func<T?, bool> action)
    {
        if (Root == null)
        {
            return;
        }
        
        Stack<AVLTreeNode<T>> stack = new Stack<AVLTreeNode<T>>();
        stack.Push(Root);

        while (stack.Count > 0)
        {
            AVLTreeNode<T> node = stack.Pop();

            if (node.Left == null && node.Right == null)
            {
                if (action(node.Value))
                {
                    return;
                }
            }
            
            stack.Push(node);
            
            if (node.Right != null)
            {
                stack.Push(node.Right);
            } else if (node.Left != null)
            {
                stack.Push(node.Left);
            }
        }
    }

    #endregion
    
    #region IEnumerable

    public IEnumerator<T> GetEnumerator()
    {
        return new Enumerator(this);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    class Enumerator : IEnumerator<T>
    {
        private List<T?> _nodeList;
        private int _index;
        private T? _current;
        
        public Enumerator(AVLTree<T> tree)
        {
            _nodeList = new List<T?>();
            
            tree.InorderTraversal((t) =>
            {
                _nodeList.Add(t);
                return false;
            });
        }

        public bool MoveNext()
        {
            if (_index < _nodeList.Count)
            {
                _current = _nodeList[_index++];
            }

            return false;
        }

        public void Reset()
        {
            _index = 0;
        }

#pragma warning disable CS8766 // 반환 형식에서 참조 형식의 null 허용 여부가 암시적으로 구현된 멤버와 일치하지 않음(null 허용 여부 특성 때문일 수 있음)
        public T? Current => _current;
#pragma warning restore CS8766 // 반환 형식에서 참조 형식의 null 허용 여부가 암시적으로 구현된 멤버와 일치하지 않음(null 허용 여부 특성 때문일 수 있음)

        object? IEnumerator.Current => Current;
        
        public void Dispose()
        {
            // 할거 없음
        }
    }

    #endregion IEnumerable

}