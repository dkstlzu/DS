namespace CSLibrary;

public class BinaryTree : ITree<int>
{
    public class Node
    {
        public int Value;
        public Node? Left;
        public Node? Right;

        public Node(int value)
        {
            Value = value;
        }
    }

    private Node _root;

    public BinaryTree(int rootValue)
    {
        _root = new Node(rootValue);
    }

    public BinaryTree(Node rootNode)
    {
        _root = rootNode;
    }

#region ITree Interface
    public void Add(int value)
    {
        Node target;
        Queue<Node> q = new Queue<Node>();
        q.Enqueue(_root);

        while (q.Count > 0)
        {
            target = q.Dequeue();

            if (AddLeft(value, target))
            {
                return;
            } else
            {
                q.Enqueue(target.Left!);
            }

            if (AddRight(value, target))
            {
                return;
            } else
            {
                q.Enqueue(target.Right!);
            }
        }
    }

    private bool AddLeft(int value, Node node)
    {
        if (node.Left != null)
        {
            return false;
        }

        node.Left = new Node(value);
        return true;
    }

    private bool AddRight(int value, Node node)
    {
        if (node.Right != null)
        {
            return false;
        }

        node.Left = new Node(value);
        return true;
    }

    public void Remove(int value)
    {
        Node target;
        Queue<Node> q = new Queue<Node>();
        q.Enqueue(_root);

        while (q.Count > 0)
        {
            target = q.Dequeue();

            if (target.Right != null)
            {
                if (target.Right.Value == value)
                {
                    var leafNode = LeafNode(target.Right);
                    if (leafNode != null)
                    {
                        leafNode.Left = target.Right.Left;
                        leafNode.Right = target.Right.Right;
                    }
                    target.Right = leafNode;
                    return;
                } else
                {
                    q.Enqueue(target.Right);
                }
            }

            if (target.Left != null)
            {
                if (target.Left.Value == value)
                {
                    var leafNode = LeafNode(target.Left);
                    if (leafNode != null)
                    {
                        leafNode.Left = target.Left.Left;
                        leafNode.Right = target.Left.Right;
                    }
                    target.Left = leafNode;
                    return;
                } else
                {
                    q.Enqueue(target.Left);
                }
            }


        }
    }

    private Node? LeafNode(Node node)
    {
        while(node.Left != null || node.Right != null)
        {
            if (node.Right != null)
            {
                node = node.Right;
            } else
            {
                node = node.Left!;
            }
        }

        return node;
    }

    public int Height()
    {
        return Height(_root);
    }

    private int Height(Node node)
    {
        int height = 0;

        if (node.Left != null)
        {
            height = Math.Max(height, Height(node.Left) + 1);
        }

        if (node.Right != null)
        {
            height = Math.Max(height, Height(node.Right) + 1);
        }

        return height;
    }

    public int Count()
    {
        return Count(_root);
    }

    private int Count(Node node)
    {
        int count = 1;

        if (node.Left != null)
        {
            count += Count(node.Left);
        }

        if (node.Right != null)
        {
            count += Count(node.Right);
        }

        return count;
    }
}