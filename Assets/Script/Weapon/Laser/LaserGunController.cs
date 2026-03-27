using UnityEngine;

public class LaserGunController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private LineRenderer lineRenderer;

    [Header("Laser Settings")]
    [SerializeField] private float maxDistance = 20f;
    [SerializeField] private LayerMask hitMask;
    [SerializeField] private float beamRadius = 0.08f;
    [SerializeField] private float dps = 40f;
    [SerializeField] private float sampleSpacing = 0.15f;

    private Camera mainCam;

    private void Awake()
    {
        mainCam = Camera.main;

        if (lineRenderer != null)
        {
            lineRenderer.enabled = false;
            lineRenderer.positionCount = 2;
        }
    }

    private void OnEnable()
    {
        if (lineRenderer != null)
            lineRenderer.enabled = false;

        if (mainCam == null)
            mainCam = Camera.main;
    }

    private void Update()
    {
        if (firePoint == null || mainCam == null)
            return;

        AimToMouse();

        bool isFiring = Input.GetMouseButton(0);

        if (!isFiring)
        {
            if (lineRenderer != null)
                lineRenderer.enabled = false;
            return;
        }

        FireLaser();
    }

    private void AimToMouse()
    {
        Vector3 mouseWorld = mainCam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = transform.position.z;

        Vector2 dir = (mouseWorld - firePoint.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void FireLaser()
    {
        Vector2 origin = firePoint.position;
        Vector2 dir = firePoint.right;

        RaycastHit2D hit = Physics2D.Raycast(origin, dir, maxDistance, hitMask);
        Vector2 endPoint = hit.collider != null
            ? hit.point
            : origin + dir * maxDistance;

        if (lineRenderer != null)
        {
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, new Vector3(origin.x, origin.y, -1f));
            lineRenderer.SetPosition(1, new Vector3(endPoint.x, endPoint.y, -1f));
        }

        LaserDamageUtility.ApplyLaserDamageAlongLine(
            origin,
            endPoint,
            beamRadius,
            dps,
            sampleSpacing
        );
    }

    public void StopLaserVisual()
    {
        if (lineRenderer != null)
            lineRenderer.enabled = false;
    }
}