using UnityEngine;

public class ObstacleView : MonoBehaviour
{
    public void Initialize(ObstacleData data)
    {
        if (data == null) return;

        transform.position = data.position;
    }
}