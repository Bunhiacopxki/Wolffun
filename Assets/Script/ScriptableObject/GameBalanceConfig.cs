using UnityEngine;

[CreateAssetMenu(menuName = "PixelDestruction/Configs/Game Balance Config")]
public class GameBalanceConfig : ScriptableObject
{
    [Header("Tap Destroy")]
    public float tapRadius = 1.5f;
    public float tapMaxDamage = 10f;
    public float tapMinDamage = 3f;

    [Header("XP")]
    public int defaultXpPerPixel = 1;

    [Header("Fragment")]
    public int minPixelCountToKeepFragment = 1;

    [Header("Physics")]
    public float inheritedVelocityFactor = 1f;
    public float splitExplosionForce = 1.5f;
}