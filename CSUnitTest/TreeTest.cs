using CSLibrary;

namespace CSUnitTest;

public class TreeTest
{
    private const int ADD_LOOP_COUNT = 100;
    private const int REMOVE_LOOP_COUNT = 100;

    [Test]
    public void CompleteBinaryTreeTest()
    {
        var tree = new CompleteBinaryTree<int>();
        
        Assert.That(tree.IsEmpty);

        Console.WriteLine("Add Test");

        var randomAddArr = GetRandomIndexes(0, ADD_LOOP_COUNT);
        
        for (int i = 0; i < randomAddArr.Length; i++)
        {
            int value = randomAddArr[i];
            
            Assert.That(tree.Count(), Is.EqualTo(i));
            Assert.That(tree.Contains(value), Is.False);
            tree.Add(value);
            Assert.That(tree.Count(), Is.EqualTo(i+1));
            Assert.That(tree.Contains(value), Is.True);
        }
        
        Console.WriteLine("Clear Test");

        tree.Clear();
        
        Assert.That(tree.IsEmpty);
        
        Console.WriteLine("Remove Test");

        var randomAddArrForRemove = GetRandomIndexes(0, REMOVE_LOOP_COUNT);

        for (int i = 0; i < randomAddArrForRemove.Length; i++)
        {
            tree.Add(randomAddArrForRemove[i]);
        }
        
        PrintCompleteTree(tree);
        
        var randomRemoveArr = GetRandomIndexes(0, REMOVE_LOOP_COUNT);

        for (int i = 0; i < randomRemoveArr.Length; i++)
        {
            int value = randomRemoveArr[i];
            Console.WriteLine($"Remove : {value}");
            
            Assert.That(tree.Count(), Is.EqualTo(REMOVE_LOOP_COUNT - i));
            Assert.That(tree.Contains(value), Is.True);
            tree.Remove(value);
            Assert.That(tree.Count(), Is.EqualTo(REMOVE_LOOP_COUNT - i - 1));
            Assert.That(tree.Contains(value), Is.False);
            PrintCompleteTree(tree);
        }
    }

    public string PrintCompleteTree<T>(CompleteBinaryTree<T> tree) where T : IEquatable<T>
    {
        int height = tree.Height();

        if (height == 0)
        {
            return string.Empty;
        }
        
        int depth = 0;
        var depthChecker = new CompleteBinaryTree<T>();

        int width = (int)Math.Pow(2, height - 1) * 3 - 1;

        string[] strs = new string[height];

        Queue<CompleteBinaryTree<T>> q = new Queue<CompleteBinaryTree<T>>();
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
            
            strs[depth] += traverse.Value!.ToString() + $"/{traverse.Count()} ";
            
            if (traverse.Left != null && !traverse.Left.IsEmpty())
                q.Enqueue(traverse.Left);
            if (traverse.Right != null && !traverse.Right.IsEmpty())
                q.Enqueue(traverse.Right);
        }

        string result = "";

        foreach (var str in strs)
        {
            Console.WriteLine(str);
            result += str + "\n";
        }
        Console.WriteLine($"Leaf : {tree.GetRemoveTargetLeafNode().Value}");

        return result;
    }

    [Test]
    public void BinarySearchTreeTest()
    {
        var tree = new BinarySearchTree<int, string>();
        
        Assert.That(tree.IsEmpty);

        Console.WriteLine("Add Test");

        var randomAddArr = GetRandomIndexes(0, ADD_LOOP_COUNT);
        
        for (int i = 0; i < randomAddArr.Length; i++)
        {
            int value = randomAddArr[i];
            
            Assert.That(tree.Count(), Is.EqualTo(i));
            Assert.That(tree.Contains(value), Is.False);
            tree.Add(value, $"Name{i}");
            Assert.That(tree.Count(), Is.EqualTo(i+1));
            Assert.That(tree.Contains(value), Is.True);
        }
        
        Console.WriteLine("Clear Test");

        tree.Clear();
        
        Assert.That(tree.IsEmpty);
        
        Console.WriteLine("Remove Test");

        var randomAddArrForRemove = GetRandomIndexes(0, REMOVE_LOOP_COUNT);

        for (int i = 0; i < randomAddArrForRemove.Length; i++)
        {
            tree.Add(randomAddArrForRemove[i], randomAddArrForRemove[i].ToString());
        }
        
        var randomRemoveArr = GetRandomIndexes(0, REMOVE_LOOP_COUNT);

        for (int i = 0; i < randomRemoveArr.Length; i++)
        {
            int value = randomRemoveArr[i];
            Console.WriteLine($"Remove : {value}");
            
            Assert.That(tree.Count(), Is.EqualTo(REMOVE_LOOP_COUNT - i));
            Assert.That(tree.Contains(value), Is.True);
            tree.Remove(value);
            Assert.That(tree.Count(), Is.EqualTo(REMOVE_LOOP_COUNT - i - 1));
            Assert.That(tree.Contains(value), Is.False);
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
}