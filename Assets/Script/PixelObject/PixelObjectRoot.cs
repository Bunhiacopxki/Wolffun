using System;
using System.Collections.Generic;
using UnityEngine;

public class PixelObjectRoot : MonoBehaviour
{
    public event Action<PixelObjectRoot> OnFragmentResolved;
    public event Action<PixelObjectRoot> OnFragmentSplitCreated;

    [Header("Debug")]
    [SerializeField] private string objectId;

    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private PolygonCollider2D polygonCollider;
    private PixelVisualBuilder visualBuilder;
    private PixelColliderBuilder colliderBuilder;
    private PixelDamageSystem damageSystem;
    private PixelSplitSystem splitSystem;

    public int Width { get; private set; }
    public int Height { get; private set; }
    public PixelCell[,] Cells { get; private set; }
    public MaterialData MaterialData { get; private set; }
    public int XpPerPixel { get; private set; }
    public bool Destructible { get; private set; }

    public Rigidbody2D Rigidbody => rb;
    public SpriteRenderer SpriteRenderer => spriteRenderer;
    public PolygonCollider2D PolygonCollider => polygonCollider;
    public PixelDamageSystem DamageSystem => damageSystem;
    public PixelSplitSystem SplitSystem => splitSystem;

    private bool isResolved;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        polygonCollider = GetComponent<PolygonCollider2D>();
        visualBuilder = GetComponent<PixelVisualBuilder>();
        colliderBuilder = GetComponent<PixelColliderBuilder>();
        damageSystem = GetComponent<PixelDamageSystem>();
        splitSystem = GetComponent<PixelSplitSystem>();
    }

    public void Initialize(string newObjectId, int width, int height, List<Vector2Int> filledPixels, MaterialData materialData, int xpPerPixel, bool destructible)
    {
        objectId = newObjectId;
        Width = Mathf.Max(1, width);
        Height = Mathf.Max(1, height);
        MaterialData = materialData;
        XpPerPixel = Mathf.Max(1, xpPerPixel);
        Destructible = destructible;

        Cells = new PixelCell[Width, Height];

        float initialHp = materialData != null ? materialData.pixelHP : 10f;

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                Cells[x, y] = new PixelCell(false, 0f);
            }
        }

        if (filledPixels != null)
        {
            for (int i = 0; i < filledPixels.Count; i++)
            {
                Vector2Int p = filledPixels[i];
                if (InBounds(p.x, p.y))
                {
                    Cells[p.x, p.y] = new PixelCell(true, initialHp);
                }
            }
        }

        rb.mass = Mathf.Max(0.1f, GetAlivePixelCount() * (materialData != null ? materialData.density : 1f) * 0.01f);

        damageSystem.Initialize(this);
        splitSystem.Initialize(this);

        Rebuild();
    }

    public bool InBounds(int x, int y)
    {
        return x >= 0 && x < Width && y >= 0 && y < Height;
    }

    public bool IsAlive(int x, int y)
    {
        return InBounds(x, y) && Cells[x, y].alive;
    }

    public void KillPixel(int x, int y)
    {
        if (!InBounds(x, y)) return;
        if (!Cells[x, y].alive) return;

        Cells[x, y].alive = false;
        Cells[x, y].hp = 0f;
    }

    public int GetAlivePixelCount()
    {
        int count = 0;

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                if (Cells[x, y].alive)
                    count++;
            }
        }

        return count;
    }

    public List<Vector2Int> GetAlivePixels()
    {
        List<Vector2Int> result = new();

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                if (Cells[x, y].alive)
                    result.Add(new Vector2Int(x, y));
            }
        }

        return result;
    }

    public Vector2 PixelToLocalCenter(int x, int y)
    {
        float unitPerPixel = 1f / GameManager.Instance.PixelsPerUnit;

        float localX = ((x + 0.5f) - Width * 0.5f) * unitPerPixel;
        float localY = ((y + 0.5f) - Height * 0.5f) * unitPerPixel;

        return new Vector2(localX, localY);
    }

    public Vector2 PixelToWorldCenter(int x, int y)
    {
        return transform.TransformPoint(PixelToLocalCenter(x, y));
    }

    public void Rebuild()
    {
        visualBuilder.Rebuild(this, spriteRenderer);
        colliderBuilder.Rebuild(this, polygonCollider);

        rb.mass = Mathf.Max(0.1f, GetAlivePixelCount() * (MaterialData != null ? MaterialData.density : 1f) * 0.01f);
    }

    public void NotifySplitCreated(PixelObjectRoot newFragment)
    {
        OnFragmentSplitCreated?.Invoke(newFragment);
    }

    public void ResolveAndDestroy()
    {
        if (isResolved) return;
        isResolved = true;

        OnFragmentResolved?.Invoke(this);
        Destroy(gameObject);
    }

    public string GetObjectId()
    {
        return objectId;
    }

    public struct PixelCell
    {
        public bool alive;
        public float hp;

        public PixelCell(bool alive, float hp)
        {
            this.alive = alive;
            this.hp = hp;
        }
    }
}