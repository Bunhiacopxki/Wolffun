using System;
using UnityEngine;

public class UpgradePopupView : MonoBehaviour
{
    [SerializeField] private GameObject _root;
    [SerializeField] private UpgradeCardView _leftCard;
    [SerializeField] private UpgradeCardView _rightCard;

    private Action<UpgradeData> _onSelected;
    private UpgradeData _first;
    private UpgradeData _second;

    private void Awake()
    {
        if (_root != null)
            _root.SetActive(false);
    }

    public void Show(UpgradeData first, UpgradeData second, Action<UpgradeData> onSelected)
    {
        _first = first;
        _second = second;
        _onSelected = onSelected;

        if (_leftCard != null)
            _leftCard.Bind(_first);

        if (_rightCard != null)
            _rightCard.Bind(_second);

        if (_root != null)
            _root.SetActive(true);
    }

    public void ChooseFirst()
    {
        if (_root != null)
            _root.SetActive(false);

        _onSelected?.Invoke(_first);
    }

    public void ChooseSecond()
    {
        if (_root != null)
            _root.SetActive(false);

        _onSelected?.Invoke(_second);
    }
}