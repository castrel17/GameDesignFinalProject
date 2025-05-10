using UnityEngine;
using System.Collections;

public class GoalNote2 : MonoBehaviour
{
    private Vector3 originalPosition;
    public float shakeMagnitude = 0.05f;
    public float shakeDuration = 0.5f;
    public float pulseMagnitude = 0.5f;  
    
    public Level1SongManager songManager;

    private Vector3 originalScale;
    private bool isPulsing = false;
    private float lastPulseTime = -1f; 

    void Start()
    {
        originalPosition = transform.position;
        originalScale = transform.localScale;
    }

    void Update()
    {
        if (songManager.startStatus())
        {
            if (this.transform.CompareTag("OuterRing"))
            {
                float beatsPosition = songManager.getBeatsPosition();

                int currentBeat = Mathf.FloorToInt(beatsPosition);
                if (currentBeat != lastPulseTime)
                {
                    lastPulseTime = currentBeat;
                    StartCoroutine(PulseCoroutine());
                }
            }
            
        }
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

    private IEnumerator PulseCoroutine()
    {
        float pulseDuration = 60f / songManager.bpm;
        float elapsed = 0f;

        while (elapsed < pulseDuration)
        {
            float pulseScale = Mathf.Abs(Mathf.Sin(elapsed / pulseDuration * Mathf.PI)) * pulseMagnitude + 1.0f;
            transform.localScale = originalScale * pulseScale;

            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localScale = originalScale;
    }
}
