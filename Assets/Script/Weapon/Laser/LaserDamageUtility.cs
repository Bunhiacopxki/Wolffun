using UnityEngine;

public static class LaserDamageUtility
{
    public static void ApplyLaserDamageAlongLine(
        Vector2 start,
        Vector2 end,
        float radius,
        float damagePerFrame,
        float sampleSpacing)
    {
        float distance = Vector2.Distance(start, end);
        if (distance <= 0.0001f)
            return;

        int sampleCount = Mathf.Max(1, Mathf.CeilToInt(distance / Mathf.Max(sampleSpacing, 0.01f)));
        Vector2 dir = (end - start).normalized;

        for (int i = 0; i <= sampleCount; i++)
        {
            Vector2 point = start + dir * (distance * i / sampleCount);

            Collider2D[] hits = Physics2D.OverlapCircleAll(point, radius);
            for (int h = 0; h < hits.Length; h++)
            {
                PixelDamageSystem damageSystem = hits[h].GetComponentInParent<PixelDamageSystem>();
                if (damageSystem == null)
                    continue;

                damageSystem.ApplyTapDamage(
                    point,
                    radius,
                    damagePerFrame,
                    damagePerFrame
                );
            }
        }
    }
}