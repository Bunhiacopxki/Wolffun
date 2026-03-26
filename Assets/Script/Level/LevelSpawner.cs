using System;
using System.Collections;
using UnityEngine;

public class LevelSpawner : MonoBehaviour
{
    [SerializeField] private PixelChunkFactory _chunkFactory;

    private LevelData _currentLevel;
    private WinConditionTracker _tracker;
    private Coroutine _spawnCoroutine;
    private int _remainObject;
    private int _spawnedObjectCount;
    private int _totalObjectCount;

    public int RemainObject => _remainObject;
    public int SpawnedObjectCount => _spawnedObjectCount;
    public int TotalObjectCount => _totalObjectCount;

    public event Action<int, int> OnSpawnProgressChanged;
    public event Action OnSpawnCompleted;

    public void Setup(LevelData levelData, WinConditionTracker winConditionTracker)
    {
        _currentLevel = levelData;
        _tracker = winConditionTracker;
        _totalObjectCount = _currentLevel != null ? _currentLevel.objectsToSpawn.Count : 0;
        _remainObject = _totalObjectCount;
        _spawnedObjectCount = 0;

        OnSpawnProgressChanged?.Invoke(_spawnedObjectCount, _totalObjectCount);
    }

    public void StartSpawning()
    {
        if (_currentLevel == null) return;
        if (_remainObject == 0)
        {
            OnSpawnCompleted?.Invoke();
            _tracker?.MarkSpawnFinished();
            return;
        }
        if (_spawnCoroutine != null) StopCoroutine(_spawnCoroutine);
        _spawnCoroutine = StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        for (int i = 0; i < _currentLevel.objectsToSpawn.Count; i++)
        {
            PixelObjectSpawnEntry entry = _currentLevel.objectsToSpawn[i];

            _tracker?.RegisterScheduledObject();

            PixelObjectRoot root = _chunkFactory.CreateFromSpawnEntry(entry);
            if (root != null)
            {
                _tracker?.RegisterActiveFragment(root);
            }
            _spawnedObjectCount++;
            _remainObject--;
            OnSpawnProgressChanged?.Invoke(_spawnedObjectCount, _totalObjectCount);
            if (i < _currentLevel.objectsToSpawn.Count - 1)
                yield return new WaitForSeconds(_currentLevel.spawnInterval);
        }

        OnSpawnCompleted?.Invoke();
        _tracker?.MarkSpawnFinished();
        _spawnCoroutine = null;
    }
}