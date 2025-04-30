using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DemoLevelManager : MonoBehaviour
{
    public GameObject potatoPrefab;
    public GameObject carrotPrefab;
    public GameObject onionPrefab;

    public float slideSpeed = 5f;
    private Vector3 targetPosition = new Vector3(0f, -3.25f, 0f);
    private Vector3 offScreenPosition = new Vector3(-10f, -3.25f, 0f);

    public GameObject currentVegetable;
    private bool isSliding = false;

    public DemoSongManager songManager;
    public bool needVeg = true;

    public TextMeshProUGUI tutorialText;

    private int spawnIndex = 0;

    public GameObject TooEarly;
    public GameObject TooLate;
    public GameObject Perfect;
    public GameObject Miss;
    public GameObject bonusStreak;
    public GameObject feedback;
    public GameObject bonus;

    public ScoreBar scoreBar;
    public GoalNote goalNote;
    private int streak;

    public TextMeshProUGUI scoreText;
    private int score = 0;
    Vector3 centerPos = new Vector3(0f, 2f, 0f);
    Vector3 centerPosDown = new Vector3(0f, 2f, 0f);

    public Slider streakSlider;
    public int fullCycles = 0;
    public int maxCycles;
    private int cyclesThisLoop = 0;

    public int level;

    void Start()
    {
        //maxCycles = level == 0 ? 21 : 50;
        //maxCycles = level == 0 ? 21 : 50;
        maxCycles = level == 0 ? 7 : 12;
        //maxCycles = level == 0 ? 3 : 3;
    }

    void Update()
    {
        if (!songManager.gameOver && songManager.loopStarted)
        {
            if (songManager.startStatus() && needVeg)
            {
                needVeg = false;
                spawnNew();
                songManager.ResetNoteCounter();
            }

            if (isSliding && currentVegetable != null)
            {
                currentVegetable.transform.position = Vector3.MoveTowards(
                    currentVegetable.transform.position,
                    targetPosition,
                    slideSpeed * Time.deltaTime
                );

                if (CheckVegetableProgress())
                {
                    isSliding = false;
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.Tab))
        {
            SceneManager.LoadScene(0);
        }
    }

    public void spawnFeedback(int opt)
    {
        switch (opt)
        {
            case 0:
                feedback = Instantiate(Perfect, centerPos, Quaternion.identity);
                streak++;
                score += 100;
                scoreBar.updateScoreBar(2);
                break;
            case 1:
                feedback = Instantiate(Miss, centerPos, Quaternion.identity);
                streak = 0;
                goalNote.shake();
                break;
            case 2:
                feedback = Instantiate(TooEarly, centerPos, Quaternion.identity);
                streak = 0;
                score += 50;
                scoreBar.updateScoreBar(1);
                break;
            case 3:
                feedback = Instantiate(TooLate, centerPos, Quaternion.identity);
                streak = 0;
                score += 50;
                scoreBar.updateScoreBar(1);
                break;
        }

        if (streak == 3)
        {
            score += 100;
            bonus = Instantiate(bonusStreak, centerPosDown, Quaternion.identity);
            bonus.SetActive(true);
            Destroy(bonus, 1.0f);
            streak = 0;
        }

        streakSlider.value = streak;
        feedback.SetActive(true);
        Destroy(feedback, 1.0f);
        scoreText.text = "Score: " + score;
    }

    public void spawnNew()
    {
        if (fullCycles >= maxCycles)
        {
            tutorialText?.SetText("All vegetables done! Excellent work!");
            if (level == 0)
                SceneManager.LoadScene("EndDemo");
            else if (level == 1)
                SceneManager.LoadScene("EndScene");
            return;
        }

        switch (fullCycles % 3)
        {
            case 0:
                SpawnCarrot();
                songManager.isCarrot = true;
                songManager.isPotato = false;
                songManager.isOnion = false;
                break;
            case 1:
                SpawnPotato();
                songManager.isCarrot = false;
                songManager.isPotato = true;
                songManager.isOnion = false;
                break;
            case 2:
                SpawnOnion();
                songManager.isCarrot = false;
                songManager.isPotato = false;
                songManager.isOnion = true;
                break;
        }

        fullCycles++;
        cyclesThisLoop++;

        // if (cyclesThisLoop == 6 && level == 0)
        // {
        //     songManager.loopStarted = false;
        //     cyclesThisLoop = 0;
        // }
    }

    public int whatLevel() => level;

    void SpawnPotato()
    {
        currentVegetable = Instantiate(potatoPrefab, offScreenPosition, Quaternion.identity);
        isSliding = true;
    }

    void SpawnCarrot()
    {
        currentVegetable = Instantiate(carrotPrefab, offScreenPosition, Quaternion.identity);
        isSliding = true;
    }

    void SpawnOnion()
    {
        currentVegetable = Instantiate(onionPrefab, offScreenPosition, Quaternion.identity);
        isSliding = true;
    }

    bool CheckVegetableProgress()
    {
        if (currentVegetable == null) return false;

        var peeler = currentVegetable.GetComponent<VegetablePeeler>();
        var cutter = currentVegetable.GetComponent<VegetableCutting>();

        if (peeler != null && !peeler.IsFullyPeeled()) return false;
        if (cutter != null && !cutter.allCut) return false;

        spawnIndex++;
        needVeg = true;

        if (cyclesThisLoop == 6 && level == 0)
        {
            songManager.loopStarted = false;
            cyclesThisLoop = 0;
        }

        Debug.Log("Vegetable fully processed. Moving to next.");
        return true; // ✅ Done!
    }
}
