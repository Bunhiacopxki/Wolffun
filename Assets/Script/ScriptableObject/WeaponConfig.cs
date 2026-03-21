using UnityEngine;

[CreateAssetMenu(menuName = "PixelDestruction/Data/Weapon Config")]
public class WeaponConfig : ScriptableObject
{
    public float length = 1.25f;
    public float rotationSpeed = 180f;
    public float dps = 8f;
    public int bladeCount = 1;
}