using System.Collections;

namespace CSLibrary;

public class IncidenceMatrixUndirectedGraph<T> : IGraph<T>
{
    private List<T?> _vertexValues = new();
    private List<List<bool>> _connected = new List<List<bool>>();
    
    private int _vertexCount => _vertexValues.Count;
    private int _edgeCount => _connected.Count > 0 ? _connected[0].Count : 0;
    
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

    public int Count => _vertexCount;
    public bool IsEmpty() => Count == 0;

    public bool IsAdjacent(int vertexNumber1, int vertexNumber2)
    {
        ValidateVertexNumber(vertexNumber1);
        ValidateVertexNumber(vertexNumber2);

        for (int i = 0; i < _edgeCount; i++)
        {
            if (_connected[vertexNumber1][i] && _connected[vertexNumber2][i])
            {
                return true;
            }
        }

        return false;
    }

    public List<int> GetNeighbors(int vertexNumber)
    {
        List<int> list = new();

        for (int i = 0; i < _edgeCount; i++)
        {
            if (_connected[vertexNumber][i])
            {
                for (int j = 0; j < _vertexCount; j++)
                {
                    if (j == vertexNumber) continue;

                    if (_connected[j][i])
                    {
                        list.Add(j);
                    }
                }
            }
        }

        return list;
    }

    public int AddVertex(T? t)
    {
        _vertexValues.Add(t);
        var newVertexConnectionList = new List<bool>();

        for (int i = 0; i < _edgeCount; i++)
        {
            newVertexConnectionList.Add(false);
        }
        
        _connected.Add(newVertexConnectionList);

        return _vertexCount - 1;
    }

    public void RemoveVertex(int vertexNumber)
    {
        ValidateVertexNumber(vertexNumber);
        
        List<int> removeEdgeIndexes = new();

        for (int i = 0; i < _edgeCount; i++)
        {
            if (_connected[vertexNumber][i])
            {
                removeEdgeIndexes.Add(i);
            }
        }

        RemoveEdges(removeEdgeIndexes);
        
        _vertexValues[vertexNumber] = _vertexValues[^1];
        _vertexValues.RemoveAt(_vertexValues.Count - 1);
    }

    public void AddEdge(int vertexNumber1, int vertexNumber2)
    {
        ValidateVertexNumber(vertexNumber1);
        ValidateVertexNumber(vertexNumber2);

        if (IsAdjacent(vertexNumber1, vertexNumber2))
        {
            throw new InvalidOperationException();
        }
        
        for (int i = 0; i < _vertexCount; i++)
        {
            if (vertexNumber1 == i || vertexNumber2 == i)
            {
                _connected[i].Add(true);
            }
            else
            {
                _connected[i].Add(false);
            }
        }
    }

    public void RemoveEdge(int vertexNumber1, int vertexNumber2)
    {
        ValidateVertexNumber(vertexNumber1);
        ValidateVertexNumber(vertexNumber2);
        
        for (int i = 0; i < _edgeCount; i++)
        {
            if (_connected[vertexNumber1][i] && _connected[vertexNumber2][i])
            {
                for (int j = 0; j < _vertexCount; j++)
                {
                    _connected[j].RemoveAt(i);
                }
                
                return;
            }
        }
        
        throw new InvalidOperationException();
    }
    
    private void RemoveEdges(List<int> removeEdgeNumbers)
    {
        if (removeEdgeNumbers.Count == 0)
        {
            return;
        }
        
        removeEdgeNumbers.Sort();

        for (int i = removeEdgeNumbers.Count - 1; i >= 0; i--)
        {
            for (int j = 0; j < _vertexCount; j++)
            {
                _connected[j].RemoveAt(removeEdgeNumbers[i]);
            }
        }
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
        
        public Enumerator(IncidenceMatrixUndirectedGraph<T> graph)
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