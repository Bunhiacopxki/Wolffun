using UnityEngine;

public class PixelDebrisPiece : MonoBehaviour
{
    [SerializeField] private float _lifeTime = 1.5f;
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private SpriteRenderer _sr;

    public void Initialize(Color color, Vector2 velocity, float angularVelocity, float scale)
    {
        if (_sr != null)
            _sr.color = color;

        transform.localScale = Vector3.one * scale;

        if (_rb != null)
        {
            _rb.velocity = velocity;
            _rb.angularVelocity = angularVelocity;
        }

        Destroy(gameObject, _lifeTime);
    }
}