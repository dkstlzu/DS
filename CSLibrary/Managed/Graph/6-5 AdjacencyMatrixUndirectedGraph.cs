using System.Collections;

namespace CSLibrary;

public class AdjacencyMatrixUndirectedGraph<T> : IGraph<T>
{
    private List<T?> _vertexValues = new ();
    private List<List<bool>> _connected = new List<List<bool>>();

    private void ValidateVertexNumber(int vertexNumber)
    {
        if (vertexNumber < 0 || vertexNumber >= _vertexValues.Count)
        {
            throw new IndexOutOfRangeException();
        }
    }
    
    #region IGraph Interface

    public void Clear()
    {
        _vertexValues.Clear();
        _connected.Clear();
    }

    public int Count => _vertexValues.Count;
    public bool IsEmpty() => Count == 0;

    public bool IsAdjacent(int vertexNumber1, int vertexNumber2)
    {
        ValidateVertexNumber(vertexNumber1);
        ValidateVertexNumber(vertexNumber2);

        return _connected[vertexNumber1][vertexNumber2];
    }

    public List<int> GetNeighbors(int vertexNumber)
    {
        ValidateVertexNumber(vertexNumber);

        List<int> list = new List<int>();
        
        for (int i = 0; i < _vertexValues.Count; i++)
        {
            if (_connected[vertexNumber][i])
            {
                list.Add(i);
            }
        }

        return list;
    }

    public int AddVertex(T? t)
    {
        _vertexValues.Add(t);

        foreach (var list in _connected)
        {
            list.Add(false);
        }

        var newVertexConnectionList = new List<bool>();

        for (int i = 0; i < _vertexValues.Count; i++)
        {
            newVertexConnectionList.Add(false);
        }
        
        _connected.Add(newVertexConnectionList);

        return _vertexValues.Count - 1;
    }

    public void RemoveVertex(int vertexNumber)
    {
        ValidateVertexNumber(vertexNumber);

        foreach (var list in _connected)
        {
            list.RemoveAt(vertexNumber);
        }

        _connected[vertexNumber] = _connected[^1];
        _connected.RemoveAt(_connected.Count - 1);
        
        _vertexValues[vertexNumber] = _vertexValues[^1];
        _vertexValues.RemoveAt(_vertexValues.Count - 1);
    }

    public void AddEdge(int vertexNumber1, int vertexNumber2)
    {
        ValidateVertexNumber(vertexNumber1);
        ValidateVertexNumber(vertexNumber2);

        if (_connected[vertexNumber1][vertexNumber2])
        {
            throw new InvalidOperationException();
        }

        _connected[vertexNumber1][vertexNumber2] = true;
        _connected[vertexNumber2][vertexNumber1] = true;
    }

    public void RemoveEdge(int vertexNumber1, int vertexNumber2)
    {
        ValidateVertexNumber(vertexNumber1);
        ValidateVertexNumber(vertexNumber2);

        if (!_connected[vertexNumber1][vertexNumber2])
        {
            throw new InvalidOperationException();
        }

        _connected[vertexNumber1][vertexNumber2] = false;
        _connected[vertexNumber2][vertexNumber1] = false;
    }

    public T? GetVertexValue(int vertexNumber)
    {
        ValidateVertexNumber(vertexNumber);

        return _vertexValues[vertexNumber];
    }

    public void SetVertexValue(int vertexNumber, T? value)
    {
        ValidateVertexNumber(vertexNumber);

        _vertexValues[vertexNumber] = value;
    }

    #endregion
    
    #region IGraphTraversal

    public bool DFS(int startVertexNumber, T? find, Action<T?> traversalAction, Action<T?> onFoundAction, bool returnOnFirstFound = false)
    {
        Stack<int> s = new Stack<int>();
        s.Push(startVertexNumber);

        bool[] visited = new bool[_vertexValues.Count];
        bool found = false;
        
        while (s.Count > 0)
        {
            int searchingVertexNumber = s.Pop();

            visited[searchingVertexNumber] = true;

            T? searchingValue = _vertexValues[searchingVertexNumber];
            
            if (find == null && searchingValue== null)
            {
                onFoundAction(searchingValue);
                found = true;
                if (returnOnFirstFound)
                {
                    return true;
                }
            } else if (find != null && find.Equals(searchingValue))
            {
                onFoundAction(searchingValue);
                found = true;
                if (returnOnFirstFound)
                {
                    return true;
                }
            }
            else
            {
                traversalAction(searchingValue);
            }

            foreach (int connected in GetNeighbors(searchingVertexNumber))
            {
                if (!visited[connected])
                {
                    s.Push(connected);
                }
            }
        }

        return found;
    }

    public bool BFS(int startVertexNumber, T? find, Action<T?> traversalAction, Action<T?> onFoundAction, bool returnOnFirstFound = false)
    {
        Queue<int> q = new Queue<int>();
        q.Enqueue(startVertexNumber);

        bool[] visited = new bool[_vertexValues.Count];
        visited[startVertexNumber] = true;
        bool found = false;
        
        while (q.Count > 0)
        {
            int searchingVertexNumber = q.Dequeue();

            T? searchingValue = _vertexValues[searchingVertexNumber];
            
            if (find == null && searchingValue== null)
            {
                onFoundAction(searchingValue);
                found = true;
                if (returnOnFirstFound)
                {
                    return true;
                }
            } else if (find != null && find.Equals(searchingValue))
            {
                onFoundAction(searchingValue);
                found = true;
                if (returnOnFirstFound)
                {
                    return true;
                }
            }
            else
            {
                traversalAction(searchingValue);
            }

            foreach (int connected in GetNeighbors(searchingVertexNumber))
            {
                if (!visited[connected])
                {
                    q.Enqueue(connected);
                    visited[connected] = true;
                }
            }
        }

        return found;
    }

    #endregion
    
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
        private List<T?> _vertexList;
        private int _index;
        private T? _current;
        
        public Enumerator(AdjacencyMatrixUndirectedGraph<T> graph)
        {
            _vertexList = graph._vertexValues;
        }

        public bool MoveNext()
        {
            if (_index < _vertexList.Count)
            {
                _current = _vertexList[_index++];
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