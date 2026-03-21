using UnityEngine;

[CreateAssetMenu(menuName = "PixelDestruction/Configs/Progression Config")]
public class ProgressionConfig : ScriptableObject
{
    public int baseXpRequired = 100;
    public int xpIncreasePerLevel = 50;

    public int GetXpRequired(int level)
    {
        level = Mathf.Max(1, level);
        return baseXpRequired + (level - 1) * xpIncreasePerLevel;
    }
}