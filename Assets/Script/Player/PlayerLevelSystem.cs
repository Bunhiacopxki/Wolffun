using System;

public class PlayerLevelSystem
{
    public int CurrentLevel { get; private set; } = 1;

    public void LevelUp()
    {
        CurrentLevel++;
    }

    public void RestartLevel()
    {
        CurrentLevel = 1;
    }
}