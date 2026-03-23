using System;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    [SerializeField] private List<UpgradeData> _allUpgrades = new();
    [SerializeField] private UpgradePopupView _popupView;
    [SerializeField] private UpgradeApplier _upgradeApplier;

    public UpgradeData LastSelectedUpgrade { get; private set; }

    public void ShowRandomUpgradeSelection(Action onClosed)
    {
        if (_allUpgrades == null || _allUpgrades.Count == 0)
        {
            LastSelectedUpgrade = null;
            onClosed?.Invoke();
            return;
        }

        UpgradeData first = _allUpgrades[UnityEngine.Random.Range(0, _allUpgrades.Count)];
        UpgradeData second = _allUpgrades[UnityEngine.Random.Range(0, _allUpgrades.Count)];

        int safe = 0;
        while (second == first && _allUpgrades.Count > 1 && safe < 20)
        {
            second = _allUpgrades[UnityEngine.Random.Range(0, _allUpgrades.Count)];
            safe++;
        }

        _popupView.Show(first, second, selected =>
        {
            LastSelectedUpgrade = selected;
            _upgradeApplier.ApplyUpgrade(selected);
            onClosed?.Invoke();
        });
    }
}