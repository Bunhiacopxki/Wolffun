using UnityEngine;

public class UpgradeApplier : MonoBehaviour
{
    [SerializeField] private SawManager _sawManager;
    [SerializeField] private SawPlacementController _sawPlacementController;

    public void ApplyUpgrade(UpgradeData data)
    {
        if (data == null) return;

        switch (data.type)
        {
            case UpgradeType.IncreaseSawSize:
                _sawManager.ApplyToAllSaws(saw => saw.AddLength(data.value));
                break;

            case UpgradeType.IncreaseSawRotationSpeed:
                _sawManager.ApplyToAllSaws(saw => saw.AddRotationSpeed(data.value));
                break;

            case UpgradeType.IncreaseSawDps:
                _sawManager.ApplyToAllSaws(saw => saw.AddDps(data.value));
                break;

            case UpgradeType.AddNewSaw:
                _sawPlacementController.BeginChooseEmptySlotMode();
                break;
        }
    }
}