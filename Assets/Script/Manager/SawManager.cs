using System;
using System.Collections.Generic;
using UnityEngine;

public class SawManager : MonoBehaviour
{
    [SerializeField] private SawWeapon _sawPrefab;
    [SerializeField] private List<SawWeapon> _activeSaws = new();
    private readonly List<SawSlot> _allSlots = new();

    public IReadOnlyList<SawSlot> AllSlots => _allSlots;

    public void SetSlots(IEnumerable<SawSlot> slots)
    {
        _allSlots.Clear();

        if (slots == null) return;

        foreach (var slot in slots)
        {
            if (slot == null) continue;
            _allSlots.Add(slot);
        }
    }

    public void ApplyToAllSaws(Action<SawWeapon> action)
    {
        for (int i = _activeSaws.Count - 1; i >= 0; i--)
        {
            SawWeapon saw = _activeSaws[i];

            if (saw == null)
            {
                _activeSaws.RemoveAt(i);
                continue;
            }

            if (!saw.IsFromSlot) continue;
            action?.Invoke(saw);
        }
    }

    public bool TrySpawnSawAtSlot(SawSlot slot)
    {
        if (slot == null) return false;
        if (slot.IsOccupied) return false;
        if (_sawPrefab == null) return false;
        if (!_allSlots.Contains(slot)) return false;

        SawWeapon newSaw = Instantiate(_sawPrefab, slot.transform.position, Quaternion.identity, transform);
        newSaw.InitializeFromSlot(slot);

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

    public void ResetAllSaws()
    {
        for (int i = _activeSaws.Count - 1; i >= 0; i--)
        {
            SawWeapon saw = _activeSaws[i];

            if (saw == null)
            {
                _activeSaws.RemoveAt(i);
                continue;
            }

            if (!saw.IsFromSlot) continue;

            saw.ResetToBaseStats();

            SawSlot slot = saw.OwnerSlot;
            if (slot != null && slot.CurrentSaw == saw)
                slot.ClearSaw();

            Destroy(saw.gameObject);
            _activeSaws.RemoveAt(i);
        }
    }
}