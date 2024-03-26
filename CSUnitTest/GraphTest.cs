#nullable disable
using CSLibrary;
using static Common.Utility;

namespace CSUnitTest;

public class GraphTest
{
    private const int LOOP_COUNT = 100;

    [Test] public void AdjacencyListDirectedGraphTest() => ValidateGraph(new AdjacencyListDirectedGraph<string>());
    [Test] public void AdjacencyMatrixDirectedGraphTest() => ValidateGraph(new AdjacencyMatrixDirectedGraph<string>());
    [Test] public void IncidenceMatrixDirectedGraphTest() => ValidateGraph(new IncidenceMatrixDirectedGraph<string>());
    [Test] public void AdjacencyListUndirectedGraphTest() => ValidateGraph(new AdjacencyListUndirectedGraph<string>());
    [Test] public void AdjacencyMatrixUndirectedGraphTest() => ValidateGraph(new AdjacencyMatrixUndirectedGraph<string>());
    [Test] public void IncidenceMatrixUndirectedGraphTest() => ValidateGraph(new IncidenceMatrixUndirectedGraph<string>());
    [Test] public void AdjacencyListWeightedDirectedGraphTest() => ValidateGraph(new AdjacencyListWeightedDirectedGraph<string>());
    [Test] public void AdjacencyMatrixWeightedDirectedGraphTest() => ValidateGraph(new AdjacencyMatrixWeightedDirectedGraph<string>());
    [Test] public void IncidenceMatrixWeightedDirectedGraphTest() => ValidateGraph(new IncidenceMatrixWeightedDirectedGraph<string>());
    [Test] public void AdjacencyListWeightedUndirectedGraphTest() => ValidateGraph(new AdjacencyListWeightedUndirectedGraph<string>());
    [Test] public void AdjacencyMatrixWeightedUndirectedGraphTest() => ValidateGraph(new AdjacencyMatrixWeightedUndirectedGraph<string>());
    [Test] public void IncidenceMatrixWeightedUndirectedGraphTest() => ValidateGraph(new IncidenceMatrixWeightedUndirectedGraph<string>());

    private void ValidateGraph(IGraph<string> graph)
    {
        AddVertexTest(graph);
        RemoveVertexTest(graph);
        AddEdgeTest(graph);
        RemoveEdgeTest(graph);
        GetNeighborTest(graph);

        DFSTest(graph);
        BFSTest(graph);
        
        if (graph is IWeightedGraph<string> wgraph)
        {
            ValidateWeightedGraph(wgraph);
        }
    }
    
    private void ValidateWeightedGraph(IWeightedGraph<string> graph)
    {
        SetWeightTest(graph);
    }

    private void AddVertexTest(IGraph<string> graph)
    {
        graph.Clear();

        printl("Graph AddVertex Test");
        GraphUtility<string> utility = new GraphUtility<string>(graph);
        
        for (int i = 0; i < LOOP_COUNT; i++)
        {
            // 10은 임의의 값입니다.
            utility.AddVertex(i + 10, $"Node{i}");
            Assert.That(utility.Count(), Is.EqualTo(i + 1));
            Assert.That(utility.GetVertexValue(i + 10), Is.EqualTo($"Node{i}"));
            
            utility.SetVertexValue(i + 10, $"SetNode{i}");
            Assert.That(utility.GetVertexValue(i + 10), Is.EqualTo($"SetNode{i}"));
            prints(utility.GetVertexValue(i + 10));
        }
        
        printl();
    }
    
    private void RemoveVertexTest(IGraph<string> graph)
    {
        graph.Clear();
        GraphUtility<string> utility = new GraphUtility<string>(graph);

        for (int i = 0; i < LOOP_COUNT; i++)
        {
            // 10은 임의의 값입니다.
            utility.AddVertex(i + 10, $"Node{i}");
        }

        for (int i = 0; i < LOOP_COUNT; i++)
        {
            // 10은 임의의 값입니다.
            utility.RemoveVertex(i + 10);
            Assert.That(graph.Count(), Is.EqualTo(LOOP_COUNT - i - 1));
        }
    }
    
    private void AddEdgeTest(IGraph<string> graph)
    {
        graph.Clear();
        GraphUtility<string> utility = new GraphUtility<string>(graph);

        for (int i = 0; i < LOOP_COUNT; i++)
        {
            // 10은 임의의 값입니다.
            utility.AddVertex(i + 10, $"Node{i}");
        }
        
        for (int i = 0; i < LOOP_COUNT - 1; i++)
        {
            // 10은 임의의 값입니다.
            Assert.That(!utility.IsAdjacent(i + 10, i + 11));
            utility.AddEdge(i + 10, i + 11);
            Assert.That(utility.IsAdjacent(i + 10, i + 11));
        }
    }
    
    private void RemoveEdgeTest(IGraph<string> graph)
    {
        graph.Clear();
        GraphUtility<string> utility = new GraphUtility<string>(graph);

        for (int i = 0; i < LOOP_COUNT; i++)
        {
            // 10은 임의의 값입니다.
            utility.AddVertex(i + 10, $"Node{i}");
        }        
        for (int i = 0; i < LOOP_COUNT - 1; i++)
        {
            // 10은 임의의 값입니다.
            utility.AddEdge(i + 10, i + 11);
        }
        
        for (int i = 0; i < LOOP_COUNT - 1; i++)
        {
            // 10은 임의의 값입니다.
            Assert.That(utility.IsAdjacent(i + 10, i + 11));
            utility.RemoveEdge(i + 10, i + 11);
            Assert.That(!utility.IsAdjacent(i + 10, i + 11));
        }
    }

