using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using UnityEngine;

[Serializable]
public struct AreaInfo
{
    public int destAreaIndex;
    public NavMeshLink link;
    public bool reverseDirection;
    public readonly Vector3 From => link.transform.position + (
        reverseDirection ? link.endPoint : link.startPoint
    );
    public readonly Vector3 To => link.transform.position + (
        reverseDirection ? link.startPoint : link.endPoint
    );
}

public class AreaScript : MonoBehaviour
{
    public static SortedDictionary<int, AreaScript> areas = new();
    public static int maxAreaIndex;
    public int areaIndex;
    public AreaInfo[] neighbors = { };
    private void Awake()
    {
        areas[areaIndex] = this;
        if (areaIndex > maxAreaIndex)
            maxAreaIndex = areaIndex;
    }

    public static Queue<Vector3> FindPath(int startArea, int targetArea)
    {
        bool[,] adjacencyMatrix = new bool[maxAreaIndex + 1, maxAreaIndex + 1];
        foreach (var area in areas.Values)
            foreach (var neighbor in area.neighbors)
                adjacencyMatrix[area.areaIndex, neighbor.destAreaIndex] = true;

        Graph graph = new(adjacencyMatrix);
        List<int> globalPath = graph.BFS(startArea, targetArea);
        Queue<Vector3> path = new();

        for (int i = 0; i < globalPath.Count - 1; i++)
        {
            AreaScript current = areas[globalPath[i]];
            AreaInfo next = current.neighbors.Where(n => n.destAreaIndex == globalPath[i + 1]).First();
            path.Enqueue(next.From);
            path.Enqueue(next.To);
        }
        return path;
    }
}
