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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        star1Renderer = lvl_star1.GetComponent<SpriteRenderer>();
        star2Renderer = lvl_star2.GetComponent<SpriteRenderer>();
        star3Renderer = lvl_star3.GetComponent<SpriteRenderer>(); 

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            spriteRenderer.sprite = star1;
            Debug.Log("P key was pressed");
            star1Renderer.sprite = filled_star;
           
        }
        else if (Input.GetKeyDown(KeyCode.P))
        {
            spriteRenderer.sprite = star2;
            Debug.Log("O key was pressed");
            star1Renderer.sprite = filled_star;
            star2Renderer.sprite = filled_star;
        }
        else if (Input.GetKeyDown(KeyCode.I))
        {
            spriteRenderer.sprite = star0;
            Debug.Log("I key was pressed");
        }

    }
}
