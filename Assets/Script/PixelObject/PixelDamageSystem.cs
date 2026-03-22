using UnityEngine;
using static PixelObjectRoot;

public class PixelDamageSystem : MonoBehaviour
{
    [SerializeField] private PixelDebrisFactory debrisFactory;

    private int debrisPerKilledPixel = 1;
    private PixelObjectRoot root;
    private PixelSplitSystem splitSystem;

    public void Initialize(PixelObjectRoot pixelRoot)
    {
        root = pixelRoot;
        splitSystem = pixelRoot != null ? pixelRoot.SplitSystem : null;
    }

    public void ApplySawDamageAlongPath(Vector2 startPoint, Vector2 endPoint, float radius, float dps, float deltaTime)
    {
        if (root == null || !root.Destructible) return;

        float multiplier = root.MaterialData != null ? root.MaterialData.sawDamageMultiplier : 1f;
        float maxDamage = dps * deltaTime * multiplier;
        float minDamage = maxDamage * 0.2f;

        bool changed = false;

        for (int x = 0; x < root.Width; x++)
        {
            for (int y = 0; y < root.Height; y++)
            {
                if (!root.Cells[x, y].alive) continue;

                Vector2 pixelWorld = root.PixelToWorldCenter(x, y);

                float distance = DistancePointToSegment(pixelWorld, startPoint, endPoint);
                if (distance > radius) continue;

                float damage = EvaluateFalloff(distance, radius, maxDamage, minDamage);

                PixelCell cell = root.Cells[x, y];
                cell.hp -= damage;

                if (cell.hp <= 0f)
                {
                    root.KillPixel(x, y);
                    changed = true;
                }
                else
                {
                    root.Cells[x, y] = cell;
                }
            }
        }

        if (changed)
        {
            if (root.GetAlivePixelCount() <= 0)
            {
                root.ResolveAndDestroy();
                return;
            }

            root.Rebuild();
            splitSystem?.CheckAndSplit();
        }
    }

    private float EvaluateFalloff(float distance, float radius, float maxDamage, float minDamage)
    {
        float t = Mathf.Clamp01(distance / Mathf.Max(radius, 0.0001f));
        return Mathf.Lerp(maxDamage, minDamage, t);
    }

    private float DistancePointToSegment(Vector2 point, Vector2 a, Vector2 b)
    {
        Vector2 ab = b - a;
        float abSqr = ab.sqrMagnitude;

        if (abSqr <= 0.000001f)
            return Vector2.Distance(point, a);

        float t = Vector2.Dot(point - a, ab) / abSqr;
        t = Mathf.Clamp01(t);

        Vector2 closest = a + ab * t;
        return Vector2.Distance(point, closest);
    }
}