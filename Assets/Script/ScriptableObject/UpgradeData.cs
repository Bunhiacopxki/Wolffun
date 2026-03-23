using UnityEngine;

public enum UpgradeType
{
    IncreaseSawSize,
    IncreaseSawRotationSpeed,
    IncreaseSawDps,
    AddNewSaw
}

[CreateAssetMenu(menuName = "PixelDestruction/Data/Upgrade Data")]
public class UpgradeData : ScriptableObject
{
    public string upgradeId;
    public string displayName;
    [TextArea] public string description;
    public UpgradeType type;
    public float value = 1f;
    public Sprite icon;
}