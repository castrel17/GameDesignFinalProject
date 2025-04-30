using UnityEngine;

public class VisualScaler : MonoBehaviour
{
    [Header("Visual child here")]
    public Transform visualTransform;

    [Range(0.1f,2f)]
    public float scaleFactor = 1f;

    void Awake()
    {
        ApplyScale();
    }

    public void ApplyScale()
    {
        if (visualTransform != null)
            visualTransform.localScale = Vector3.one * scaleFactor;
    }
}
