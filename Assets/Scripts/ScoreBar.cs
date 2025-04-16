using UnityEngine;

public class ScoreBar : MonoBehaviour
{
    public Sprite[] progress;
    private int index = 0;
    private SpriteRenderer render;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        render = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void updateScoreBar(int value)
    {
        index += value;
        if (index >= progress.Length)
        {
            index = progress.Length - 1;
        }
        render.sprite = progress[index];
    }
}
