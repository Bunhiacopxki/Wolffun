using UnityEngine;
using UnityEngine.UI;

public class LaserGunToggleButton : MonoBehaviour
{
    [SerializeField] private GameObject laserGunObject;
    [SerializeField] private Button toggleButton;

    private void Awake()
    {
        if (toggleButton != null)
            toggleButton.onClick.AddListener(ToggleLaserGun);
    }

    private void OnDestroy()
    {
        if (toggleButton != null)
            toggleButton.onClick.RemoveListener(ToggleLaserGun);
    }

    public void ToggleLaserGun()
    {
        if (laserGunObject == null)
            return;

        bool willShow = !laserGunObject.activeSelf;

        if (!willShow)
        {
            LaserGunController controller = laserGunObject.GetComponent<LaserGunController>();
            if (controller != null)
                controller.StopLaserVisual();
        }

        laserGunObject.SetActive(willShow);
    }
}