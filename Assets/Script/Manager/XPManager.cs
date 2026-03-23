using System;
using UnityEngine;

public class XPManager : MonoBehaviour
{
    public event Action<int, int> OnXPChanged;
    public event Action<int> OnLevelUp;

    [SerializeField] private ProgressionConfig _progressionConfig;
    private PlayerLevelSystem _playerLevelSystem;

    public int CurrentXP { get; private set; }
    public int RequiredXP { get; private set; }
    public int CurrentLevel => _playerLevelSystem.CurrentLevel;

    public void Initialize()
    {
        _playerLevelSystem = new PlayerLevelSystem();

        CurrentXP = 0;
        RequiredXP = _progressionConfig.GetRequiredXp(_playerLevelSystem.CurrentLevel);

        OnXPChanged?.Invoke(CurrentXP, RequiredXP);
    }


    public void AddXP(int amount)
    {
        if (amount <= 0) return;

        CurrentXP += amount;

        while (CurrentXP >= RequiredXP)
        {
            CurrentXP -= RequiredXP;
            _playerLevelSystem.LevelUp();

            OnLevelUp?.Invoke(_playerLevelSystem.CurrentLevel);

            RequiredXP = _progressionConfig.GetRequiredXp(_playerLevelSystem.CurrentLevel);
        }

        OnXPChanged?.Invoke(CurrentXP, RequiredXP);
    }
}