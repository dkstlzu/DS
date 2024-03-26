namespace CSLibrary;

public class GraphUtility<T>
{
    private Dictionary<int, int> _indexDict = new Dictionary<int, int>();
    private IGraph<T> _graph;

    public GraphUtility(IGraph<T> graph)
    {
        _graph = graph;
    }

    public int GetRealKey(int virtualVertexNumber)
    {
        return _indexDict[virtualVertexNumber];
    }

    public int GetVirtualKey(int realVertexNumber)
    {
        return _indexDict.First(e => e.Value == realVertexNumber).Key;
    }

    public bool IsAdjacent(int virtualVertexNumber1, int virtualVertexNumber2)
    {
        return _graph.IsAdjacent(_indexDict[virtualVertexNumber1], _indexDict[virtualVertexNumber2]);
    }

    public List<int> GetNeighbors(int virtualVertexNumber)
    {
        return _graph.GetNeighbors(_indexDict[virtualVertexNumber]);
    }

    public void AddVertex(int virtualVertexNumber, T? t)
    {
        _indexDict.Add(virtualVertexNumber, _graph.AddVertex(t));
    }

    public void RemoveVertex(int virtualVertexNumber)
    {
        int maxKey = 0;
        foreach (var (key, value) in _indexDict)
        {
            if (value == _graph.Count() - 1)
            {
                maxKey = key;
                break;
            }
        }

        _indexDict[maxKey] = _indexDict[virtualVertexNumber];

        _graph.RemoveVertex(_indexDict[virtualVertexNumber]);

        _indexDict.Remove(virtualVertexNumber);
    }

    public void AddEdge(int virtualVertexNumber1, int virtualVertexNumber2)
    {
        _graph.AddEdge(_indexDict[virtualVertexNumber1], _indexDict[virtualVertexNumber2]);
    }

    public void RemoveEdge(int virtualVertexNumber1, int virtualVertexNumber2)
    {
        _graph.RemoveEdge(_indexDict[virtualVertexNumber1], _indexDict[virtualVertexNumber2]);
    }

    public T? GetVertexValue(int virtualVertexNumber)
    {
        return _graph.GetVertexValue(_indexDict[virtualVertexNumber]);
    }

    public void SetVertexValue(int virtualVertexNumber, T? value)
    {
        _graph.SetVertexValue(_indexDict[virtualVertexNumber], value);
    }

    public bool DFS(int virtualVertexNumber, T? find, Action<T?> traversalAction, Action<T?> onFoundAction, bool returnOnFirstFound = true)
    {
        return _graph.DFS(_indexDict[virtualVertexNumber], find, traversalAction, onFoundAction, returnOnFirstFound);
    }
    
    public bool BFS(int virtualVertexNumber, T? find, Action<T?> traversalAction, Action<T?> onFoundAction, bool returnOnFirstFound = true)
    {
        return _graph.BFS(_indexDict[virtualVertexNumber], find, traversalAction, onFoundAction, returnOnFirstFound);
    }

    public void SetEdgeWeight(int virtualVertexNumber1, int virtualVertexNumber2, int weight)
    {
        if (_graph is IWeightedGraph<T> wgraph)
        {
            wgraph.SetEdgeWeight(_indexDict[virtualVertexNumber1], _indexDict[virtualVertexNumber2], weight);
            return;
        }

        throw new InvalidOperationException();
    }
    
    public int GetEdgeWeight(int virtualVertexNumber1, int virtualVertexNumber2)
    {
        if (_graph is IWeightedGraph<T> wgraph)
        {
            return wgraph.GetEdgeWeight(_indexDict[virtualVertexNumber1], _indexDict[virtualVertexNumber2]);
        }

        throw new InvalidOperationException();
    }

    public void Clear()
    {
        _graph.Clear();
        _indexDict.Clear();
    }

    public int Count()
    {
        return _graph.Count();
    }

    public bool IsEmpty()
    {
        return _graph.IsEmpty();
    }
}