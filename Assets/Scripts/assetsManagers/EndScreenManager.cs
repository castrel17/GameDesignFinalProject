using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class EndScreenManager : MonoBehaviour
{
    public Sprite star0;
    public Sprite star1;
    public Sprite star2;
    public Sprite star3;

    public Sprite empty_star;
    public Sprite filled_star;

    public GameObject lvl_star1;
    public GameObject lvl_star2;
    public GameObject lvl_star3;
    public TextMeshProUGUI percentText;

    private SpriteRenderer _frameRenderer;
    private SpriteRenderer _star1Renderer;
    private SpriteRenderer _star2Renderer;
    private SpriteRenderer _star3Renderer;

    void Start()
    {
        int finalPercent = PlayerPrefs.GetInt("beatPercent", 0);

        string levelName = SceneManager.GetActiveScene().name;

        int previousBest;
        if (!ScoreManager.previousScores.TryGetValue(levelName, out previousBest))
            previousBest = 0;

        if (finalPercent > previousBest)
            ScoreManager.previousScores[levelName] = finalPercent;

        Debug.Log($"[EndScreen] Level '{levelName}', percent = {finalPercent}, previous best = {previousBest}");

        _frameRenderer    = GetComponent<SpriteRenderer>();
        _star1Renderer    = lvl_star1.GetComponent<SpriteRenderer>();
        _star2Renderer    = lvl_star2.GetComponent<SpriteRenderer>();
        _star3Renderer    = lvl_star3.GetComponent<SpriteRenderer>();

        //    90%+ -> 3, 66%+ -> 2, 33%+ -> 1, else 0
        if      (finalPercent >= 90)
        {
            _frameRenderer.sprite = star3;
            _star1Renderer.sprite = filled_star;
            _star2Renderer.sprite = filled_star;
            _star3Renderer.sprite = filled_star;
        }
        else if (finalPercent >= 66)
        {
            _frameRenderer.sprite = star2;
            _star1Renderer.sprite = filled_star;
            _star2Renderer.sprite = filled_star;
        }
        else if (finalPercent >= 33)
        {
            _frameRenderer.sprite = star1;
            _star1Renderer.sprite = filled_star;
        }
        else
        {
            _frameRenderer.sprite = star0;
        }

        if (percentText != null)
            percentText.text = $"{finalPercent}%";
    }
}
