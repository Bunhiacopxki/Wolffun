using UnityEngine;

[CreateAssetMenu(menuName = "PixelDestruction/Data/Material Data")]
public class MaterialData : ScriptableObject
{
    public string materialId;
    public Color color = Color.white;
    public float pixelHP = 10f;
    public float density = 1f;
    public float sawDamageMultiplier = 1f;
    public float tapDamageMultiplier = 1f;
}