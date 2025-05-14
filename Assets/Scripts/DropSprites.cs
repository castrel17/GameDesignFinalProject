using UnityEngine;
using System.Collections.Generic;
using NUnit.Framework.Constraints;

public class DropSprites : MonoBehaviour
{
    public float dropSpeed = 5f;
    public float dropDistance = 10f;

    public Sprite finished_star;

    public GameObject lvl0_star1;
    public GameObject lvl0_star2;
    public GameObject lvl0_star3;

    private SpriteRenderer lv0star1Renderer;
    private SpriteRenderer lv0star2Renderer;
    private SpriteRenderer lv0star3Renderer;

    public GameObject lvl1_star1;
    public GameObject lvl1_star2;
    public GameObject lvl1_star3;

    private SpriteRenderer lv1star1Renderer;
    private SpriteRenderer lv1star2Renderer;
    private SpriteRenderer lv1star3Renderer;

    public GameObject lvl2_star1;
    public GameObject lvl2_star2;
    public GameObject lvl2_star3;

    private SpriteRenderer lv2star1Renderer;
    private SpriteRenderer lv2star2Renderer;
    private SpriteRenderer lv2star3Renderer;

    public GameObject lvl3_star1;
    public GameObject lvl3_star2;
    public GameObject lvl3_star3;

    private SpriteRenderer lv3star1Renderer;
    private SpriteRenderer lv3star2Renderer;
    private SpriteRenderer lv3star3Renderer;

    private List<Transform> droppableSprites = new List<Transform>();
    private Dictionary<Transform, float> initialYPositions = new Dictionary<Transform, float>();
    private bool shouldDrop = false;

    private int level_score;
    private string which_level;

    void Start()
    {
        GameObject[] sprites = GameObject.FindGameObjectsWithTag("Droppable");
        foreach (GameObject sprite in sprites)
        {
            Transform t = sprite.transform;
            droppableSprites.Add(t);
            initialYPositions[t] = t.position.y;
        }
        level_score = PlayerPrefs.GetInt("score");
        which_level = PlayerPrefs.GetString("scene");

        Debug.Log($"Score {level_score}, Level Name: {which_level}");

        lv0star1Renderer = lvl0_star1.GetComponent<SpriteRenderer>();
        lv0star2Renderer = lvl0_star2.GetComponent<SpriteRenderer>();
        lv0star3Renderer = lvl0_star3.GetComponent<SpriteRenderer>();

        lv1star1Renderer = lvl1_star1.GetComponent<SpriteRenderer>();
        lv1star2Renderer = lvl1_star2.GetComponent<SpriteRenderer>();
        lv1star3Renderer = lvl1_star3.GetComponent<SpriteRenderer>();

        lv2star1Renderer = lvl2_star1.GetComponent<SpriteRenderer>();
        lv2star2Renderer = lvl2_star2.GetComponent<SpriteRenderer>();
        lv2star3Renderer = lvl2_star3.GetComponent<SpriteRenderer>();

        lv3star1Renderer = lvl3_star1.GetComponent<SpriteRenderer>();
        lv3star2Renderer = lvl3_star2.GetComponent<SpriteRenderer>();
        lv3star3Renderer = lvl3_star3.GetComponent<SpriteRenderer>();

        check_for_stars(which_level, level_score);
    }

    void Update()
    {
        if (shouldDrop)
        {
            bool allDone = true;

            foreach (Transform t in droppableSprites)
            {
                float distanceDropped = initialYPositions[t] - t.position.y;

                if (distanceDropped < dropDistance)
                {
                    t.Translate(Vector3.down * dropSpeed * Time.deltaTime);
                    allDone = false;
                }
            }

            // Stop dropping when all have completed their drop
            if (allDone)
            {
                shouldDrop = false;
            }
        }
    }

    public void check_for_stars(string level_name, int Score)
    {
        Score = ScoreManager.previousScores["Demo"];
        if (Score >= 100)
        {
            lv0star3Renderer.sprite = finished_star;
        }
        if (Score >= 66)
        {
            lv0star2Renderer.sprite = finished_star;
        }
        if (Score >= 33)
        {
            lv0star1Renderer.sprite = finished_star;
        }

        Score = ScoreManager.previousScores["Level1"];
        if (Score >= 100)
        {
            lv1star3Renderer.sprite = finished_star;
        }
        if (Score >= 66)
        {
            lv1star2Renderer.sprite = finished_star;
        }
        if (Score >= 33)
        {
            lv1star1Renderer.sprite = finished_star;
        }

        Score = ScoreManager.previousScores["Level2"];
        if (Score >= 100)
        {
            lv2star3Renderer.sprite = finished_star;
        }
        if (Score >= 66)
        {
            lv2star2Renderer.sprite = finished_star;
        }
        if (Score >= 33)
        {
            lv2star1Renderer.sprite = finished_star;
        }

        Score = ScoreManager.previousScores["Level3"];
        if (Score >= 100)
        {
            lv3star3Renderer.sprite = finished_star;
        }
        if (Score >= 66)
        {
            lv3star2Renderer.sprite = finished_star;
        }
        if (Score >= 33)
        {
            lv3star1Renderer.sprite = finished_star;
        }
        
    }

    public void TriggerDrop()
    {
        Debug.Log("trigger drop");
        shouldDrop = true;
    }
}