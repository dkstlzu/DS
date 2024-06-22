namespace CSLibrary;

public class BinarySearchTree<T>
{
    public class Node
    {
        public T Value;
        public Node? Left;
        public Node? Right;

        public Node(T value)
        {
            Value = value;
        }
    }

    private Node _root;

    public BinarySearchTree(T rootValue)
    {
        _root = new Node(rootValue);
    }

    public BinarySearchTree(Node rootNode)
    {
        _root = rootNode;
    }

    public void Add(T? t)
    {

    }

    public void Remove(T? t)
    {

    }
}