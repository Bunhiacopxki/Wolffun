using UnityEngine;

public class ObstacleView : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _renderSprite;
    public void Initialize(ObstacleData data)
    {
        if (data == null) return;

        transform.position = data.position;
        _renderSprite.sprite = data.sprite;
    }
}