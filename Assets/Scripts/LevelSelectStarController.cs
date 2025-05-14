using UnityEngine;

public class LevelSelectStarController : MonoBehaviour
{
    [Header("Finished Star Sprite")]
    public Sprite filledStar;

    [Header("Demo (Tutorial) Stars")]
    public SpriteRenderer demoStar1;
    public SpriteRenderer demoStar2;
    public SpriteRenderer demoStar3;

    [Header("Level 1 Stars")]
    public SpriteRenderer level1Star1;
    public SpriteRenderer level1Star2;
    public SpriteRenderer level1Star3;

    [Header("Level 2 Stars")]
    public SpriteRenderer level2Star1;
    public SpriteRenderer level2Star2;
    public SpriteRenderer level2Star3;

    [Header("Level 3 Stars")]
    public SpriteRenderer level3Star1;
    public SpriteRenderer level3Star2;
    public SpriteRenderer level3Star3;

    void Start()
    {
        ApplyStars("Demo",  demoStar1,  demoStar2,  demoStar3);
        ApplyStars("Level1", level1Star1, level1Star2, level1Star3);
        ApplyStars("Level2", level2Star1, level2Star2, level2Star3);
        ApplyStars("Level3", level3Star1, level3Star2, level3Star3);
    }

    void ApplyStars(string key, SpriteRenderer s1, SpriteRenderer s2, SpriteRenderer s3)
    {
        int score = 0;
        ScoreManager.previousScores.TryGetValue(key, out score);
        int count = score >= 90 ? 3
                  : score >= 66 ? 2
                  : score >= 33 ? 1
                  : 0;

        if (count >= 1) s1.sprite = filledStar;
        if (count >= 2) s2.sprite = filledStar;
        if (count >= 3) s3.sprite = filledStar;
    }
}