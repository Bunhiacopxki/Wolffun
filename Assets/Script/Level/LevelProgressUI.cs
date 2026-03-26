using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelProgressUI : MonoBehaviour
{
    [SerializeField] private Slider _spawnSlider;
    [SerializeField] private TMP_Text _levelText;
    private string _levelFormat = "Level {0}";

    public void SetLevelText(int levelNumber)
    {
        if (_levelText != null)
            _levelText.text = string.Format(_levelFormat, levelNumber);
    }

    public void SetupSpawnProgress(int totalObjects)
    {
        if (_spawnSlider == null) return;

        int safeTotal = Mathf.Max(0, totalObjects);
        _spawnSlider.minValue = 0;
        _spawnSlider.maxValue = safeTotal;
        _spawnSlider.value = 0;
    }

    public void UpdateSpawnProgress(int spawnedCount)
    {
        if (_spawnSlider == null) return;
        _spawnSlider.value = Mathf.Clamp(spawnedCount, 0, (int)_spawnSlider.maxValue);
    }

    public void FillSpawnProgress()
    {
        if (_spawnSlider == null) return;
        _spawnSlider.value = _spawnSlider.maxValue;
    }
}