using System;
using System.Collections.Generic;

public class Graph
{
    private readonly bool[,] adjacencyMatrix;
    public int Count { get; private set; }

    public Graph(bool[,] matrix)
    {
        adjacencyMatrix = matrix;
        Count = matrix.GetLength(0);
    }

    public List<int> BFS(int start, int target)
    {
        int[] parents = new int[Count];
        bool[] visited = new bool[Count];
        Queue<int> q = new();
        parents[start] = -1;
        visited[start] = true;
        q.Enqueue(start);

        while (q.Count > 0)
        {
            int current = q.Dequeue();
            if (current == target)
                return Path(parents, current);

            for (int i = 0; i < Count; i++)
            {
                if (!visited[i] && adjacencyMatrix[current, i])
                {
                    visited[i] = true;
                    parents[i] = current;
                    q.Enqueue(i);
                }
            }
        }
        return null;
    }

    private List<int> Path(int[] parents, int current)
    {
        List<int> path = new();
        while (current != -1)
        {
            path.Add(current);
            current = parents[current];
        }

        path.Reverse();
        return path;
    }
}
