using System.Collections.Generic;
using UnityEngine;

public class PixelSplitSystem : MonoBehaviour
{
    [SerializeField] private PixelChunkFactory chunkFactory;

    private PixelObjectRoot root;

    public void Initialize(PixelObjectRoot pixelRoot)
    {
        root = pixelRoot;
    }

    public void CheckAndSplit()
    {
        if (root == null || chunkFactory == null) return;

        List<List<Vector2Int>> components = FindConnectedComponents();

        if (components.Count <= 1)
            return;

        components.Sort((a, b) => b.Count.CompareTo(a.Count));

        List<Vector2Int> keepComponent = components[0];
        HashSet<Vector2Int> keepSet = new HashSet<Vector2Int>(keepComponent);

        for (int x = 0; x < root.Width; x++)
        {
            for (int y = 0; y < root.Height; y++)
            {
                if (!root.Cells[x, y].alive) continue;

                Vector2Int p = new Vector2Int(x, y);
                if (!keepSet.Contains(p))
                {
                    root.KillPixel(x, y);
                }
            }
        }

        root.Rebuild();

        for (int i = 1; i < components.Count; i++)
        {
            PixelObjectRoot fragment = chunkFactory.CreateFragmentFromComponent(root, components[i]);
            if (fragment != null)
            {
                root.NotifySplitCreated(fragment);
            }
        }

        if (root.GetAlivePixelCount() <= 0)
        {
            root.ResolveAndDestroy();
        }
    }

    private List<List<Vector2Int>> FindConnectedComponents()
    {
        List<List<Vector2Int>> result = new();
        bool[,] visited = new bool[root.Width, root.Height];

        Vector2Int[] dirs =
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };

        for (int x = 0; x < root.Width; x++)
        {
            for (int y = 0; y < root.Height; y++)
            {
                if (visited[x, y]) continue;
                if (!root.Cells[x, y].alive) continue;

                List<Vector2Int> component = new();
                Queue<Vector2Int> queue = new();

                queue.Enqueue(new Vector2Int(x, y));
                visited[x, y] = true;

                while (queue.Count > 0)
                {
                    Vector2Int current = queue.Dequeue();
                    component.Add(current);

                    for (int i = 0; i < dirs.Length; i++)
                    {
                        Vector2Int next = current + dirs[i];
                        if (!root.InBounds(next.x, next.y)) continue;
                        if (visited[next.x, next.y]) continue;
                        if (!root.Cells[next.x, next.y].alive) continue;

                        visited[next.x, next.y] = true;
                        queue.Enqueue(next);
                    }
                }

                result.Add(component);
            }
        }

        return result;
    }
}