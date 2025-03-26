using System.Collections;
using System.Diagnostics;

namespace CSLibrary;

public class BTreeNode<T> where T : IComparable<T>
{
    public BTreeNode<T>?[] Children;
    public T[] Values;
    public int Count;

    public BTreeNode(int valueCapacity)
    {
        Values = new T[valueCapacity];
        Children = new BTreeNode<T>[valueCapacity + 1];
    }

    public int GetChildIndexOf(T value)
    {
        for (int i = 0; i < Count; i++)
        {
            int compare = value.CompareTo(Values[i]);
            
            if (compare < 0)
            {
                return Children[i]!.GetChildIndexOf(value);
            } else if (compare == 0)
            {
                return -1;
            }
        }

        return Children[Count]!.GetChildIndexOf(value);
    }

    /// <summary>
    /// 이미 꽉찼으면 false
    /// </summary>
    public bool TryInsert(T item)
    {
        bool alreadyFull = Count >= Values.Length; 
        
        for (int i = 0; i < Count; i++)
        {
            int compare = item.CompareTo(Values[i]);

            if (compare == 0)
            {
                throw new InvalidOperationException("이미 있는걸 넣을라 그랬음");
            }
            
            if (compare > 0)
            {
                for (int j = Count - 1; j >= i; j--)
                {
                    
                }
            }
        }
            
        return alreadyFull;
    }

    public bool IsLeafNode()
    {
        for (int i = 0; i < Children.Length; i++)
        {
            if (Children[i] != null)
            {
                return false;
            }
        }
        
        return true;
    }
}

public class BTree<T> : ITree<T>, ITreeTraversal<T>, IEnumerable<T> where T : IComparable<T>
{
    public BTreeNode<T>? Root;
    public int ValueCapacity;
    private int _count;

    public BTree(int valueCapacity)
    {
        Debug.Assert(valueCapacity > 0);
        
        ValueCapacity = valueCapacity;
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
        if (IsEmpty())
        {
            return;
        }
    }

    public void Remove(T item)
    {
        if (IsEmpty())
        {
            return;
        }

        RemoveInternal(item, Root!);
    }

    private bool RemoveInternal(T item, BTreeNode<T> root)
    {
        for (int i = 0; i < root.Count; i++)
        {
            int compare = item.CompareTo(root.Values[i]);
            
            if (compare < 0)
            {
                if (root.Children[i] != null)
                {
                    return ContainsInternal(item, root.Children[i]);
                }
                else
                {
                    return false;
                }
            }

            if (compare == 0)
            {
                
                return true;
            }
        }

        if (root.Children[root.Count] != null)
        {
            return ContainsInternal(item, root.Children[root.Count]!);
        }

        return false;
    }

    public bool Contains(T item)
    {
        if (IsEmpty())
        {
            return false;
        }
        
        return ContainsInternal(item, Root!);
    }

    private bool ContainsInternal(T item, BTreeNode<T> root)
    {
        for (int i = 0; i < root.Count; i++)
        {
            int compare = item.CompareTo(root.Values[i]);
            
            if (compare < 0)
            {
                if (root.Children[i] != null)
                {
                    return ContainsInternal(item, root.Children[i]);
                }
                else
                {
                    return false;
                }
            }

            if (compare == 0)
            {
                return true;
            }
        }

        if (root.Children[root.Count] != null)
        {
            return ContainsInternal(item, root.Children[root.Count]!);
        }

        return false;
    }

    #endregion ITree

    #region ITreeTraversal

    public void PreorderTraversal(Func<T?, bool> action)
    {
        if (IsEmpty())
        {
            return;
        }

        PreorderTraversalInternal(action, Root!);
    }

    private bool PreorderTraversalInternal(Func<T?, bool> action, BTreeNode<T> node)
    {
        for (int i = 0; i < node.Count; i++)
        {
            if (action.Invoke(node.Values[i]))
            {
                return true;
            }
        }
        
        for (int i = 0; i < node.Count + 1; i++)
        {
            if (node.Children[i] == null)
            {
                continue;
            }
            
            if (PreorderTraversalInternal(action, node.Children[i]!))
            {
                return true;
            }
        }
        
        return false;
    }

    public void InorderTraversal(Func<T?, bool> action)
    {
        if (IsEmpty())
        {
            return;
        }
        
        InorderTraversalInternal(action, Root!);
    }

    private bool InorderTraversalInternal(Func<T?, bool> action, BTreeNode<T> node)
    {
        for (int i = 0; i < node.Count; i++)
        {
            if (node.Children[i] == null)
            {
                continue;
            }
            
            if (InorderTraversalInternal(action, node.Children[i]!))
            {
                return true;
            }
            
            if (action.Invoke(node.Values[i]))
            {
                return true;
            }
        }
        
        if (node.Children[node.Count] == null)
        {
            return false;
        }
        
        if (InorderTraversalInternal(action, node.Children[node.Count]!))
        {
            return true;
        }
        
        return false;
    }

    public void PostorderTraversal(Func<T?, bool> action)
    {
        if (IsEmpty())
        {
            return;
        }
        
        PostorderTraversalInternal(action, Root!);
    }

    private bool PostorderTraversalInternal(Func<T?, bool> action, BTreeNode<T> node)
    {
        for (int i = 0; i < node.Count + 1; i++)
        {
            if (node.Children[i] == null)
            {
                continue;
            }
            
            if (PostorderTraversalInternal(action, node.Children[i]!))
            {
                return true;
            }
        }

        for (int i = 0; i < node.Count; i++)
        {
            if (action.Invoke(node.Values[i]))
            {
                return true;
            }
        }

        return false;
    }

    #endregion ITreeTraversal

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
        
        public Enumerator(BTree<T> tree)
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

    #endregion
}