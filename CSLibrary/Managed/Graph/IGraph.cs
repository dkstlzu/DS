namespace CSLibrary;

public interface IGraph<T> : ICollection, IGraphTraversal<T>, IEnumerable<T>
{
    bool IsAdjacent(int vertexNumber1, int vertexNumber2);
    List<int> GetNeighbors(int vertexNumber);
    int AddVertex(T? t);
    void RemoveVertex(int vertexNumber);
    void AddEdge(int vertexNumber1, int vertexNumber2);
    void RemoveEdge(int vertexNumber1, int vertexNumber2);
    T? GetVertexValue(int vertexNumber);
    void SetVertexValue(int vertexNumber, T? value);
}

public interface IWeightedGraph<T> : IGraph<T>
{
    int GetEdgeWeight(int vertexNumber1, int vertexNumber2);
    void SetEdgeWeight(int vertexNumber1, int vertexNumber2, int weight);
}

public interface IDirectedGraph<T> : IGraph<T>
{
    
}

public interface IGraphTraversal<T>
{
    bool DFS(int startVertexNumber, T? find, Action<T?> traversalAction, Action<T?> onFoundAction, bool returnOnFirstFound = true);
    bool BFS(int startVertexNumber, T? find, Action<T?> traversalAction, Action<T?> onFoundAction, bool returnOnFirstFound = true);
}