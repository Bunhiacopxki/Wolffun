using System;
using System.Collections.Generic;
using UnityEngine;

public class PixelColliderBuilder : MonoBehaviour
{
    [SerializeField] private float colliderPadding = 0f;

    private struct Edge : IEquatable<Edge>
    {
        public Vector2Int a;
        public Vector2Int b;

        public Edge(Vector2Int p1, Vector2Int p2)
        {
            if (p1.x < p2.x || (p1.x == p2.x && p1.y <= p2.y))
            {
                a = p1;
                b = p2;
            }
            else
            {
                a = p2;
                b = p1;
            }
        }

        public bool Equals(Edge other)
        {
            return a == other.a && b == other.b;
        }

        public override bool Equals(object obj)
        {
            return obj is Edge other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(a, b);
        }
    }

    public void Rebuild(PixelObjectRoot root, PolygonCollider2D polygonCollider)
    {
        if (root == null || polygonCollider == null) return;

        float pixelSize = 1f / GameManager.Instance.PixelsPerUnit;
        float halfPad = colliderPadding * 0.5f;

        HashSet<Edge> boundaryEdges = BuildBoundaryEdges(root);

        if (boundaryEdges.Count == 0)
        {
            polygonCollider.pathCount = 0;
            polygonCollider.enabled = false;
            return;
        }

        List<List<Vector2>> paths = BuildPathsFromEdges(boundaryEdges, root, pixelSize, halfPad);

        if (paths.Count == 0)
        {
            polygonCollider.pathCount = 0;
            polygonCollider.enabled = false;
            return;
        }

        polygonCollider.enabled = true;
        polygonCollider.pathCount = paths.Count;

        for (int i = 0; i < paths.Count; i++)
        {
            polygonCollider.SetPath(i, paths[i].ToArray());
        }
    }

    private HashSet<Edge> BuildBoundaryEdges(PixelObjectRoot root)
    {
        HashSet<Edge> edges = new HashSet<Edge>();

        for (int x = 0; x < root.Width; x++)
        {
            for (int y = 0; y < root.Height; y++)
            {
                if (!root.Cells[x, y].alive) continue;

                Vector2Int bl = new Vector2Int(x, y);
                Vector2Int br = new Vector2Int(x + 1, y);
                Vector2Int tl = new Vector2Int(x, y + 1);
                Vector2Int tr = new Vector2Int(x + 1, y + 1);

                ToggleEdge(edges, new Edge(bl, br)); // bottom
                ToggleEdge(edges, new Edge(br, tr)); // right
                ToggleEdge(edges, new Edge(tr, tl)); // top
                ToggleEdge(edges, new Edge(tl, bl)); // left
            }
        }

        return edges;
    }

    private void ToggleEdge(HashSet<Edge> edges, Edge edge)
    {
        if (!edges.Add(edge))
            edges.Remove(edge);
    }

    private List<List<Vector2>> BuildPathsFromEdges(HashSet<Edge> boundaryEdges, PixelObjectRoot root, float pixelSize, float halfPad)
    {
        Dictionary<Vector2Int, List<Vector2Int>> graph = new Dictionary<Vector2Int, List<Vector2Int>>();

        foreach (Edge edge in boundaryEdges)
        {
            AddConnection(graph, edge.a, edge.b);
            AddConnection(graph, edge.b, edge.a);
        }

        HashSet<(Vector2Int, Vector2Int)> usedDirectedEdges = new HashSet<(Vector2Int, Vector2Int)>();
        List<List<Vector2>> result = new List<List<Vector2>>();

        foreach (Edge edge in boundaryEdges)
        {
            if (usedDirectedEdges.Contains((edge.a, edge.b)) || usedDirectedEdges.Contains((edge.b, edge.a)))
                continue;

            List<Vector2Int> loop = TraceLoop(edge.a, edge.b, graph, usedDirectedEdges);
            if (loop.Count < 3) continue;

            List<Vector2> localPath = new List<Vector2>(loop.Count);

            for (int i = 0; i < loop.Count; i++)
            {
                Vector2 p = GridCornerToLocal(loop[i], root, pixelSize);

                if (halfPad > 0f)
                {
                    Vector2 dir = (p).normalized;
                    if (dir.sqrMagnitude > 0.000001f)
                        p += dir * halfPad;
                }

                localPath.Add(p);
            }

            result.Add(localPath);
        }

        return result;
    }

    private void AddConnection(Dictionary<Vector2Int, List<Vector2Int>> graph, Vector2Int from, Vector2Int to)
    {
        if (!graph.TryGetValue(from, out List<Vector2Int> list))
        {
            list = new List<Vector2Int>();
            graph[from] = list;
        }

        if (!list.Contains(to))
            list.Add(to);
    }

    private List<Vector2Int> TraceLoop(
        Vector2Int start,
        Vector2Int next,
        Dictionary<Vector2Int, List<Vector2Int>> graph,
        HashSet<(Vector2Int, Vector2Int)> usedDirectedEdges)
    {
        List<Vector2Int> loop = new List<Vector2Int>();
        loop.Add(start);

        Vector2Int prev = start;
        Vector2Int current = next;

        usedDirectedEdges.Add((start, next));

        while (true)
        {
            loop.Add(current);

            if (current == start)
                break;

            if (!graph.TryGetValue(current, out List<Vector2Int> neighbors) || neighbors.Count == 0)
                break;

            Vector2Int chosen = neighbors[0];

            if (neighbors.Count > 1)
            {
                bool found = false;
                for (int i = 0; i < neighbors.Count; i++)
                {
                    Vector2Int candidate = neighbors[i];
                    if (candidate == prev) continue;
                    if (usedDirectedEdges.Contains((current, candidate))) continue;

                    chosen = candidate;
                    found = true;
                    break;
                }

                if (!found)
                {
                    for (int i = 0; i < neighbors.Count; i++)
                    {
                        Vector2Int candidate = neighbors[i];
                        if (!usedDirectedEdges.Contains((current, candidate)))
                        {
                            chosen = candidate;
                            break;
                        }
                    }
                }
            }

            usedDirectedEdges.Add((current, chosen));
            prev = current;
            current = chosen;
        }

        return loop;
    }

    private Vector2 GridCornerToLocal(Vector2Int corner, PixelObjectRoot root, float pixelSize)
    {
        float localX = (corner.x - root.Width * 0.5f) * pixelSize;
        float localY = (corner.y - root.Height * 0.5f) * pixelSize;
        return new Vector2(localX, localY);
    }
}