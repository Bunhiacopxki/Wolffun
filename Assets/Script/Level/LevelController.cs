using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    [SerializeField] private LevelSpawner _levelSpawner;
    [SerializeField] private WinConditionTracker _winConditionTracker;
    [SerializeField] private LevelLayoutSpawner _levelLayoutSpawner;
    [SerializeField] private SawManager _sawManager;

    public event Action OnLevelWon;
    private LevelData _levelData;

    public int RemainObjectToSpawn => _levelSpawner != null ? _levelSpawner.RemainObject : 0;
    public LevelData CurrentLevel => _levelData;

    public void BeginLevel(LevelData levelData)
    {
        _levelData = levelData;
        BuildLevelLayout(_levelData);
        _levelSpawner.Setup(_levelData, _winConditionTracker);
        _levelSpawner.StartSpawning();
    }

    private void BuildLevelLayout(LevelData levelData)
    {
        if (_sawManager != null)
            _sawManager.ResetAllSaws();

        if (_levelLayoutSpawner != null)
            _levelLayoutSpawner.ClearLayout();

        List<SawSlot> spawnedSlots = new();

        if (_levelLayoutSpawner != null && levelData != null)
        {
            spawnedSlots = _levelLayoutSpawner.SpawnWeaponSlots(levelData.weaponSlots);
            _levelLayoutSpawner.SpawnObstacles(levelData.obstacles);
        }

        if (_sawManager != null)
            _sawManager.SetSlots(spawnedSlots);
    }


    private void OnEnable()
    {
        if (_winConditionTracker != null)
            _winConditionTracker.OnAllResolved += HandleLevelWon;
    }

    private void OnDisable()
    {
        if (_winConditionTracker != null)
            _winConditionTracker.OnAllResolved -= HandleLevelWon;
    }

    private void HandleLevelWon()
    {
        Debug.Log($"WIN: {_levelData.levelId}");
        OnLevelWon?.Invoke();
    }
}