using System;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    [SerializeField] private LevelData _levelData;
    [SerializeField] private LevelSpawner _levelSpawner;
    [SerializeField] private WinConditionTracker _winConditionTracker;

    public event Action OnLevelWon;

    public int RemainObjectToSpawn => _levelSpawner != null ? _levelSpawner.RemainObject : 0;
    public LevelData CurrentLevel => _levelData;

    private void Start()
    {
        _levelSpawner.Setup(_levelData, _winConditionTracker);
        _levelSpawner.StartSpawning();
    }

    private void OnEnable()
    {
        _winConditionTracker.OnAllResolved += HandleLevelWon;
    }

    private void OnDisable()
    {
        _winConditionTracker.OnAllResolved -= HandleLevelWon;
    }

    private void HandleLevelWon()
    {
        Debug.Log($"WIN: {_levelData.levelId}");
        OnLevelWon?.Invoke();
    }
}