    private void GetNeighborTest(IGraph<string> graph)
    {
        graph.Clear();
        GraphUtility<string> utility = new GraphUtility<string>(graph);

        for (int i = 0; i < LOOP_COUNT; i++)
        {
            // 10은 임의의 값입니다.
            utility.AddVertex(i + 10, $"Node{i}");
        }
        
        for (int i = 0; i < LOOP_COUNT - 1; i++)
        {
            // 10은 임의의 값입니다.
            utility.AddEdge(i + 10, i + 11);
        }
        
        printl("Graph GetNeighbor Test");
        
        for (int i = 0; i < LOOP_COUNT; i++)
        {
            // 10은 임의의 값입니다.
            var neighbors = utility.GetNeighbors(i + 10);

            prints($"Neighbors of {i + 10} :");
            foreach (int neighbor in neighbors)
            {
                prints(utility.GetVirtualKey(neighbor));
            }
            printl();
        }
    }
    
    private void DFSTest(IGraph<string> graph)
    {
        graph.Clear();
        GraphUtility<string> utility = new GraphUtility<string>(graph);

        for (int i = 0; i < LOOP_COUNT; i++)
        {
            // 10은 임의의 값입니다.
            utility.AddVertex(i + 10, $"Node{i}");
        }
        
        for (int i = 0; i < LOOP_COUNT - 1; i++)
        {
            // 10은 임의의 값입니다.
            utility.AddEdge(i + 10, i + 11);
        }
        
        for (int i = 0; i < LOOP_COUNT - 2; i++)
        {
            // 10은 임의의 값입니다.
            utility.AddEdge(i + 10, i + 12);
        }
        
        printl("DFS Test");

        if (graph is IDirectedGraph<string>)
        {
            // 첫번째에서 부터 탐색 시작
            Assert.That(utility.DFS(10, "Node65", prints, found => printl("Found! : " + found)));
            printl();

            // 정 가운데에서 부터 탐색 시작
            Assert.That(!utility.DFS(10 + LOOP_COUNT/2 - 1, "Node5", prints, found => printl("Found! : " + found)));
            printl();

            // 마지막에서 부터 탐색 시작
            Assert.That(!utility.DFS(10 + LOOP_COUNT - 1, "Node68", prints, found => printl("Found! : " + found)));
            printl();
        }
        else
        {
            // 첫번째에서 부터 탐색 시작
            Assert.That(utility.DFS(10, "Node65", prints, found => printl("Found! : " + found)));
            printl();

            // 정 가운데에서 부터 탐색 시작
            Assert.That(utility.DFS(10 + LOOP_COUNT/2 - 1, "Node5", prints, found => printl("Found! : " + found)));
            printl();

            // 마지막에서 부터 탐색 시작
            Assert.That(utility.DFS(10 + LOOP_COUNT - 1, "Node68", prints, found => printl("Found! : " + found)));
            printl();
        }
    }
    
    private void BFSTest(IGraph<string> graph)
    {
        graph.Clear();
        GraphUtility<string> utility = new GraphUtility<string>(graph);

        for (int i = 0; i < LOOP_COUNT; i++)
        {
            // 10은 임의의 값입니다.
            utility.AddVertex(i + 10, $"Node{i}");
        }
        
        for (int i = 0; i < LOOP_COUNT - 1; i++)
        {
            // 10은 임의의 값입니다.
            utility.AddEdge(i + 10, i + 11);
        }
        
        for (int i = 0; i < LOOP_COUNT - 2; i++)
        {
            // 10은 임의의 값입니다.
            utility.AddEdge(i + 10, i + 12);
        }

        printl("BFS Test");

        if (graph is IDirectedGraph<string>)
        {
            // 첫번째에서 부터 탐색 시작
            Assert.That(utility.BFS(10, "Node65", prints, found => printl("Found! : " + found)));
            printl();

            // 정 가운데에서 부터 탐색 시작
            Assert.That(!utility.BFS(10 + LOOP_COUNT/2 - 1, "Node5", prints, found => printl("Found! : " + found)));
            printl();

            // 마지막에서 부터 탐색 시작
            Assert.That(!utility.BFS(10 + LOOP_COUNT - 1, "Node68", prints, found => printl("Found! : " + found)));
            printl();
        }
        else
        {
            // 첫번째에서 부터 탐색 시작
            Assert.That(utility.BFS(10, "Node65", prints, found => printl("Found! : " + found)));
            printl();

            // 정 가운데에서 부터 탐색 시작
            Assert.That(utility.BFS(10 + LOOP_COUNT/2 - 1, "Node5", prints, found => printl("Found! : " + found)));
            printl();

            // 마지막에서 부터 탐색 시작
            Assert.That(utility.BFS(10 + LOOP_COUNT - 1, "Node68", prints, found => printl("Found! : " + found)));
            printl();
        }
    }

    private void SetWeightTest(IWeightedGraph<string> graph)
    {
        graph.Clear();
        GraphUtility<string> utility = new GraphUtility<string>(graph);

        for (int i = 0; i < LOOP_COUNT; i++)
        {
            // 10은 임의의 값입니다.
            utility.AddVertex(i + 10, $"Node{i}");
        }
        
        for (int i = 0; i < LOOP_COUNT - 1; i++)
        {
            // 10은 임의의 값입니다.
            utility.AddEdge(i + 10, i + 11);
            utility.SetEdgeWeight(i + 10, i + 11, i);
            Assert.That(utility.GetEdgeWeight(i + 10, i + 11), Is.EqualTo(i));
        }
    }

    private void PrintGraph(IGraph<string> graph)
    {
        var enumerator = graph.GetEnumerator();

        while (enumerator.MoveNext())
        {
            if (enumerator.Current != null)
            {
                prints(enumerator.Current);
            }
            else
            {
                prints("Null");
            }
        }
        printl();
        
        enumerator.Dispose();
    }
}