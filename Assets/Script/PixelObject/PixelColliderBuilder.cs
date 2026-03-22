using UnityEngine;

public class PixelColliderBuilder : MonoBehaviour
{
    [SerializeField] private float colliderPadding = 0f;

    public void Rebuild(PixelObjectRoot root, BoxCollider2D boxCollider)
    {
        if (root == null || boxCollider == null) return;

        int minX = int.MaxValue;
        int minY = int.MaxValue;
        int maxX = int.MinValue;
        int maxY = int.MinValue;

        bool hasAlive = false;

        for (int x = 0; x < root.Width; x++)
        {
            for (int y = 0; y < root.Height; y++)
            {
                if (!root.Cells[x, y].alive) continue;

                hasAlive = true;
                if (x < minX) minX = x;
                if (y < minY) minY = y;
                if (x > maxX) maxX = x;
                if (y > maxY) maxY = y;
            }
        }

        if (!hasAlive)
        {
            boxCollider.enabled = false;
            return;
        }

        boxCollider.enabled = true;

        float width = (maxX - minX + 1);
        float height = (maxY - minY + 1);

        float centerX = (minX + maxX + 1) * 0.5f - root.Width * 0.5f;
        float centerY = (minY + maxY + 1) * 0.5f - root.Height * 0.5f;

        float unitPerPixel = 1f / GameManager.Instance.PixelsPerUnit;

        boxCollider.offset = new Vector2(
            centerX * unitPerPixel,
            centerY * unitPerPixel
        );

        boxCollider.size = new Vector2(
            width * unitPerPixel + colliderPadding,
            height * unitPerPixel + colliderPadding
        );
    }
}