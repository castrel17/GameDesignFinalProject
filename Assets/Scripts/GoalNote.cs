using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GoalNote : MonoBehaviour
{
    private Vector3 originalPosition;
    public float shakeMagnitude = 0.05f; 
    public float shakeDuration = 0.5f; 

    void Start()
    {
        originalPosition = transform.position;
    }
    public void shake()
    {
        StartCoroutine(shakeCoroutine());
    }

    private IEnumerator shakeCoroutine()
    {
        float elapsed = 0f;
        while (elapsed < shakeDuration)
        {
            float xOffset = Random.Range(-shakeMagnitude, shakeMagnitude);
            float yOffset = Random.Range(-shakeMagnitude, shakeMagnitude);
            float zOffset = Random.Range(-shakeMagnitude, shakeMagnitude);
            transform.position = originalPosition + new Vector3(xOffset, yOffset, zOffset);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = originalPosition;
    }
}
