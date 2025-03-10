using UnityEngine;
using TMPro;

public class textChange : MonoBehaviour
{
    public TextMeshProUGUI text;
    public float beatTempo; 
    public bool hasStarted;
    
    private float beatInterval; 
    private float timeSinceLastBeat;
    private float displayTime = 0.2f; 

    void Start()
    {
        if (beatTempo <= 0) return;
        beatInterval = 240f / beatTempo;
        timeSinceLastBeat = 0f;
        text.text = "";
    }

    void Update()
    {
        if (!hasStarted) return;

        timeSinceLastBeat += Time.deltaTime;

        if (timeSinceLastBeat >= beatInterval)
        {
            timeSinceLastBeat -= beatInterval; 
            text.text = "Hit Now";
            Invoke("ClearText", displayTime); 
        }
    }

    void ClearText()
    {
        text.text = "";
    }
}
