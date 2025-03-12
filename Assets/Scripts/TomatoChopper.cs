using UnityEngine;

public class TomatoChopper : MonoBehaviour
{
    [Header("Tomato Sprites")]
    public Sprite[] tomatoSprites;

    private SpriteRenderer spriteRenderer;
    private int chopIndex = 0;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = tomatoSprites[chopIndex];
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ChopTomato();
        }
    }

    void ChopTomato()
    {
        if (chopIndex < tomatoSprites.Length - 1)
        {
            chopIndex++;
            spriteRenderer.sprite = tomatoSprites[chopIndex];
        }
        else
        {
            Debug.Log("Tomato is fully chopped!");
        }
    }
}