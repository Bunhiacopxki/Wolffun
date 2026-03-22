using UnityEngine;

public class PixelDebrisFactory : MonoBehaviour
{
    [SerializeField] private PixelDebrisPiece debrisPrefab;
    [SerializeField] private float debrisScale = 0.15f;
    [SerializeField] private float launchSpeedMin = 1.0f;
    [SerializeField] private float launchSpeedMax = 3.0f;
    [SerializeField] private float angularSpeedMin = -180f;
    [SerializeField] private float angularSpeedMax = 180f;

    public void SpawnDebris(Vector2 worldPos, Color color, int count = 1)
    {
        if (debrisPrefab == null) return;

        for (int i = 0; i < count; i++)
        {
            PixelDebrisPiece piece = Instantiate(debrisPrefab, worldPos, Quaternion.identity);

            Vector2 dir = Random.insideUnitCircle.normalized;
            if (dir.sqrMagnitude < 0.001f)
                dir = Vector2.up;

            float speed = Random.Range(launchSpeedMin, launchSpeedMax);
            float angular = Random.Range(angularSpeedMin, angularSpeedMax);

            piece.Initialize(
                color,
                dir * speed,
                angular,
                debrisScale
            );
        }
    }
}