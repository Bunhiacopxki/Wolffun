using System.Collections.Generic;
using UnityEngine;

public class PixelChunkFactory : MonoBehaviour
{
    [SerializeField] private PixelObjectRoot pixelChunkPrefab;

    public PixelObjectRoot CreateFromSpawnEntry(PixelObjectSpawnEntry entry)
    {
        PixelObjectRoot root = Instantiate(pixelChunkPrefab, entry.spawnPosition, Quaternion.identity);
        root.transform.localScale = Vector3.one * entry.scale;

        root.Initialize(
            entry.objectId,
            entry.shape.width,
            entry.shape.height,
            entry.shape.filledPixels,
            entry.materialData,
            entry.xpPerPixel,
            entry.destructible
        );

        return root;
    }

    public PixelObjectRoot CreateFragmentFromComponent(PixelObjectRoot source, List<Vector2Int> component)
    {
        if (pixelChunkPrefab == null || source == null || component == null || component.Count == 0)
            return null;

        GetBounds(component, out int minX, out int minY, out int maxX, out int maxY);

        int newWidth = maxX - minX + 1;
        int newHeight = maxY - minY + 1;

        List<Vector2Int> remappedPixels = new List<Vector2Int>(component.Count);
        for (int i = 0; i < component.Count; i++)
        {
            Vector2Int p = component[i];
            remappedPixels.Add(new Vector2Int(p.x - minX, p.y - minY));
        }

        Vector2 oldLocalCenter = GetGridBoundsCenterLocal(source.Width, source.Height, minX, minY, maxX, maxY);
        Vector2 newLocalCenter = GetGridBoundsCenterLocal(newWidth, newHeight, 0, 0, newWidth - 1, newHeight - 1);

        Vector2 worldOldCenter = source.transform.TransformPoint(oldLocalCenter);
        Vector2 worldNewOrigin = worldOldCenter - RotateAndScaleVector(newLocalCenter, source.transform);

        PixelObjectRoot fragment = Instantiate(pixelChunkPrefab, worldNewOrigin, source.transform.rotation);
        fragment.transform.localScale = source.transform.localScale;

        fragment.Initialize(
            source.GetObjectId() + "_Fragment",
            newWidth,
            newHeight,
            remappedPixels,
            source.MaterialData,
            source.XpPerPixel,
            source.Destructible
        );

        fragment.Rigidbody.velocity = source.Rigidbody.velocity;
        fragment.Rigidbody.angularVelocity = source.Rigidbody.angularVelocity;

        return fragment;
    }

    private void GetBounds(List<Vector2Int> pixels, out int minX, out int minY, out int maxX, out int maxY)
    {
        minX = int.MaxValue;
        minY = int.MaxValue;
        maxX = int.MinValue;
        maxY = int.MinValue;

        for (int i = 0; i < pixels.Count; i++)
        {
            Vector2Int p = pixels[i];

            if (p.x < minX) minX = p.x;
            if (p.y < minY) minY = p.y;
            if (p.x > maxX) maxX = p.x;
            if (p.y > maxY) maxY = p.y;
        }
    }

    private Vector2 GetGridBoundsCenterLocal(int gridWidth, int gridHeight, int minX, int minY, int maxX, int maxY)
    {
        float centerX = (minX + maxX + 1) * 0.5f - gridWidth * 0.5f;
        float centerY = (minY + maxY + 1) * 0.5f - gridHeight * 0.5f;

        float unitPerPixel = 1f / GameManager.Instance.PixelsPerUnit;
        return new Vector2(centerX * unitPerPixel, centerY * unitPerPixel);
    }

    private Vector2 RotateAndScaleVector(Vector2 localVector, Transform targetTransform)
    {
        Vector3 scaled = Vector3.Scale(localVector, targetTransform.lossyScale);
        return targetTransform.rotation * scaled;
    }
}