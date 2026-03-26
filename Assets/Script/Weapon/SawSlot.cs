using UnityEngine;

public class SawSlot : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _normalRenderer;
    [SerializeField] private Collider2D _slotCollider;
    [SerializeField] private Animator _upgradeSaw;

    private void Awake()
    {
        _upgradeSaw.updateMode = AnimatorUpdateMode.UnscaledTime;
    }

    public bool IsOccupied { get; private set; }
    public SawWeapon CurrentSaw { get; private set; }

    public void SetHighlight(bool value)
    {
        if (_normalRenderer != null)
            _normalRenderer.enabled = value;
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

    public void ToggleAnim(bool state)
    {
        _upgradeSaw.SetBool("isUpgrade", state);
    }
}