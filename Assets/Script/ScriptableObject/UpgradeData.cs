using UnityEngine;

public enum UpgradeType
{
    SawLength,
    SawRotationSpeed,
    SawDps,
    AddNewSaw,
    AddBladeToAllSaws
}

[CreateAssetMenu(menuName = "PixelDestruction/Data/Upgrade Data")]
public class UpgradeData : ScriptableObject
{
    public string upgradeId;
    public string displayName;
    [TextArea] public string description;
    public Sprite icon;
    public UpgradeType type;
    public float value = 1f;
}