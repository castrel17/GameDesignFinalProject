using UnityEngine;

public class EndScreenManager : MonoBehaviour
{
    public Sprite star0;
    public Sprite star1;
    public Sprite star2;
    public Sprite star3;

    public Sprite filled_star;
    public Sprite empty_star;

    public GameObject endscreen;
    public GameObject lvl_star1;
    public GameObject lvl_star2;
    public GameObject lvl_star3;

    private SpriteRenderer spriteRenderer;
    private SpriteRenderer star1Renderer;
    private SpriteRenderer star2Renderer;
    private SpriteRenderer star3Renderer;

    private int finalScore;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        finalScore = PlayerPrefs.GetInt("score");
        Debug.Log($"Score: {finalScore}");
        star1Renderer = lvl_star1.GetComponent<SpriteRenderer>();
        star2Renderer = lvl_star2.GetComponent<SpriteRenderer>();
        star3Renderer = lvl_star3.GetComponent<SpriteRenderer>();

        if (finalScore >= 99)
        {
            spriteRenderer.sprite = star3;
            Debug.Log("3 Stars");
            star1Renderer.sprite = filled_star;
            star2Renderer.sprite = filled_star;
            star3Renderer.sprite = filled_star;

        }
        else if (finalScore >= 66)
        {
            spriteRenderer.sprite = star2;
            Debug.Log("2 Stars");
            star1Renderer.sprite = filled_star;
            star2Renderer.sprite = filled_star;
        }
        else if (finalScore >= 33)
        {
            spriteRenderer.sprite = star1;
            Debug.Log("1 Stars");
            star1Renderer.sprite = filled_star;
        }
        else
        {
            Debug.Log("0 Stars");
        }
    }

    // Update is called once per frame
    void Update()
    {
       

    }
}