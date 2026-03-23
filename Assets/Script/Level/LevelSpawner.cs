using System.Collections;
using UnityEngine;

public class LevelSpawner : MonoBehaviour
{
    [SerializeField] private PixelChunkFactory _chunkFactory;

    private LevelData _currentLevel;
    private WinConditionTracker _tracker;
    private Coroutine _spawnCoroutine;
    private int _remainObject;

    public int RemainObject => _remainObject;

    public void Setup(LevelData levelData, WinConditionTracker winConditionTracker)
    {
        _currentLevel = levelData;
        _tracker = winConditionTracker;
        _remainObject = _currentLevel != null ? _currentLevel.objectsToSpawn.Count : 0;
    }

    public void StartSpawning()
    {
        if (_remainObject == 0) return;
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

            _remainObject--;
            yield return new WaitForSeconds(_currentLevel.spawnInterval);
        }

        _tracker?.MarkSpawnFinished();
    }
}