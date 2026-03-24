using UnityEngine;

public class TapToDestroyController : MonoBehaviour
{
    [Header("Tap Damage Config")]
    [SerializeField] private float radius = 0.1f;
    [SerializeField] private float maxDamage = 100f;
    [SerializeField] private float minDamage = 20f;
    [SerializeField] private LayerMask targetMask = ~0;

    private Camera mainCam;

    private void Awake()
    {
        mainCam = Camera.main;
    }

    private void Update()
    {
        if (mainCam == null) return;

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 worldPos = mainCam.ScreenToWorldPoint(Input.mousePosition);
            ApplyTapAt(worldPos);
        }
    }

    private void ApplyTapAt(Vector2 worldPos)
    {
        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero, 0f, targetMask);
        if (!hit.collider) return;

        PixelObjectRoot root = hit.collider.GetComponentInParent<PixelObjectRoot>();
        if (root == null) return;

        PixelDamageSystem damageSystem = root.DamageSystem;
        if (damageSystem == null) return;

        damageSystem.ApplyTapDamage(worldPos, radius, maxDamage, minDamage);
    }
}