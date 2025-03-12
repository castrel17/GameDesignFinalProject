using UnityEngine;

public class TomatoChopper : MonoBehaviour
{
    [Header("Tomato Sprites")]
    public Sprite[] tomatoSprites;

    private SpriteRenderer spriteRenderer;
    private int chopIndex = 0;

    public textChange textChangeRef;

    public GameManager gameManagerRef;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = tomatoSprites[chopIndex];
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && textChangeRef.inHitWindow)
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

            if (chopIndex == tomatoSprites.Length - 1)
            {
                Debug.Log("Tomato is fully chopped!");
                gameManagerRef.StopEverything();
            }
        }
    }

}
