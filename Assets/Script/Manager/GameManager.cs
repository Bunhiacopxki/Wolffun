using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Global")]
    [SerializeField] private int _pixelsPerUnit = 16;
    public int PixelsPerUnit => _pixelsPerUnit;

    [Header("Managers")]
    [SerializeField] private XPManager _xpManager;
    [SerializeField] private LevelController _levelController;
    [SerializeField] private UpgradeManager _upgradeManager;
    [SerializeField] private SawPlacementController _sawPlacementController;
    [SerializeField] private SawManager _sawManager;

    [Header("Level Sequence")]
    [SerializeField] private List<LevelData> _allLevels = new();
    [SerializeField] private int _startLevelIndex = 0;
    [SerializeField] private GameObject _winPanel;

    [Header("Debug")]
    [SerializeField] private bool _logStateChanges = true;

    public int CurrentLevelIndex { get; private set; }
    public XPManager XpManager => _xpManager;
    public SawManager SawManager => _sawManager;
    public LevelData CurrentLevelData
    {
        get
        {
            if (_allLevels == null) return null;
            if (CurrentLevelIndex < 0 || CurrentLevelIndex >= _allLevels.Count) return null;
            return _allLevels[CurrentLevelIndex];
        }
    }

    public bool IsPausedByUpgrade { get; private set; }
    public bool IsChoosingSawSlot { get; private set; }

    private bool _isInitialized;
    private bool _waitingForPlacementToResume;
    private bool _isTransitioningLevel;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        Time.timeScale = 1f;
        _winPanel.SetActive(false);
    }

    private void Start()
    {
        Initialize();
    }

    private void OnEnable()
    {
        if (_xpManager != null)
            _xpManager.OnLevelUp += HandlePlayerLevelUp;

        if (_levelController != null)
            _levelController.OnLevelWon += HandleLevelWon;

        if (_sawPlacementController != null)
            _sawPlacementController.OnPlacementFinished += HandleSawPlacementFinished;
    }

    private void OnDisable()
    {
        if (_xpManager != null)
            _xpManager.OnLevelUp -= HandlePlayerLevelUp;

        if (_levelController != null)
            _levelController.OnLevelWon -= HandleLevelWon;

        if (_sawPlacementController != null)
            _sawPlacementController.OnPlacementFinished -= HandleSawPlacementFinished;
    }

    private void Initialize()
    {
        if (_isInitialized || _xpManager == null || _levelController == null || _upgradeManager == null || _allLevels == null || _allLevels.Count == 0) return;

        CurrentLevelIndex = Mathf.Clamp(_startLevelIndex, 0, _allLevels.Count - 1);

        _xpManager.Initialize();
        BeginCurrentLevel();

        _isInitialized = true;
    }

    private void BeginCurrentLevel()
    {
        LevelData levelData = CurrentLevelData;
        if (levelData == null)
        {
            Debug.LogError("CurrentLevelData is null.");
            return;
        }

        if (_logStateChanges)
            Debug.Log($"[GameManager] Begin Level Index={CurrentLevelIndex}, Id={levelData.levelId}");

        _isTransitioningLevel = false;
        IsChoosingSawSlot = false;
        _waitingForPlacementToResume = false;

        ResumeGame();
        _levelController.BeginLevel(levelData);
    }

    private void HandlePlayerLevelUp(int newLevel)
    {
        if (_isTransitioningLevel) return;

        if (_logStateChanges)
            Debug.Log($"[GameManager] Player Level Up => {newLevel}");

        PauseGame();

        _upgradeManager.ShowRandomUpgradeSelection(OnUpgradeSelectionClosed);
    }

    private void OnUpgradeSelectionClosed()
    {
        if (_logStateChanges)
            Debug.Log("[GameManager] Upgrade selection closed.");

        UpgradeData selected = _upgradeManager.LastSelectedUpgrade;

        if (selected == null)
        {
            if (_logStateChanges)
                Debug.LogWarning("[GameManager] No upgrade selected. Resume game.");

            ResumeGame();
            return;
        }

        if (selected.type == UpgradeType.AddNewSaw)
        {
            if (_sawPlacementController == null)
            {
                Debug.LogWarning("[GameManager] Missing SawPlacementController. Resume game.");
                ResumeGame();
                return;
            }

            IsChoosingSawSlot = true;
            _waitingForPlacementToResume = true;

            if (_logStateChanges)
                Debug.Log("[GameManager] Enter choose saw slot mode.");

            _sawPlacementController.BeginChooseEmptySlotMode();
            return;
        }

        ResumeGame();
    }

    private void HandleSawPlacementFinished()
    {
        if (!IsChoosingSawSlot && !_waitingForPlacementToResume)
            return;

        if (_logStateChanges)
            Debug.Log("[GameManager] Saw placement finished.");

        IsChoosingSawSlot = false;

        if (_waitingForPlacementToResume)
        {
            _waitingForPlacementToResume = false;
            ResumeGame();
        }
    }

    private void HandleLevelWon()
    {
        if (_isTransitioningLevel) return;

        _isTransitioningLevel = true;

        if (_logStateChanges)
            Debug.Log($"[GameManager] Level Won => {CurrentLevelData?.levelId}");

        GoToNextLevel();
    }

    public void GoToNextLevel()
    {
        int nextIndex = CurrentLevelIndex + 1;

        if (nextIndex >= _allLevels.Count)
        {
            if (_logStateChanges)
                Debug.Log("[GameManager] All levels completed.");
            ShowWinPanel();
            ResumeGame();
            return;
        }
        _sawManager.ResetAllSaws();
        CurrentLevelIndex = nextIndex;
        BeginCurrentLevel();
    }

    public void RestartGame()
    {
        CurrentLevelIndex = _startLevelIndex;
        _sawManager.ResetAllSaws();
        _xpManager.RestartXP();
        _winPanel.SetActive(false);
        BeginCurrentLevel();
    }

    private void ShowWinPanel()
    {
        _winPanel.SetActive(true);
    }

    public void RestartCurrentLevel()
    {
        if (_logStateChanges)
            Debug.Log($"[GameManager] Restart Level => {CurrentLevelData?.levelId}");

        BeginCurrentLevel();
    }

    public void LoadLevelByIndex(int levelIndex)
    {
        if (_allLevels == null || _allLevels.Count == 0)
        {
            Debug.LogError("No levels available.");
            return;
        }

        if (levelIndex < 0 || levelIndex >= _allLevels.Count)
        {
            Debug.LogWarning($"Invalid levelIndex {levelIndex}");
            return;
        }

        CurrentLevelIndex = levelIndex;
        BeginCurrentLevel();
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        IsPausedByUpgrade = true;

        if (_logStateChanges)
            Debug.Log("[GameManager] Pause Game");
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        IsPausedByUpgrade = false;

        if (_logStateChanges)
            Debug.Log("[GameManager] Resume Game");
    }
}