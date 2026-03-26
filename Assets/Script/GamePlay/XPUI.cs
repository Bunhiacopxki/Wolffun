using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class XPUI : MonoBehaviour
{
    [Header("XP UI")]
    [SerializeField] private Slider _xpSlider;

    [Header("Animation")]
    [SerializeField] private float _xpLerpSpeed = 8f;
    [SerializeField] private bool _animateXPBar = true;

    [Header("Level Up Effect")]
    //[SerializeField] private GameObject _levelUpEffect;
    [SerializeField] private float _levelPunchScale = 1.2f;
    [SerializeField] private float _levelPunchDuration = 0.2f;

    private Coroutine _xpBarRoutine;
    private Coroutine _levelAnimRoutine;

    private void OnEnable()
    {
        GameManager.Instance.XpManager.OnXPChanged += HandleXPChanged;
        GameManager.Instance.XpManager.OnLevelUp += HandleLevelUp;
    }

    private void OnDisable()
    {
        GameManager.Instance.XpManager.OnXPChanged -= HandleXPChanged;
        GameManager.Instance.XpManager.OnLevelUp -= HandleLevelUp;
    }

    private void Start()
    {
        RefreshAllInstant();
    }

    private void RefreshAllInstant()
    {
        UpdateXPUIInstant(GameManager.Instance.XpManager.CurrentXP, GameManager.Instance.XpManager.RequiredXP);
    }

    private void HandleXPChanged(int currentXP, int requiredXP)
    {
        float targetValue = requiredXP > 0 ? (float)currentXP / requiredXP : 0f;

        if (_xpSlider != null)
        {
            if (_animateXPBar)
            {
                if (_xpBarRoutine != null)
                    StopCoroutine(_xpBarRoutine);

                _xpBarRoutine = StartCoroutine(AnimateXPBar(targetValue));
            }
            else
            {
                _xpSlider.value = targetValue;
            }
        }
    }

    private void HandleLevelUp(int newLevel)
    {
        if (_levelAnimRoutine != null)
            StopCoroutine(_levelAnimRoutine);
    }

    private void UpdateXPUIInstant(int currentXP, int requiredXP)
    {
        if (_xpSlider != null)
            _xpSlider.value = requiredXP > 0 ? (float)currentXP / requiredXP : 0f;
    }

    private IEnumerator AnimateXPBar(float targetValue)
    {
        while (Mathf.Abs(_xpSlider.value - targetValue) > 0.001f)
        {
            _xpSlider.value = Mathf.Lerp(_xpSlider.value, targetValue, Time.deltaTime * _xpLerpSpeed);
            yield return null;
        }

        _xpSlider.value = targetValue;
    }
}