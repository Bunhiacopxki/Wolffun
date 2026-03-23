using UnityEngine;

public class SawWeapon : MonoBehaviour
{
    [Header("Saw Settings")]
    [SerializeField] private float _hitRadius = 0.25f;
    [SerializeField] private float _dps = 20f;
    [SerializeField] private float _rotationSpeed = 180f;

    [Header("Visual")]
    [SerializeField] private Transform _visualRoot;
    [SerializeField] private SpriteRenderer _visualSprite;

    [Header("Trigger")]
    [SerializeField] private CapsuleCollider2D _triggerCollider;

    private float _currentAngle;
    private Vector2 _lastPosition;
    private bool _hasLastPosition;

    private void OnEnable()
    {
        _lastPosition = transform.position;
        _hasLastPosition = true;
        RefreshVisualAndCollider();
    }

    private void Update()
    {
        _currentAngle += _rotationSpeed * Time.deltaTime;
        _currentAngle %= 360f;

        if (_visualRoot != null)
            _visualRoot.localRotation = Quaternion.Euler(0f, 0f, _currentAngle);
    }

    private void LateUpdate()
    {
        _lastPosition = transform.position;
        _hasLastPosition = true;
    }

    public void AddLength(float value)
    {
        _hitRadius += value * 0.1f;
        _hitRadius = Mathf.Max(0.05f, _hitRadius);
        RefreshVisualAndCollider();
    }

    public void AddRotationSpeed(float value)
    {
        _rotationSpeed += value;
    }

    public void AddDps(float value)
    {
        _dps = Mathf.Max(0f, _dps + value);
    }

    private void RefreshVisualAndCollider()
    {
        RefreshVisual();
        RefreshCollider();
    }

    private void RefreshVisual()
    {
        if (_visualRoot == null || _visualSprite == null || _visualSprite.sprite == null)
            return;

        float targetDiameterWorld = _hitRadius * 2f;

        Vector2 spriteSize = _visualSprite.sprite.bounds.size;
        float baseDiameterWorldAtScale1 = Mathf.Max(spriteSize.x, spriteSize.y);
        if (baseDiameterWorldAtScale1 <= 0.0001f) return;

        float parentLossyScale = 1f;
        if (_visualRoot.parent != null)
        {
            Vector3 ps = _visualRoot.parent.lossyScale;
            parentLossyScale = Mathf.Max(Mathf.Abs(ps.x), Mathf.Abs(ps.y));
        }

        float localUniformScale = targetDiameterWorld / (baseDiameterWorldAtScale1 * parentLossyScale);
        _visualRoot.localScale = new Vector3(localUniformScale, localUniformScale, 1f);
    }

    private void RefreshCollider()
    {
        if (_triggerCollider == null) return;

        Vector3 ls = _triggerCollider.transform.lossyScale;
        float lossyX = Mathf.Abs(ls.x);
        float lossyY = Mathf.Abs(ls.y);

        if (lossyX <= 0.0001f) lossyX = 1f;
        if (lossyY <= 0.0001f) lossyY = 1f;

        float diameter = _hitRadius * 2f;

        _triggerCollider.size = new Vector2(
            diameter / lossyX,
            diameter / lossyY
        );
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        PixelObjectRoot root = other.GetComponentInParent<PixelObjectRoot>();
        if (root == null) return;

        PixelDamageSystem damageSystem = root.DamageSystem;
        if (damageSystem == null) return;

        Vector2 damageCenter = _triggerCollider != null
            ? _triggerCollider.bounds.center
            : (Vector2)transform.position;

        float innerRadius = _hitRadius * 0.65f;

        damageSystem.ApplySawDamage(
            damageCenter,
            _hitRadius,
            innerRadius,
            _dps,
            Time.fixedDeltaTime
        );
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Vector3 center = transform.position;
        if (_triggerCollider != null)
            center = _triggerCollider.bounds.center;

        Gizmos.DrawWireSphere(center, _hitRadius);
    }
}