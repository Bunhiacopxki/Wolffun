using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeCardView : MonoBehaviour
{
    [SerializeField] private TMP_Text _titleText;
    [SerializeField] private TMP_Text _descriptionText;
    [SerializeField] private Image _iconImage;

    public void Bind(UpgradeData data)
    {
        if (data == null) return;

        if (_titleText != null)
            _titleText.text = data.displayName;

        if (_descriptionText != null)
            _descriptionText.text = data.description;

        if (_iconImage != null)
        {
            _iconImage.sprite = data.icon;
            _iconImage.enabled = data.icon != null;
        }
    }
}