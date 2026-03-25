using System.Collections.Generic;
using UnityEngine;

public class LevelLayoutSpawner : MonoBehaviour
{
    [SerializeField] private SawSlot _sawSlotPrefab;
    [SerializeField] private Transform _slotParent;

    [SerializeField] private ObstacleView _obstaclePrefab;
    [SerializeField] private Transform _obstacleParent;

    private readonly List<GameObject> _spawnedLayoutObjects = new();

    public void ClearLayout()
    {
        for (int i = _spawnedLayoutObjects.Count - 1; i >= 0; i--)
        {
            if (_spawnedLayoutObjects[i] != null)
                Destroy(_spawnedLayoutObjects[i]);
        }

        _spawnedLayoutObjects.Clear();
    }

    public List<SawSlot> SpawnWeaponSlots(List<WeaponSlotData> slotDatas)
    {
        List<SawSlot> result = new();

        if (slotDatas == null || _sawSlotPrefab == null) return result;

        foreach (var data in slotDatas)
        {
            if (data == null) continue;

            SawSlot slot = Instantiate(
                _sawSlotPrefab,
                data.position,
                Quaternion.identity,
                _slotParent
            );

            result.Add(slot);
            _spawnedLayoutObjects.Add(slot.gameObject);
        }

        return result;
    }

    public void SpawnObstacles(List<ObstacleData> obstacleDatas)
    {
        if (obstacleDatas == null || _obstaclePrefab == null) return;

        foreach (var data in obstacleDatas)
        {
            if (data == null) continue;

            ObstacleView obstacle = Instantiate(
                _obstaclePrefab,
                data.position,
                Quaternion.identity,
                _obstacleParent
            );

            obstacle.Initialize(data);
            _spawnedLayoutObjects.Add(obstacle.gameObject);
        }
    }
}