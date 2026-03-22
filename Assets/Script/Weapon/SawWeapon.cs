using UnityEngine;

public class SawWeapon : MonoBehaviour
{
    [Header("Saw Settings")]
    [SerializeField] private float hitRadius = 0.5f;
    [SerializeField] private float dps = 20f;

    private Vector2 lastPosition;
    private bool hasLastPosition;

    private void OnEnable()
    {
        lastPosition = transform.position;
        hasLastPosition = true;
    }

    private void LateUpdate()
    {
        lastPosition = transform.position;
        hasLastPosition = true;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        PixelObjectRoot root = other.GetComponentInParent<PixelObjectRoot>();
        if (root == null) return;
        PixelDamageSystem damageSystem = root.DamageSystem;
        if (damageSystem == null) return;
        Vector2 currentPosition = transform.position;
        Vector2 startPosition = hasLastPosition ? lastPosition : currentPosition;

        damageSystem.ApplySawDamageAlongPath(
            startPosition,
            currentPosition,
            hitRadius,
            dps,
            Time.deltaTime
        );
    }
}