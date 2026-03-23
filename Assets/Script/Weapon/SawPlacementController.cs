using System;
using System.Collections.Generic;
using UnityEngine;

public class SawPlacementController : MonoBehaviour
{
    [SerializeField] private SawManager _sawManager;
    [SerializeField] private Camera _mainCamera;

    public event Action OnPlacementFinished;

    private bool _isChoosingSlot;
    private List<SawSlot> _availableSlots = new();

    public void BeginChooseEmptySlotMode()
    {
        _availableSlots = _sawManager.GetEmptySlots();

        if (_availableSlots.Count == 0)
        {
            OnPlacementFinished?.Invoke();
            return;
        }

        _isChoosingSlot = true;

        foreach (var slot in _availableSlots)
        {
            if (slot == null) continue;
            slot.SetHighlight(true);
        }
    }

    private void Update()
    {
        if (!_isChoosingSlot) return;

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 world = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(world, Vector2.zero);

            if (hit.collider == null) return;

            SawSlot slot = hit.collider.GetComponentInParent<SawSlot>();
            if (slot == null) return;
            if (!_availableSlots.Contains(slot)) return;

            bool success = _sawManager.TrySpawnSawAtSlot(slot);
            if (success)
                EndChooseMode();
        }
    }

    private void EndChooseMode()
    {
        _isChoosingSlot = false;

        foreach (var slot in _availableSlots)
        {
            if (slot == null) continue;
            slot.SetHighlight(false);
        }

        _availableSlots.Clear();
        OnPlacementFinished?.Invoke();
    }
}