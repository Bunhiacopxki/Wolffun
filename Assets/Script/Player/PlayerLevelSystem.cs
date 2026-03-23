using System;

public class PlayerLevelSystem
{
    public event Action<int> OnLevelChanged;

    public int CurrentLevel { get; private set; } = 1;

    public void LevelUp()
    {
        CurrentLevel++;
        OnLevelChanged?.Invoke(CurrentLevel);
    }
}