using UnityEngine;

public class BottomCollector : MonoBehaviour
{
    [SerializeField] private XPManager xpManager;

    private void Reset()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PixelObjectRoot root = other.GetComponentInParent<PixelObjectRoot>();
        if (root == null) return;

        int alivePixels = root.GetAlivePixelCount();
        int xp = alivePixels * root.XpPerPixel;

        if (xpManager != null)
            xpManager.AddXP(xp);

        root.ResolveAndDestroy();
    }
}