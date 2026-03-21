using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PixelDestruction/Data/Pixel Shape Data")]
public class PixelShapeData : ScriptableObject
{
    public int width = 8;
    public int height = 8;
    public List<Vector2Int> filledPixels = new();

    public bool Contains(int x, int y)
    {
        return filledPixels.Contains(new Vector2Int(x, y));
    }
}