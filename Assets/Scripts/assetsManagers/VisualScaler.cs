using UnityEngine;

public class VisualScaler : MonoBehaviour
{
    public Transform visualTransform;

    public float scaleFactor = 1f;

    private Vector3 _baseScale;

    void Awake()
    {
        _baseScale = visualTransform.localScale;
        ApplyScale();
    }

    public void ApplyScale()
    {
        visualTransform.localScale = _baseScale * scaleFactor;
    }
}
