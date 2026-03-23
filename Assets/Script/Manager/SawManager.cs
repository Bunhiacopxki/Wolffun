using System;
using System.Collections.Generic;
using UnityEngine;

public class SawManager : MonoBehaviour
{
    [SerializeField] private SawWeapon _sawPrefab;
    [SerializeField] private List<SawWeapon> _activeSaws = new();
    [SerializeField] private List<SawSlot> _allSlots = new();

    public IReadOnlyList<SawSlot> AllSlots => _allSlots;

    public void ApplyToAllSaws(Action<SawWeapon> action)
    {
        foreach (var saw in _activeSaws)
        {
            if (saw == null) continue;
            action?.Invoke(saw);
        }
    }

    public bool TrySpawnSawAtSlot(SawSlot slot)
    {
        if (slot == null) return false;
        if (slot.IsOccupied) return false;
        if (_sawPrefab == null) return false;

        SawWeapon newSaw = Instantiate(_sawPrefab, slot.transform.position, Quaternion.identity, transform);
        _activeSaws.Add(newSaw);
        slot.AssignSaw(newSaw);

        return true;
    }

    public List<SawSlot> GetEmptySlots()
    {
        List<SawSlot> result = new();

        foreach (var slot in _allSlots)
        {
            if (slot != null && !slot.IsOccupied)
                result.Add(slot);
        }

        return result;
    }
}