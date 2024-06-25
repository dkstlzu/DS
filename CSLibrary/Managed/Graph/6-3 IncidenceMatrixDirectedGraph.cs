using System.Collections;

namespace CSLibrary;

public class IncidenceMatrixDirectedGraph<T> : IGraph<T>, IDirectedGraph<T>
{
    private const int DEFAULT_CAPACITY = 16;
    
    private T?[] _vertexValues;
    private List<byte>[] _connected;

    private const byte EDGE_CONNECTION_NONE = 0;
    private const byte EDGE_CONNECTION_FROM = 1;
    private const byte EDGE_CONNECTION_TO = 2;
    private const byte EDGE_CONNECTION_SELF = 3;


    private int _minimumCapacity;
    private int m_size;
    private int _size 
    {
        get => m_size;
        set
        {
            m_size = value;
            CheckVertexResize();
        }
    }
    
    private int _vertexCount => _vertexValues.Length;
    private int _edgeCount => _connected.Length > 0 ? _connected[0].Count : 0;

    public IncidenceMatrixDirectedGraph() : this(DEFAULT_CAPACITY)
    {
    }

    public IncidenceMatrixDirectedGraph(int capacity)
    {
        _vertexValues = new T[capacity];
        _connected = new List<byte>[capacity];
        for (int i = 0; i < capacity; i++)
        {
            _connected[i] = new List<byte>();
        }
        _minimumCapacity = capacity;
    }
    
    private void ValidateVertexNumber(int vertexNumber)
    {
        if (vertexNumber < 0 || vertexNumber >= _size)
        {
            throw new IndexOutOfRangeException();
        }
    }

    private void ResizeVertex()
    {
        int newCapacity = 1;
        while (newCapacity < 2*_size)
        {
            newCapacity <<= 1;
        }

        var previousVertexValues = _vertexValues; 
        _vertexValues = new T?[Math.Max(newCapacity, _minimumCapacity)];

        for (int i = 0; i < _size; i++)
        {
            _vertexValues[i] = previousVertexValues[i];
        }

        var previousConnected = _connected;
        _connected = new List<byte>[_vertexValues.Length];;

        int copyLength = Math.Min(previousVertexValues.Length, _vertexValues.Length);
        
        for (int i = 0; i < copyLength; i++)
        {
            _connected[i] = previousConnected[i];
        }

        for (int i = copyLength; i < _vertexValues.Length; i++)
        {
            _connected[i] = new List<byte>();
        }
    }
    
    private void CheckVertexResize()
    {
        if (_vertexValues.Length <= _minimumCapacity && _size < _vertexValues.Length * 0.5) return;
        
        if (_size >= _vertexValues.Length || _size < _vertexValues.Length * 0.5)
        {
            ResizeVertex();
        }
    }
    
    #region IGraph Interface

    public void Clear()
    {
        for (int i = 0; i < _vertexValues.Length; i++)
        {
            _vertexValues[i] = default;
        }

        for (int i = 0; i < _vertexCount; i++)
        {
            _connected[i].Clear();
        }

        _size = 0;
    }

    public int Count => _size;
    public bool IsEmpty() => Count == 0;
    public bool IsAdjacent(int vertexNumber1, int vertexNumber2)
    {
        ValidateVertexNumber(vertexNumber1);
        ValidateVertexNumber(vertexNumber2);

        if (vertexNumber1 == vertexNumber2)
        {
            for (int i = 0; i < _edgeCount; i++)
            {
                if (_connected[vertexNumber1][i] == EDGE_CONNECTION_SELF)
                {
                    return true;
                }
            }
        }
        else
        {
            for (int i = 0; i < _edgeCount; i++)
            {
                if (_connected[vertexNumber1][i] == EDGE_CONNECTION_FROM && _connected[vertexNumber2][i] == EDGE_CONNECTION_TO)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public List<int> GetNeighbors(int vertexNumber)
    {
        List<int> list = new();

        for (int i = 0; i < _edgeCount; i++)
        {
            if (_connected[vertexNumber][i] == EDGE_CONNECTION_FROM)
            {
                for (int j = 0; j < _vertexCount; j++)
                {
                    if (j == vertexNumber) continue;

                    if (_connected[j][i] == EDGE_CONNECTION_TO)
                    {
                        list.Add(j);
                    }
                }
            } else if (_connected[vertexNumber][i] == EDGE_CONNECTION_SELF)
            {
                list.Add(i);
            }
        }

        return list;
    }

    public int AddVertex(T? t)
    {
        _vertexValues[_size] = t;
        _size++;
        return _size - 1;
    }

    public void RemoveVertex(int vertexNumber)
    {
        ValidateVertexNumber(vertexNumber);

        List<int> removeEdgeIndexes = new();

        for (int i = 0; i < _edgeCount; i++)
        {
            if (_connected[vertexNumber][i] != EDGE_CONNECTION_NONE)
            {
                removeEdgeIndexes.Add(i);
            }
        }

        RemoveEdges(removeEdgeIndexes);
        
        _vertexValues[vertexNumber] = _vertexValues[--_size];
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
            if (vertexNumber1 == i && vertexNumber2 == i)
            {
                _connected[i].Add(EDGE_CONNECTION_SELF);
            } else if (vertexNumber1 == i)
            {
                _connected[i].Add(EDGE_CONNECTION_FROM);
            } else if (vertexNumber2 == i)
            {
                _connected[i].Add(EDGE_CONNECTION_TO);
            }
            else
            {
                _connected[i].Add(EDGE_CONNECTION_NONE);
            }
        }
    }

    public void RemoveEdge(int vertexNumber1, int vertexNumber2)
    {
        ValidateVertexNumber(vertexNumber1);
        ValidateVertexNumber(vertexNumber2);

        int edgeIndex = -1;
        if (vertexNumber1 == vertexNumber2)
        {
            for (int i = 0; i < _edgeCount; i++)
            {
                if (_connected[vertexNumber1][i] == EDGE_CONNECTION_SELF)
                {
                    edgeIndex = i;
                    break;
                }
            }
        }
        else
        {
            for (int i = 0; i < _edgeCount; i++)
            {
                if (_connected[vertexNumber1][i] == EDGE_CONNECTION_FROM && _connected[vertexNumber2][i] == EDGE_CONNECTION_TO)
                {
                    edgeIndex = i;
                    break;
                }
            }
        }

        if (edgeIndex >= 0)
        {
            for (int i = 0; i < _size; i++)
            {
                _connected[i].RemoveAt(edgeIndex);
            }
            
            return;
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

        bool[] visited = new bool[_vertexValues.Length];
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

        bool[] visited = new bool[_vertexValues.Length];
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
        
        public Enumerator(IncidenceMatrixDirectedGraph<T> graph)
        {
            _vertexList = new List<T?>(graph._vertexValues);
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