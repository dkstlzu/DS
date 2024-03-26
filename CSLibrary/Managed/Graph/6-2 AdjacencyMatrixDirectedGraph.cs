using System.Collections;

namespace CSLibrary;

public class AdjacencyMatrixDirectedGraph<T> : IGraph<T>, IDirectedGraph<T>
{
    private const int DEFAULT_CAPACITY = 16;
    
    private T?[] _vertexValues;
    private bool[,] _connected;

    private int _minimumCapacity;
    private int m_size;
    private int _size
    {
        get => m_size;
        set
        {
            m_size = value;
            CheckResize();
        }
    }

    public AdjacencyMatrixDirectedGraph() : this(DEFAULT_CAPACITY)
    {
        
    }
    
    public AdjacencyMatrixDirectedGraph(int capacity)
    {
        _vertexValues = new T[capacity];
        _connected = new bool[capacity, capacity];
        _minimumCapacity = capacity;
    }
    
    private void ValidateVertexNumber(int vertexNumber)
    {
        if (vertexNumber < 0 || vertexNumber >= _size)
        {
            throw new IndexOutOfRangeException();
        }
    }

    private void Resize()
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
        _connected = new bool[_vertexValues.Length, _vertexValues.Length];

        for (int i = 0; i < _size; i++)
        {
            for (int j = 0; j < _size; j++)
            {
                _connected[i, j] = previousConnected[i, j];
            }
        }
    }

    private void CheckResize()
    {
        if (_vertexValues.Length <= _minimumCapacity && _size < _vertexValues.Length * 0.5) return;
        
        if (_size >= _vertexValues.Length || _size < _vertexValues.Length * 0.5)
        {
            Resize();
        }
    }

    #region IGraph Interface

    public void Clear()
    {
        for (int i = 0; i < _vertexValues.Length; i++)
        {
            _vertexValues[i] = default;
        }

        for (int i = 0; i < _connected.GetUpperBound(0); i++)
        {
            for (int j = 0; j < _connected.GetUpperBound(1); j++)
            {
                _connected[i, j] = default;
            }
        }

        _size = 0;
    }

    public int Count() => _size;
    public bool IsEmpty() => Count() == 0;

    public bool IsAdjacent(int vertexNumber1, int vertexNumber2)
    {
        ValidateVertexNumber(vertexNumber1);
        ValidateVertexNumber(vertexNumber2);
        
        return _connected[vertexNumber1, vertexNumber2];
    }
    
    public List<int> GetNeighbors(int vertexNumber)
    {
        ValidateVertexNumber(vertexNumber);

        List<int> list = new();
        for (int i = 0; i < _vertexValues.Length; i++)
        {
            if (_connected[vertexNumber, i])
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

        _vertexValues[vertexNumber] = _vertexValues[--_size];
        
        for (int i = 0; i < _vertexValues.Length; i++)
        {
            _connected[vertexNumber, i] = _connected[_size, i];
            _connected[i, vertexNumber] = _connected[i, _size];
        }

        _connected[vertexNumber, vertexNumber] = _connected[_size, _size];
    }

    public void AddEdge(int vertexNumber1, int vertexNumber2)
    {
        ValidateVertexNumber(vertexNumber1);
        ValidateVertexNumber(vertexNumber2);

        if (IsAdjacent(vertexNumber1, vertexNumber2))
        {
            throw new InvalidOperationException();
        }

        _connected[vertexNumber1, vertexNumber2] = true;
    }

    public void RemoveEdge(int vertexNumber1, int vertexNumber2)
    {
        ValidateVertexNumber(vertexNumber1);
        ValidateVertexNumber(vertexNumber2);

        if (!IsAdjacent(vertexNumber1, vertexNumber2))
        {
            throw new InvalidOperationException();
        }

        _connected[vertexNumber1, vertexNumber2] = false;
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
        
        public Enumerator(AdjacencyMatrixDirectedGraph<T> graph)
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