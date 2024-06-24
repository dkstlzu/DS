namespace CSLibrary;

public class AVLTree<TOrder, TValue> where TOrder : IComparable<TOrder>
{
    private (TOrder, TValue?)?[] _elements;
    private int _capacity;
    private int _count;

    public AVLTree(int initialCapacity)
    {
        _capacity = CeilingPowerOfTwo(initialCapacity);
        _elements = new (TOrder, TValue?)?[_capacity];
    }

    private int CeilingPowerOfTwo(int value)
    {
        int result = 1;

        while (result < value)
        {
            result *= 2;
        }

        return result;
    }

    private int LeftChildIndex(int index) => index * 2 + 1;
    private int RightChildIndex(int index) => index * 2 + 2;
    private int ParentIndex(int index) => index / 2;

    /// <summary>
    /// Depth from root
    /// </summary>
    private int Depth(int index)
    {
        index++;
        int height = 0;

        while (index > 1)
        {
            index >>= 1;
            height++;
        }

        return height;
    }

    /// <summary>
    /// Height from leaf
    /// </summary>
    private int Height(int index)
    {
        if (_elements[index] == null)
        {
            return -1;
        }
        
        int leftChildIndex = LeftChildIndex(index);
        int rightChildIndex = RightChildIndex(index);
        
        if (leftChildIndex >= _capacity
            || (_elements[leftChildIndex] == null && _elements[rightChildIndex] == null))
        {
            return 0;
        }

        return Math.Max(Height(leftChildIndex), Height(rightChildIndex)) + 1;
    }

    private bool NeedBalancing()
    {
        return Math.Abs(GetBalanceFactor()) > 1;
    }

    private int GetBalanceFactor()
    {
        int leftTreeIndex = 1;
        int rightTreeIndex = 2;

        return Height(leftTreeIndex) - Height(rightTreeIndex);
    }

    private void Resize(int newCapacity)
    {
        newCapacity = CeilingPowerOfTwo(newCapacity);
        if (newCapacity == _capacity)
        {
            return;
        }

        (TOrder, TValue?)?[] newElements = new (TOrder, TValue?)?[newCapacity];
        int loopLength = Math.Min(_capacity, newCapacity);

        for (int i = 0; i < loopLength; i++)
        {
            newElements[i] = _elements[i];
        }

        _elements = newElements;
    }

    private void ReBalanceLeaf(int index)
    {
        bool parentIsRight = index % 2 == 1;
        bool grandParentIsRight = (index / 2) % 2 == 1;

        int parentIndex = ParentIndex(index);
        int grandParentIndex = ParentIndex(parentIndex);

        if (parentIsRight && grandParentIsRight)
        {
            _elements[RightChildIndex(grandParentIndex)] = _elements[grandParentIndex]!.Value;
            _elements[grandParentIndex] = _elements[parentIndex];
            _elements[parentIndex] = _elements[index];
        } else if (parentIsRight && !grandParentIsRight)
        {
            _elements[LeftChildIndex(grandParentIndex)] = _elements[grandParentIndex];
            _elements[grandParentIndex] = _elements[index];
        } else if (!parentIsRight && grandParentIsRight)
        {
            _elements[RightChildIndex(grandParentIndex)] = _elements[grandParentIndex];
            _elements[grandParentIndex] = _elements[index];
        } else if (!parentIsRight && !grandParentIsRight)
        {
            _elements[LeftChildIndex(grandParentIndex)] = _elements[grandParentIndex];
            _elements[grandParentIndex] = _elements[parentIndex];
            _elements[parentIndex] = _elements[index];
        }
        
        _elements[index] = null;
    }

    private void ReBalanceRoot()
    {
        int rootIndex = 0;
        int leftTreeIndex = 1;
        int rightTreeIndex = 2;
        
        if (GetBalanceFactor() < 0)
        {
            // right tree is higher
            
            // left child tree shift
            ShiftToLeftChild(leftTreeIndex, leftTreeIndex);
            
            // move root to previous left position
            _elements[leftTreeIndex] = _elements[rootIndex];

            // find the smallest value among bigger values than root
            int smallestBiggerThanRoot_Index = rightTreeIndex;

            while (_elements[LeftChildIndex(smallestBiggerThanRoot_Index)] != null)
            {
                smallestBiggerThanRoot_Index = LeftChildIndex(smallestBiggerThanRoot_Index);
            }

            // set new root value with found
            _elements[rootIndex] = _elements[smallestBiggerThanRoot_Index];
            
            // fill the empty space with right children
            ShiftToLeftParent(RightChildIndex(smallestBiggerThanRoot_Index), RightChildIndex(smallestBiggerThanRoot_Index));
        }
        else
        {
            // left tree is higher
            
            // right child tree shift
            ShiftToRightChild(rightTreeIndex, rightTreeIndex);
            
            // move root to previous right position
            _elements[leftTreeIndex] = _elements[rootIndex];
            
            // find the biggest value among smaller values than root
            int biggestSmallerThanRoot_Index = rightTreeIndex;

            while (_elements[RightChildIndex(biggestSmallerThanRoot_Index)] != null)
            {
                biggestSmallerThanRoot_Index = RightChildIndex(biggestSmallerThanRoot_Index);
            }
            
            // set new root value with found
            _elements[rootIndex] = _elements[biggestSmallerThanRoot_Index];
            
            // fill the empty space with right children
            ShiftToRightParent(LeftChildIndex(biggestSmallerThanRoot_Index), LeftChildIndex(biggestSmallerThanRoot_Index));
        }
    }

