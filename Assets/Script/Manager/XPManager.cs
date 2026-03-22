using System;
using UnityEngine;

public class XPManager : MonoBehaviour
{
    public event Action<int> OnXPChanged;

    public int CurrentXP { get; private set; }

    public void AddXP(int amount)
    {
        if (amount <= 0) return;

        CurrentXP += amount;
        OnXPChanged?.Invoke(CurrentXP);

        Debug.Log($"XP +{amount} => Total XP: {CurrentXP}");
    }
}