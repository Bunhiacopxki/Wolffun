using UnityEngine;
using static PixelObjectRoot;

public class PixelDamageSystem : MonoBehaviour
{
    [SerializeField] private PixelDebrisFactory debrisFactory;
    [SerializeField] private PixelFlashParticleFactory flashFactory;

    private PixelObjectRoot root;
    private PixelSplitSystem splitSystem;

    public void Initialize(PixelObjectRoot pixelRoot)
    {
        root = pixelRoot;
        splitSystem = pixelRoot != null ? pixelRoot.SplitSystem : null;
    }

    public void ApplySawDamage(Vector2 centerPoint, float outerRadius, float innerRadius, float dps, float deltaTime)
    {
        if (root == null || !root.Destructible) return;

        float multiplier = root.MaterialData != null ? root.MaterialData.sawDamageMultiplier : 1f;
        float maxDamage = dps * deltaTime * multiplier;
        float minDamage = maxDamage * 0.2f;
        Color materialColor = root.MaterialData != null ? root.MaterialData.color : Color.white;

        bool changed = false;

        float pixelHalfSize = 1.0f / GameManager.Instance.PixelsPerUnit;
        float minDistance = 2;
        for (int x = 0; x < root.Width; x++)
        {
            for (int y = 0; y < root.Height; y++)
            {
                if (!root.Cells[x, y].alive) continue;

                Vector2 pixelWorld = root.PixelToWorldCenter(x, y);
                float distance = Vector2.Distance(pixelWorld, centerPoint);
                if (minDistance > distance)
                {
                    minDistance = distance;
                }
                
                if (distance > outerRadius + pixelHalfSize) continue;
                if (distance < innerRadius - pixelHalfSize) continue;

                float t = Mathf.InverseLerp(innerRadius, outerRadius, distance);
                float damage = Mathf.Lerp(minDamage, maxDamage, t);

                PixelCell cell = root.Cells[x, y];
                cell.hp -= damage;

                if (cell.hp <= 0f)
                {
                    root.KillPixel(x, y);

                    GameManager.Instance.XpManager.AddXP(root.XpPerPixel);

                    if (debrisFactory != null)
                        debrisFactory.SpawnDebris(pixelWorld, materialColor);

                    changed = true;
                }
                else
                {
                    root.Cells[x, y] = cell;
                }

                if (flashFactory != null)
                {
                    flashFactory.SpawnFlash(pixelWorld, Color.white);
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
}