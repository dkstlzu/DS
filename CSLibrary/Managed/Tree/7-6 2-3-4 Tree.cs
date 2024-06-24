// using System.Diagnostics;
//
// namespace CSLibrary;
//
// public class Tree234<TOrder, TValue> where TOrder : IComparable<TOrder>
// {
//     public const int MAX_ELEMENT = 3;
//     
//     public (TOrder, TValue?)? SmallKey;
//     public (TOrder, TValue?)? MiddleKey;
//     public (TOrder, TValue?)? LargeKey;
//
//     public Tree234<TOrder, TValue>? Parent;
//     public Tree234<TOrder, TValue>? Left;
//     public Tree234<TOrder, TValue>? MiddleLeft;
//     public Tree234<TOrder, TValue>? MiddleRight;
//     /// <summary>
//     /// Use only when three elements are used
//     /// </summary>
//     public Tree234<TOrder, TValue>? Right;
//
//     public bool IsLeafNode;
//     
//     public Tree234()
//     {
//         IsLeafNode = true;
//     }
//
//     public Tree234(bool isLeafNode)
//     {
//         IsLeafNode = isLeafNode;
//     }
//
//     public Tree234<TOrder, TValue> Add(TOrder order, TValue? value)
//     {
//         FindLeafNodeWith(order).AddFromLeaf(order, value, null, null);
//
//         var node = this;
//
//         while (node.Parent != null)
//         {
//             node = node.Parent;
//         }
//
//         return node;
//     }
//
//     private void AddFromLeaf(TOrder order, TValue? value, 
//         Tree234<TOrder, TValue>? childNode, Tree234<TOrder, TValue>? splittedChildNode)
//     {
//         switch (SelfCount())
//         {
//             default:
//                 throw new InvalidOperationException();
//             case 0:
//                 SmallKey = (order, value);
//                 return;
//             case 1:
//                 if (order.CompareTo(SmallKey!.Value.Item1) < 0)
//                 {
//                     LargeKey = SmallKey;
//                     SmallKey = (order, value);
//                 }
//                 else
//                 {
//                     LargeKey = (order, value);
//                 }
//
//                 if (IsLeafNode)
//                 {
//                     return;
//                 }
//                 
//                 if (childNode == Left)
//                 {
//                     MiddleRight = MiddleLeft;
//                     MiddleLeft = splittedChildNode;
//
//                 } else if (childNode == MiddleLeft)
//                 {
//                     MiddleRight = splittedChildNode;
//                 }
//                 else
//                 {
//                     throw new InvalidOperationException();
//                 }
//                 
//                 splittedChildNode!.Parent = this;
//                 
//                 return;
//             case 2:
//                 (order, value) = Insert(order, value);
//
//                 if (IsLeafNode)
//                 {
//                     break;
//                 }
//                 
//                 Debug.Assert(splittedChildNode != null);
//                 
//                 var splittedNode = new Tree234<TOrder, TValue>(false);
//                 splittedNode.SmallKey = LargeKey;
//                 LargeKey = null;
//             
//                 if (childNode == Left)
//                 {
//                     splittedNode.Left = MiddleLeft;
//                     splittedNode.Middle = MiddleRight;
//
//                     MiddleLeft = splittedChildNode;
//                     MiddleRight = null;
//                     splittedChildNode.Parent = this;
//                 } else if (childNode == MiddleLeft)
//                 {
//                     splittedNode.Left = splittedChildNode;
//                     splittedNode.Middle = MiddleRight;
//                     splittedChildNode.Parent = splittedNode;
//
//                     MiddleRight = null;
//                 } else if (childNode == MiddleRight)
//                 {
//                     splittedNode.Left = MiddleRight;
//                     splittedNode.Middle = splittedChildNode;
//                     splittedChildNode.Parent = splittedNode;
//
//                     MiddleRight = null;
//                 }
//
//                 if (Parent == null)
//                 {
//                     Parent = new Tree234<TOrder, TValue>(false);
//
//                     Parent.SmallKey = (order, value);
//
//                     Parent.Left = this;
//                     Parent.Middle = splittedNode;
//                     splittedNode.Parent = Parent;
//                 }
//                 else
//                 {
//                     Parent.AddFromLeaf(order, value, this, splittedNode);
//                 }
//                 
//                 break;
//         }
//     }
//     
//         
//     /// <returns>Key value that should go up node</returns>
//     private (TOrder, TValue?) Insert(TOrder order, TValue? value)
//     {
//         if (SelfCount() < MAX_ELEMENT)
//         {
//             throw new InvalidOperationException();
//         }
//         
//         if (order.CompareTo(SmallKey!.Value.Item1) < 0)
//         {
//             var temp = SmallKey;
//             SmallKey = (order, value);
//             return temp.Value;
//         }
//         
//         if (order.CompareTo(LargeKey!.Value.Item1) < 0)
//         {
//             return (order, value);
//         } 
//         
//         if (order.CompareTo(LargeKey.Value.Item1) > 0)
//         {
//             var temp = LargeKey;
//             LargeKey = (order, value);
//             return temp.Value;
//         }
//         
//         throw new InvalidOperationException();
//     }
//
//     public Tree234<TOrder, TValue> Remove(TOrder order)
//     {
//         var node = FindNodeWith(order);
//
//         if (node.RemoveAndEmpty(order))
//         {
//             RearrangeTree(node);
//         }
//
//         while (node.Parent != null)
//         {
//             if (node == this)
//             {
//                 return this;
//             }
//             
//             node = node.Parent;
//         }
//
//         return node;
//     }
//
//     /// <returns>true when removed all elements</returns>
//     public bool RemoveAndEmpty(TOrder order)
//     {
//         if (SmallKey != null && order.CompareTo(SmallKey.Value.Item1) == 0)
//         {
//             SmallKey = LargeKey;
//             return LargeKey == null;
//         }
//         
//         if (LargeKey != null && order.CompareTo(LargeKey.Value.Item1) == 0)
//         {
//             LargeKey = null;
//             return false;
//         }
//
//         throw new InvalidOperationException();
//     }
//
//     private ChildPosition GetSibling(out Tree234<TOrder, TValue> sibling)
//     {
//         Debug.Assert(Parent != null);
//         
//         if (this == Parent.Left)
//         {
//             sibling = Parent.Middle!;
//             return ChildPosition.MIDDLE;
//         } else if (this == Parent.Middle)
//         {
//             if (Parent.Right != null)
//             {
//                 if (Parent.Left!.SelfCount() >= Parent.Right.SelfCount())
//                 {
//                     sibling = Parent.Left;
//                     return ChildPosition.LEFT;
//                 }
//                 else
//                 {
//                     sibling = Parent.Right;
//                     return ChildPosition.RIGHT;
//                 }
//             }
//
//             sibling = Parent.Left!;
//             return ChildPosition.LEFT;
//         } else /*if (this == Parent.Right)*/
//         {
//             sibling = Parent.Middle!;
//             return ChildPosition.MIDDLE;
//         }
//     }
//
//     private static void RearrangeTree(Tree234<TOrder, TValue> tree)
//     {
//         if (tree.Parent == null)
//         {
//             return;
//         }
//
//         ChildPosition selfPosition = tree.GetPosition();
//         var parent = tree.Parent;
//         int parentSelfCount = parent.SelfCount();
//         ChildPosition siblingPosition = tree.GetSibling(out var sibling);
//         int siblingSelfCount = sibling.SelfCount();
//
//         // tree.Parent has one key
//         if (parentSelfCount == 1)
//         {
//             switch (siblingPosition)
//             {
//                 default:
//                 case ChildPosition.NOPARENT:
//                 case ChildPosition.RIGHT:
//                     throw new InvalidOperationException();
//                 case ChildPosition.LEFT:
//                     if (siblingSelfCount == 1)
//                     {
//                         sibling.LargeKey = tree.Parent.SmallKey;
//                         tree.Parent.SmallKey = null;
//                         tree.Parent.Middle = null;
//                     }
//                     else
//                     {
//                         tree.SmallKey = tree.Parent.SmallKey;
//                         tree.Parent.SmallKey = sibling.LargeKey;
//                         sibling.LargeKey = null;
//                     }
//                     break;
//                 case ChildPosition.MIDDLE:
//                     if (siblingSelfCount == 1)
//                     {
//                         tree.SmallKey = tree.Parent.SmallKey;
//                         tree.LargeKey = sibling.SmallKey;
//                         tree.Parent.SmallKey = null;
//                         tree.Parent.Middle = null;
//                     }
//                     else
//                     {
//                         tree.SmallKey = tree.Parent.SmallKey;
//                         tree.Parent.SmallKey = sibling.SmallKey;
//                         sibling.SmallKey = sibling.LargeKey;
//                         sibling.LargeKey = null;
//                     }
//                     break;
//             }
//         }
//         else
//         {
//             // Parent has two key
//             switch (siblingPosition)
//             {
//                 default:
//                 case ChildPosition.NOPARENT:
//                     throw new InvalidOperationException();
//                 case ChildPosition.LEFT:
//                     if (siblingSelfCount == 1)
//                     {
//                         tree.SmallKey = parent.LargeKey;
//                         tree.LargeKey = parent.Right!.SmallKey;
//                         
//                         parent.LargeKey = null;
//                         parent.Right = null;
//                     }
//                     else
//                     {
//                         tree.SmallKey = parent.SmallKey;
//                         parent.SmallKey = sibling.LargeKey;
//
//                         sibling.LargeKey = null;
//                     }
//                     break;
//                 case ChildPosition.MIDDLE:
//                     if (selfPosition == ChildPosition.LEFT)
//                     {
//                         if (siblingSelfCount == 1)
//                         {
//                             tree.SmallKey = parent.SmallKey;
//                             parent.SmallKey = sibling.SmallKey;
//                             sibling.SmallKey = parent.LargeKey;
//                             sibling.LargeKey = parent.Right!.SmallKey;
//
//                             parent.LargeKey = null;
//                             parent.Right = null;
//                         }
//                         else
//                         {
//                             tree.SmallKey = parent.SmallKey;
//                             parent.SmallKey = sibling.SmallKey;
//                             sibling.SmallKey = sibling.LargeKey;
//                             sibling.LargeKey = null;
//                         }
//                     }
//                     else
//                     {
//                         if (siblingSelfCount == 1)
//                         {
//                             sibling.LargeKey = parent.LargeKey;
//                             
//                             parent.LargeKey = null;
//                             parent.Right = null;
//                         }
//                         else
//                         {
//                             tree.SmallKey = parent.LargeKey;
//                             parent.LargeKey = sibling.LargeKey;
//                             
//                             sibling.LargeKey = null;
//                         }
//                     }
//                     break;
//                 case ChildPosition.RIGHT:
//                     if (siblingSelfCount == 1)
//                     {
//                         tree.SmallKey = parent.LargeKey;
//                         tree.LargeKey = sibling.SmallKey;
//
//                         parent.LargeKey = null;
//                         parent.Right = null;
//                     }
//                     else
//                     {
//                         tree.SmallKey = parent.LargeKey;
//                         parent.LargeKey = sibling.SmallKey;
//                         sibling.SmallKey = sibling.LargeKey;
//                         sibling.LargeKey = null;
//                     }
//                     break;
//             }
//         }
//             
//         if (siblingSelfCount == 1)
//         {
//             RearrangeTree(parent);
//         }
//     }
//
//     enum ChildPosition
//     {
//         NOPARENT,
//         LEFT,
//         MIDDLELEFT,
//         MIDDLERIGHT,
//         RIGHT
//     }
//
//     private ChildPosition GetPosition()
//     {
//         if (Parent == null)
//         {
//             return ChildPosition.NOPARENT;
//         }
//
//         if (this == Parent.Left)
//         {
//             return ChildPosition.LEFT;
//         }
//
//         if (this == Parent.MiddleLeft)
//         {
//             return ChildPosition.MIDDLELEFT;
//         }
//         
//         if (this == Parent.MiddleRight)
//         {
//             return ChildPosition.MIDDLERIGHT;
//         }
//
//         if (this == Parent.Right)
//         {
//             return ChildPosition.RIGHT;
//         }
//
//         throw new InvalidOperationException();
//     }
//     
//     public Tree234<TOrder, TValue> FindLeafNodeWith(TOrder order)
//     {
//         int compare1 = SmallKey != null ? order.CompareTo(SmallKey!.Value.Item1) : -1;
//         int compare2 = MiddleKey != null ? order.CompareTo(MiddleKey.Value.Item1) : -1;
//         int compare3 = LargeKey != null ? order.CompareTo(LargeKey.Value.Item1) : -1;
//
//         if (compare1 == 0 || compare2 == 0 || compare3 == 0)
//         {
//             if (IsLeafNode)
//             {
//                 return this;
//             }
//             
//             throw new InvalidOperationException();
//         } else if (compare1 < 0)
//         {
//             return Left!.FindLeafNodeWith(order);
//         } else if (compare2 < 0)
//         {
//             return MiddleLeft!.FindLeafNodeWith(order);
//         } else if (compare3 < 0)
//         {
//             return MiddleRight!.FindLeafNodeWith(order);
//         }
//         else
//         {
//             return Right!.FindLeafNodeWith(order);
//         }
//     }
//     
//     public Tree234<TOrder, TValue> FindNodeWith(TOrder order)
//     {
//         int compare1 = SmallKey != null ? order.CompareTo(SmallKey.Value.Item1) : -1;
//         int compare2 = MiddleKey != null ? order.CompareTo(MiddleKey.Value.Item1) : -1;
//         int compare3 = LargeKey != null ? order.CompareTo(LargeKey.Value.Item1) : -1;
//         
//         if (compare1 == 0 || compare2 == 0 || compare3 == 0)
//         {
//             return this;
//         }
//
//         if (IsLeafNode)
//         {
//             throw new InvalidOperationException();
//         } else if (compare1 < 0)
//         {
//             return Left!.FindNodeWith(order);
//         } else if (compare2 < 0)
//         {
//             return MiddleLeft!.FindNodeWith(order);
//         } else if (compare3 < 0)
//         {
//             return MiddleRight!.FindNodeWith(order);
//         }
//         else
//         {
//             return Right!.FindNodeWith(order);
//         }
//     }
//     
//     public TValue? GetValue(TOrder order)
//     {
//         return FindNodeWith(order).GetSelfValue(order);
//     }
//     
//     public TValue? GetSelfValue(TOrder order)
//     {
//         int compare1 = SmallKey != null ? order.CompareTo(SmallKey.Value.Item1) : -1;
//         int compare2 = MiddleKey != null ? order.CompareTo(MiddleKey.Value.Item1) : -1;
//         int compare3 = LargeKey != null ? order.CompareTo(LargeKey.Value.Item1) : -1;
//         
//         if (compare1 == 0)
//         {
//             return SmallKey!.Value.Item2;
//         } else if (compare2 == 0)
//         {
//             return MiddleKey!.Value.Item2;
//         } else if (compare3 == 0)
//         {
//             return LargeKey!.Value.Item2;
//         }
//
//         throw new InvalidOperationException();
//     }
//
//     public void Clear()
//     {
//         SmallKey = null;
//         MiddleKey = null;
//         LargeKey = null;
//
//         Parent = null;
//         Left = null;
//         MiddleLeft = null;
//         MiddleRight = null;
//         Right = null;
//     }
//
//     public int SelfCount()
//     {
//         int count = 0;
//
//         count += SmallKey != null ? 1 : 0;
//         count += MiddleKey != null ? 1 : 0;
//         count += LargeKey != null ? 1 : 0;
//
//         return count;
//     }
//     
//     public int Count()
//     {
//         int count = 0;
//
//         count += SelfCount();
//         count += Left?.Count() ?? 0;
//         count += MiddleLeft?.Count() ?? 0;
//         count += MiddleRight?.Count() ?? 0;
//         count += Right?.Count() ?? 0;
//
//         return count;
//     }
//
//     public bool IsEmpty()
//     {
//         return Count() == 0;
//     }
// }