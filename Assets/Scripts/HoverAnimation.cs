using UnityEngine;

public class HoverAnimation : MonoBehaviour
{
    public Sprite frame1; // First frame (default)
    public Sprite frame2; // Second frame (on hover)

    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = frame1;
    }

    void OnMouseEnter()
    {
        sr.sprite = frame2;
    }

    void OnMouseExit()
    {
        sr.sprite = frame1;
    }
}