using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class XPUI : MonoBehaviour
{
    [Header("XP UI")]
    [SerializeField] private Slider _xpSlider;

    [Header("Level UI")]
    [SerializeField] private TMP_Text _levelText;

    [Header("Animation")]
    [SerializeField] private float _xpLerpSpeed = 8f;
    [SerializeField] private bool _animateXPBar = true;

    [Header("Level Up Effect")]
    //[SerializeField] private GameObject _levelUpEffect;
    [SerializeField] private float _levelPunchScale = 1.2f;
    [SerializeField] private float _levelPunchDuration = 0.2f;

    private Coroutine _xpBarRoutine;
    private Coroutine _levelAnimRoutine;
    private Vector3 _levelOriginalScale;

    private void Awake()
    {
        if (_levelText != null)
            _levelOriginalScale = _levelText.transform.localScale;
    }

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
        UpdateLevelText(GameManager.Instance.XpManager.CurrentLevel);
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
        UpdateLevelText(newLevel);

        //if (_levelUpEffect != null)
        //    _levelUpEffect.SetActive(true);

        if (_levelAnimRoutine != null)
            StopCoroutine(_levelAnimRoutine);

        _levelAnimRoutine = StartCoroutine(PlayLevelPunch());
    }

    private void UpdateLevelText(int level)
    {
        if (_levelText != null)
            _levelText.text = level.ToString();
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

    private IEnumerator PlayLevelPunch()
    {
        if (_levelText == null) yield break;

        Transform target = _levelText.transform;
        Vector3 startScale = _levelOriginalScale;
        Vector3 punchScale = _levelOriginalScale * _levelPunchScale;

        float half = _levelPunchDuration * 0.5f;
        float t = 0f;

        while (t < half)
        {
            t += Time.deltaTime;
            float lerp = t / half;
            target.localScale = Vector3.Lerp(startScale, punchScale, lerp);
            yield return null;
        }

        t = 0f;
        while (t < half)
        {
            t += Time.deltaTime;
            float lerp = t / half;
            target.localScale = Vector3.Lerp(punchScale, startScale, lerp);
            yield return null;
        }

        target.localScale = startScale;
    }
}