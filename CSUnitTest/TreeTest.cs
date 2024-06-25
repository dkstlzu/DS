using System.Text;
using CSLibrary;

namespace CSUnitTest;

public class TreeTest
{
    private const int ADD_LOOP_COUNT = 100;
    private const int REMOVE_LOOP_COUNT = 100;

    [Test]
    public void CompleteBinaryTreeTest() => ValidateTree(new CompleteBinaryTree<int>());
    [Test]
    public void BinarySearchTreeTest() => ValidateTree(new BinarySearchTree<int, int>());
    // [Test]
    // public void HeapBasedPriorityQueueTest() => ValidateTree(new Heap<int, int>());
    // [Test]
    // public void BinarySearchTreeTest() => ValidateTree(new BinarySearchTree<int, int>());
    // [Test]
    // public void BinarySearchTreeTest() => ValidateTree(new BinarySearchTree<int, int>());
    // [Test]
    // public void BinarySearchTreeTest() => ValidateTree(new BinarySearchTree<int, int>());
    // [Test]
    // public void BinarySearchTreeTest() => ValidateTree(new BinarySearchTree<int, int>());
    // [Test]
    // public void BinarySearchTreeTest() => ValidateTree(new BinarySearchTree<int, int>());
    
    
    private void ValidateTree(ITree<int, int> tree)
    {
        Assert.That(tree.IsEmpty);

        AddTest(tree);
        RemoveTest(tree);
    }

    private void AddTest(ITree<int, int> tree)
    {
        tree.Clear();
        Console.WriteLine("Add Test");

        var randomAddArr = GetRandomIndexes(0, ADD_LOOP_COUNT);
        
        for (int i = 0; i < randomAddArr.Length; i++)
        {
            int value = randomAddArr[i];
            
            Assert.That(tree.Count, Is.EqualTo(i), value.ToString);
            Assert.That(tree.Contains(value), Is.False, value.ToString);

            if (tree is CompleteBinaryTree<int> completeBinaryTree)
            {
                completeBinaryTree.Add(value);
            }
            else
            {
                tree.Add(value, value);
            }

            PrintTree(tree);
            
            Assert.That(tree.Count, Is.EqualTo(i+1), value.ToString);
            Assert.That(tree.Contains(value), Is.True, value.ToString);
        }
    }

    private void RemoveTest(ITree<int, int> tree)
    {
        tree.Clear();
        Console.WriteLine("Remove Test");

        var randomAddArrForRemove = GetRandomIndexes(0, REMOVE_LOOP_COUNT);

        for (int i = 0; i < randomAddArrForRemove.Length; i++)
        {
            if (tree is CompleteBinaryTree<int> completeBinaryTree)
            {
                completeBinaryTree.Add(randomAddArrForRemove[i]);
            }
            else
            {
                tree.Add(randomAddArrForRemove[i], randomAddArrForRemove[i]);
            }
        }
        
        PrintTree(tree);
        
        var randomRemoveArr = GetRandomIndexes(0, REMOVE_LOOP_COUNT);

        for (int i = 0; i < randomRemoveArr.Length; i++)
        {
            int value = randomRemoveArr[i];
            Console.WriteLine($"Remove : {value}");
            
            Assert.That(tree.Count, Is.EqualTo(REMOVE_LOOP_COUNT - i), value.ToString);
            Assert.That(tree.Contains(value), Is.True, value.ToString);
            tree.Remove(value);
            Assert.That(tree.Count, Is.EqualTo(REMOVE_LOOP_COUNT - i - 1), value.ToString);
            Assert.That(tree.Contains(value), Is.False, value.ToString);
            PrintTree(tree);
        }
    }
    
    private int[] GetRandomIndexes(int from, int to)
    {
        int[] arr = new int[to - from];

        for (int i = 0; i < arr.Length; i++)
        {
            arr[i] = from + i;
        }

        Random random = new Random();

        for (int i = 0; i < arr.Length; i++)
        {
            int index = random.Next(0, arr.Length);

            (arr[i], arr[index]) = (arr[index], arr[i]);
        }

        return arr;
    }

    private List<int> GetRandomIndexList(int from, int to)
    {
        List<int> list = new List<int>();
        List<int> result = new List<int>();

        for (int i = 0; i < to - from; i++)
        {
            list.Add(from + i);
        }

        Random random = new Random();

        while (list.Count > 0)
        {
            int index = random.Next(0, list.Count);

            result.Add(list[index]);
            list.RemoveAt(index);
        }

        return result;
    }
    
    public string PrintTree<T>(ITree<T, T> tree) where T : IEquatable<T>, IComparable<T>
    {
        int height = tree.Height();

        if (height == 0)
        {
            return string.Empty;
        }
        
        int depth = 0;
        var depthChecker = new CompleteBinaryTree<T>();

        string[] strs = new string[height];

        Queue<ITree<T, T>> q = new Queue<ITree<T, T>>();
        q.Enqueue(tree);
        q.Enqueue(depthChecker);

        while (q.Count > 0)
        {
            var traverse = q.Dequeue();

            if (traverse == depthChecker)
            {
                if (q.Count == 0)
                {
                    break;
                }
                q.Enqueue(depthChecker);
                depth++;
                continue;                
            }
            
            strs[depth] += traverse.Value!.ToString() + $"/{traverse.Count} ";

            for (int i = 0; i < traverse.ChildrenCount; i++)
            {
                var child = traverse.GetChild(i);

                if (child != null)
                {
                    q.Enqueue(child);
                }
            }
        }

        StringBuilder result = new StringBuilder();

        foreach (var str in strs)
        {
            Console.WriteLine(str);
            result.AppendLine(str);
        }

        return result.ToString();
    }
}