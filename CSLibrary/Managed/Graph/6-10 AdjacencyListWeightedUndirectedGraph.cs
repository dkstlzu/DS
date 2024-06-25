using System.Collections;

namespace CSLibrary;

public class AdjacencyListWeightedUndirectedGraph<T> : IWeightedGraph<T> 
{
    private List<AdjacencyListWeightedVertex<T>> _vertexList = new ();

    private void ValidateVertexNumber(int vertexNumber)
    {
        if (vertexNumber < 0 || vertexNumber >= _vertexList.Count)
        {
            throw new IndexOutOfRangeException();
        }
    }

    #region IGraph Interface

    public void Clear()
    {
        _vertexList.Clear();
    }

    public int Count => _vertexList.Count;
    public bool IsEmpty() => Count == 0;

    public bool IsAdjacent(int vertexNumber1, int vertexNumber2)
    {
        ValidateVertexNumber(vertexNumber1);
        ValidateVertexNumber(vertexNumber2);
        
        return _vertexList[vertexNumber1].IsAdjacentWith(vertexNumber2);
    }

    public List<int> GetNeighbors(int vertexNumber)
    {
        ValidateVertexNumber(vertexNumber);
        
        return _vertexList[vertexNumber].GetAdjacentList();
    }

    public int AddVertex(T? t)
    {
        _vertexList.Add(new AdjacencyListWeightedVertex<T>(t));
        return _vertexList.Count - 1;
    }
    
    public void RemoveVertex(int vertexNumber)
    {
        ValidateVertexNumber(vertexNumber);

        for (int i = 0; i < _vertexList.Count; i++)
        {
            _vertexList[i].RemoveAdjacent(vertexNumber);
            _vertexList[i].ReplaceAdjacent(_vertexList.Count - 1, vertexNumber);
        }
        
        _vertexList[vertexNumber] = _vertexList[^1];
        _vertexList.RemoveAt(_vertexList.Count - 1);
    }

    public void AddEdge(int vertexNumber1, int vertexNumber2)
    {
        ValidateVertexNumber(vertexNumber1);
        ValidateVertexNumber(vertexNumber2);

        if (_vertexList[vertexNumber1].IsAdjacentWith(vertexNumber2))
        {
            throw new InvalidOperationException();
        }

        _vertexList[vertexNumber1].AddAdjacent(vertexNumber2);
        _vertexList[vertexNumber2].AddAdjacent(vertexNumber1);
    }

    public void RemoveEdge(int vertexNumber1, int vertexNumber2)
    {
        ValidateVertexNumber(vertexNumber1);
        ValidateVertexNumber(vertexNumber2);
        
        if (!_vertexList[vertexNumber1].IsAdjacentWith(vertexNumber2))
        {
            throw new InvalidOperationException();
        }
        
        _vertexList[vertexNumber1].RemoveAdjacent(vertexNumber2);
        _vertexList[vertexNumber2].RemoveAdjacent(vertexNumber1);
    }

    public T? GetVertexValue(int vertexNumber)
    {
        ValidateVertexNumber(vertexNumber);

        return _vertexList[vertexNumber].Value;
    }

    public void SetVertexValue(int vertexNumber, T? value)
    {
        ValidateVertexNumber(vertexNumber);
        
        _vertexList[vertexNumber].Value = value;
    }

    public int GetEdgeWeight(int vertexNumber1, int vertexNumber2)
    {
        ValidateVertexNumber(vertexNumber1);
        ValidateVertexNumber(vertexNumber2);
        
        if (!_vertexList[vertexNumber1].IsAdjacentWith(vertexNumber2))
        {
            throw new InvalidOperationException();
        }
        
        return _vertexList[vertexNumber1].GetWeight(vertexNumber2);
    }

    public void SetEdgeWeight(int vertexNumber1, int vertexNumber2, int weight)
    {
        ValidateVertexNumber(vertexNumber1);
        ValidateVertexNumber(vertexNumber2);
        
        if (!_vertexList[vertexNumber1].IsAdjacentWith(vertexNumber2))
        {
            throw new InvalidOperationException();
        }
        
        _vertexList[vertexNumber1].SetWeight(vertexNumber2, weight);
    }
    
    #endregion

    #region IGraphTraversal

    public bool DFS(int startVertexNumber, T? find, Action<T?> traversalAction, Action<T?> onFoundAction, bool returnOnFirstFound = false)
    {
        Stack<int> s = new Stack<int>();
        s.Push(startVertexNumber);

        bool[] visited = new bool[_vertexList.Count];
        bool found = false;
        
        while (s.Count > 0)
        {
            int searchingVertexNumber = s.Pop();

            visited[searchingVertexNumber] = true;

            T? searchingValue = _vertexList[searchingVertexNumber].Value;
            
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

        bool[] visited = new bool[_vertexList.Count];
        visited[startVertexNumber] = true;
        bool found = false;
        
        while (q.Count > 0)
        {
            int searchingVertexNumber = q.Dequeue();

            T? searchingValue = _vertexList[searchingVertexNumber].Value;
            
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
        private List<AdjacencyListWeightedVertex<T>> _vertexList;
        private int _index;
        private T? _current;
        
        public Enumerator(AdjacencyListWeightedUndirectedGraph<T> graph)
        {
            _vertexList = graph._vertexList;
        }

        public bool MoveNext()
        {
            if (_index < _vertexList.Count)
            {
                _current = _vertexList[_index++].Value;
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