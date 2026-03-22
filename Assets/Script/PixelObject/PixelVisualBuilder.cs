using UnityEngine;

public class PixelVisualBuilder : MonoBehaviour
{
    

    public void Rebuild(PixelObjectRoot root, SpriteRenderer rendererTarget)
    {
        if (root == null || rendererTarget == null) return;

        Texture2D texture = new Texture2D(root.Width, root.Height, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;

        Color clear = new Color(0f, 0f, 0f, 0f);
        Color fillColor = root.MaterialData != null ? root.MaterialData.color : Color.white;

        for (int x = 0; x < root.Width; x++)
        {
            for (int y = 0; y < root.Height; y++)
            {
                texture.SetPixel(x, y, root.Cells[x, y].alive ? fillColor : clear);
            }
        }

        texture.Apply();

        Rect rect = new Rect(0, 0, root.Width, root.Height);
        Vector2 pivot = new Vector2(0.5f, 0.5f);

        Sprite sprite = Sprite.Create(texture, rect, pivot, GameManager.Instance.PixelsPerUnit);
        rendererTarget.sprite = sprite;
    }
}