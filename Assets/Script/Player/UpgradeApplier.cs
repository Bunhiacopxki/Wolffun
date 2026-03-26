using UnityEngine;

public class UpgradeApplier : MonoBehaviour
{
    [SerializeField] private SawPlacementController _sawPlacementController;

    public void ApplyUpgrade(UpgradeData data)
    {
        if (data == null) return;

        switch (data.type)
        {
            case UpgradeType.IncreaseSawSize:
                GameManager.Instance.SawManager.ApplyToAllSaws(saw => saw.AddLength(data.value));
                break;

            case UpgradeType.IncreaseSawRotationSpeed:
                GameManager.Instance.SawManager.ApplyToAllSaws(saw => saw.AddRotationSpeed(data.value));
                break;

            case UpgradeType.IncreaseSawDps:
                GameManager.Instance.SawManager.ApplyToAllSaws(saw => saw.AddDps(data.value));
                break;

            case UpgradeType.AddNewSaw:
                _sawPlacementController.BeginChooseEmptySlotMode();
                break;
        }
    }
}