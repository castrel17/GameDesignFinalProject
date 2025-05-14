using UnityEngine;

public class ScoreBar : MonoBehaviour
{
    public Sprite[] progress;
    private SpriteRenderer rend;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rend = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetProgress(float ratio) 
    {
        ratio = Mathf.Clamp01(ratio);
        int idx = Mathf.FloorToInt(ratio * (progress.Length - 1));
        rend.sprite = progress[idx];
    }
}
