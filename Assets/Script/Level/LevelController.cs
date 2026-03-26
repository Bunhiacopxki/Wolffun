using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    [SerializeField] private LevelSpawner _levelSpawner;
    [SerializeField] private WinConditionTracker _winConditionTracker;
    [SerializeField] private LevelLayoutSpawner _levelLayoutSpawner;
    [SerializeField] private LevelProgressUI _levelProgressUI;

    public event Action OnLevelWon;
    private LevelData _levelData;

    public int RemainObjectToSpawn => _levelSpawner != null ? _levelSpawner.RemainObject : 0;
    public LevelData CurrentLevel => _levelData;

    public void BeginLevel(LevelData levelData)
    {
        _levelData = levelData;
        if (_winConditionTracker != null) _winConditionTracker.ResetTracker();
        BuildLevelLayout(_levelData);
        _levelSpawner.Setup(_levelData, _winConditionTracker);
        if (_levelProgressUI != null)
        {
            int levelNumber = GameManager.Instance != null ? GameManager.Instance.CurrentLevelIndex + 1 : 1;
            int totalObjects = _levelData != null ? _levelData.objectsToSpawn.Count : 0;

            _levelProgressUI.SetLevelText(levelNumber);
            _levelProgressUI.SetupSpawnProgress(totalObjects);
        }
        _levelSpawner.StartSpawning();
    }

    private void BuildLevelLayout(LevelData levelData)
    {
        GameManager.Instance.SawManager.ResetAllSaws();

        if (_levelLayoutSpawner != null)
            _levelLayoutSpawner.ClearLayout();

        List<SawSlot> spawnedSlots = new();

        if (_levelLayoutSpawner != null && levelData != null)
        {
            spawnedSlots = _levelLayoutSpawner.SpawnWeaponSlots(levelData.weaponSlots);
            _levelLayoutSpawner.SpawnObstacles(levelData.obstacles);
        }

        GameManager.Instance.SawManager.SetSlots(spawnedSlots);
    }


    private void OnEnable()
    {
        if (_winConditionTracker != null)
            _winConditionTracker.OnAllResolved += HandleLevelWon;

        if (_levelSpawner != null)
        {
            _levelSpawner.OnSpawnProgressChanged += HandleSpawnProgressChanged;
            _levelSpawner.OnSpawnCompleted += HandleSpawnCompleted;
        }
    }

    private void OnDisable()
    {
        if (_winConditionTracker != null)
            _winConditionTracker.OnAllResolved -= HandleLevelWon;

        if (_levelSpawner != null)
        {
            _levelSpawner.OnSpawnProgressChanged -= HandleSpawnProgressChanged;
            _levelSpawner.OnSpawnCompleted -= HandleSpawnCompleted;
        }
    }

    private void HandleSpawnProgressChanged(int spawnedCount, int totalCount)
    {
        if (_levelProgressUI != null)
            _levelProgressUI.UpdateSpawnProgress(spawnedCount);
    }

    private void HandleSpawnCompleted()
    {
        if (_levelProgressUI != null)
            _levelProgressUI.FillSpawnProgress();
    }

    private void HandleLevelWon()
    {
        Debug.Log($"WIN: {_levelData.levelId}");
        OnLevelWon?.Invoke();
    }
}