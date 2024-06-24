namespace CSLibrary;

public class RBTree<TOrder, TValue> where TOrder : IComparable<TOrder>
{
    
}

public class Map<TKey, TValue> where TKey : IComparable<TKey>
{
    // private RBTree<TKey, TValue> _rbTree;
    //
    // public Map()
    // {
    //     
    // }
    //
    // public TValue? this[TKey key]
    // {
    //     get
    //     {
    //         return _rbTree.GetValue(key);
    //     }
    //
    //     set
    //     {
    //         if (!_rbTree.Contains(key))
    //         {
    //             _rbTree.Add(key, value);
    //         }
    //         
    //         
    //     }
    // }
    //
    // public int Count() => _rbTree.Count();
    // public void Clear() => _rbTree.Clear();
}

