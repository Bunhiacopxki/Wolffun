using UnityEngine;

[CreateAssetMenu(menuName = "PixelDestruction/Config/Progression Config")]
public class ProgressionConfig : ScriptableObject
{
    public int baseXpRequired = 10;
    public int xpIncreasePerLevel = 5;

    public int GetRequiredXp(int level)
    {
        level = Mathf.Max(1, level);
        return baseXpRequired + (level - 1) * xpIncreasePerLevel;
    }
}