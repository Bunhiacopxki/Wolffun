using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PixelDestruction/Data/Level Data")]
public class LevelData : ScriptableObject
{
    public string levelId;
    public float spawnInterval = 1.5f;

    public List<PixelObjectSpawnEntry> objectsToSpawn = new();
    public List<ObstacleData> obstacles = new();
    public List<WeaponSlotData> weaponSlots = new();
}

[System.Serializable]
public class PixelObjectSpawnEntry
{
    public string objectId = "Object";
    public PixelShapeData shape;
    public MaterialData materialData;
    public Vector2 spawnPosition;
    public float scale = 1f;
    public bool destructible = true;
    public int xpPerPixel = 1;
}

[System.Serializable]
public class ObstacleData
{
    public Vector2 position;
}

[System.Serializable]
public class WeaponSlotData
{
    public Vector2 position;
}