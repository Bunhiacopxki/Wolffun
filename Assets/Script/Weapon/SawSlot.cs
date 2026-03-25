using UnityEngine;

public class SawSlot : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _highlightRenderer;
    [SerializeField] private Collider2D _slotCollider;

    public bool IsOccupied { get; private set; }
    public SawWeapon CurrentSaw { get; private set; }

    public void SetHighlight(bool value)
    {
        if (_highlightRenderer != null)
            _highlightRenderer.enabled = value;
    }

    public void AssignSaw(SawWeapon saw)
    {
        CurrentSaw = saw;
        IsOccupied = saw != null;
    }

    public void ClearSaw()
    {
        CurrentSaw = null;
        IsOccupied = false;
        SetHighlight(true);
    }
}