using UnityEngine;

public class PixelFlashParticleFactory : MonoBehaviour
{
    [SerializeField] private ParticleSystem _flashPrefab;

    public void SpawnFlash(Vector2 worldPos, Color color)
    {
        if (_flashPrefab == null) return;

        ParticleSystem ps = Instantiate(_flashPrefab, worldPos, Quaternion.identity);

        var main = ps.main;
        main.startColor = color;

        ps.Play();

        Destroy(ps.gameObject, main.duration + main.startLifetime.constantMax + 0.1f);
    }
}