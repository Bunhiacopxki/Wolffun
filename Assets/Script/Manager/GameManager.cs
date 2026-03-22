using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public enum GameState
{
    None,
    Playing,
    Paused,
    LevelUpSelection,
    Win
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Runtime")]
    [SerializeField] private LevelController _levelController;
    [SerializeField] private XPManager _xpManager;
    [Header("Pixel")]
    [SerializeField] private float _pixelsPerUnit = 8f;
    [SerializeField] private PixelChunkFactory _pixelChunkFactory;

    public float PixelsPerUnit => _pixelsPerUnit;

    public GameState CurrentState { get; private set; } = GameState.None;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        CurrentState = GameState.Playing;
        Instantiate(_pixelChunkFactory, gameObject.transform.position, Quaternion.identity);
    }
}