    private void ShiftToLeftChild(int index,  int root)
    {
        if (_elements[index] == null)
        {
            return;
        }

        ShiftToLeftChild(LeftChildIndex(index), root);
        ShiftToLeftChild(RightChildIndex(index), root);
        
        _elements[RelativeIndex(index, root, LeftChildIndex(root))] = _elements[index];
    }

    private void ShiftToRightChild(int index, int root)
    {
        if (_elements[index] == null)
        {
            return;
        }
        
        ShiftToRightChild(LeftChildIndex(index), root);
        ShiftToRightChild(RightChildIndex(index), root);
        
        _elements[RelativeIndex(index, root, RightChildIndex(root))] = _elements[index];
    }

    private void ShiftToLeftParent(int index, int root)
    {
        if (_elements[index] == null)
        {
            return;
        }

        _elements[RelativeIndex(index, root, ParentIndex(root))] = _elements[index];
        
        ShiftToLeftParent(LeftChildIndex(index), root);
        ShiftToLeftParent(RightChildIndex(index), root);
    }

    private void ShiftToRightParent(int index, int root)
    {
        if (_elements[index] == null)
        {
            return;
        }
        
        _elements[RelativeIndex(index, root, ParentIndex(root))] = _elements[index];

        ShiftToRightParent(LeftChildIndex(index), root);
        ShiftToRightParent(RightChildIndex(index), root);
    }

    private int RelativeIndex(int index, int root, int newRoot)
    {
        Stack<bool> toRight = new Stack<bool>();
        
        while (index > root)
        {
            toRight.Push(index % 2 == 0);
            index = ParentIndex(index);
        }

        while (toRight.Count > 0)
        {
            if (toRight.Pop())
            {
                newRoot = RightChildIndex(newRoot);
            }
            else
            {
                newRoot = LeftChildIndex(newRoot);
            }
        }

        return newRoot;
    }

    public void Add(TOrder order, TValue? value)
    {
        int index = 0;

        while (_elements[index] != null)
        {
            if (order.CompareTo(_elements[index]!.Value.Item1) < 0)
            {
                index = index * 2 + 1;
            } else if (order.CompareTo(_elements[index]!.Value.Item1) > 0)
            {
                index = index * 2 + 2;
            }
            else
            {
                throw new InvalidOperationException();
            }

            if (index >= _capacity)
            {
                Resize(_capacity * 2);
            }
        }

        _elements[index] = (order, value);

        if (NeedBalancing())
        {
            ReBalanceLeaf(index);   
        }
    }

    public void Remove(TOrder order)
    {
        int index = 0;

        while (_elements[index] != null)
        {
            if (order.CompareTo(_elements[index]!.Value.Item1) < 0)
            {
                index = index * 2 + 1;
            } else if (order.CompareTo(_elements[index]!.Value.Item1) > 0)
            {
                index = index * 2 + 2;
            }
            else
            {
                break;
            }
        }

        if (_elements[index] == null)
        {
            throw new InvalidOperationException();
        }
        
        _elements[index] = null;

        if (NeedBalancing())
        {
            ReBalanceRoot();
        }
    }

    public TValue? GetValue(TOrder order)
    {
        if (IsEmpty())
        {
            throw new InvalidOperationException();
        }
        
        int index = 0;
        int compare;

        do
        {
            compare = order.CompareTo(_elements[index]!.Value.Item1);
            if (compare == 0)
            {
                return _elements[index]!.Value.Item2;
            } else if (compare < 0)
            {
                index = LeftChildIndex(index);
            }
            else if (compare > 0)
            {
                index = RightChildIndex(index);
            }

            if (_elements[index] == null)
            {
                return default;
            }
        } while (true);
    }

    public void Clear()
    {
        _elements = new (TOrder, TValue?)?[_capacity];
    }

    public int Count()
    {
        return _count;
    }

    public bool IsEmpty()
    {
        return Count() == 0;
    }
}