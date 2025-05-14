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
    private bool firstPotato = true;
    private bool tutorialPause = false;

    private bool allCut = false;

    private int numCuts = 0;

    private int successfulCuts = 0;
    private int scoreBarIndex = 0;

    public static float endLevelHitRatio;

    private string currScene;

    private float possibleHits = 0f;
    private float accumulatedHits = 0f;
    public float totalExpectedNotes;

    void Start()
    {
        
        maxCycles = level == 0 ? 7 : 9;
        switch(level)
        {
        case 0: totalExpectedNotes = 46f; break;
        case 1: totalExpectedNotes = 69f; break;
        case 2: totalExpectedNotes = 150f; break;
        case 3: totalExpectedNotes = 105f; break;
        default:
            totalExpectedNotes = songManager.getNumBeats();
            break;
        }

        currScene = SceneManager.GetActiveScene().name;
    }

    void Update()
    {
        if (!songManager.gameOver && songManager.loopStarted)
        {
            if(tutorialPause && Input.GetMouseButtonDown(0))
            {
                Time.timeScale = 1f;
                AudioListener.pause = false;
                songManager.UnpauseMusic();
                songManager.instructionText.gameObject.SetActive(false);
                tutorialPause = false;
            }
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
            if (level == 0 && songManager.isPotato && firstPotato && currentVegetable.transform.position == targetPosition)
            {
                firstPotato = false;
                Time.timeScale = 0f;
                AudioListener.pause = true;
                songManager.PauseMusic();
                tutorialPause = true;
                songManager.instructionText.SetText("To peel the potato you have to press and hold until you get to the end of the note (left click to continue)");
                songManager.instructionText.gameObject.SetActive(true);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Tab))
        {
            SceneManager.LoadScene(0);
        }

        // if (Input.GetKeyDown(KeyCode.P))
        // {
        //     int maxScore = songManager.getNumBeats() * 100;
        //     int percentScore = (100*score / maxScore);
        //     percentScore = 100;
        //     PlayerPrefs.SetInt("score", percentScore);
        //     PlayerPrefs.SetString("scene", currScene);
        //     Debug.Log($"Max {maxScore}, Score {score}, Percent {percentScore}");
        //     SceneManager.LoadScene("EndScene");
        // }

        // if (Input.GetKeyDown(KeyCode.O))
        // {
        //     int maxScore = songManager.getNumBeats() * 100;
        //     int percentScore = (100 * score / maxScore);
        //     percentScore = 50;
        //     PlayerPrefs.SetInt("score", percentScore);
        //     PlayerPrefs.SetString("scene", currScene);
        //     Debug.Log($"Max {maxScore}, Score {score}, Percent {percentScore}");
        //     SceneManager.LoadScene("EndScene");
        // }
    }

    private void UpdateHitProgress(int opt)
    {
        float add = 0f;
        switch (opt)
        {
            case 0: add = 1f; break;
            case 2:
            case 3: add = 0.5f; break;
        }
        accumulatedHits += add;

        float ratio = Mathf.Clamp01(accumulatedHits / totalExpectedNotes);
        scoreBar.SetProgress(ratio);
    }

    public void spawnFeedback(int opt)
    {
        int numBeats = songManager.getNumBeats();
        int spriteCount = scoreBar.progress.Length;

        possibleHits += 1f;
        bool isScoringAction = false;

        switch (opt)
        {
            case 0: // Perfect
                feedback = Instantiate(Perfect, centerPos, Quaternion.identity);
                streak++;
                isScoringAction = true;
                break;

            case 1: // Miss
                feedback = Instantiate(Miss, centerPos, Quaternion.identity);
                streak = 0;
                goalNote.shake();
                break;

            case 2: // Too Early
            case 3: // Too Late
                feedback = Instantiate(opt == 2 ? TooEarly : TooLate, centerPos, Quaternion.identity);
                streak = 0;
                isScoringAction = true;
                break;
        }

        // Optional: Streak bonus
        // if (streak == 3)
        // {
        //     score += 100;
        //     bonus = Instantiate(bonusStreak, centerPosDown, Quaternion.identity);
        //     Destroy(bonus, 1.0f);
        //     streak = 0;
        // }
        if (isScoringAction)
            UpdateHitProgress(opt);

        streakSlider.value = streak;
        feedback.SetActive(true);
        Destroy(feedback, 0.5f);

        float tb = songManager.getNumBeats();
        float hb = accumulatedHits;
        Debug.Log($"Beats hit: {hb} / Total beats: {tb}");
    }



    public void spawnNew()
    {
        Debug.Log("cycle value " + fullCycles);
        if (allCut)
        {
            return;
        }
        if (level <=1)
        {
            if (fullCycles >= maxCycles)
            {
                string endScene = "EndScene";
                EndLevel(endScene);
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
        }
        else
        {
            if (numCuts >= songManager.getNumBeats())
            {
                tutorialText?.SetText("All vegetables done! Excellent work!");
                //allCut = true;
                string endScene = "EndScene";
                EndLevel(endScene);
                return;

            }
            switch (fullCycles % 2)
            {
                case 0:
                    SpawnCarrot();
                    songManager.isCarrot = true;
                    songManager.isPotato = false;
                    songManager.isOnion = false;
                    numCuts += 4;
                    break;
                case 1:
                    SpawnOnion();
                    songManager.isCarrot = false;
                    songManager.isPotato = false;
                    songManager.isOnion = true;
                    numCuts += 11;
                    break;
            }

            fullCycles++;
            cyclesThisLoop++;
        }
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
        return true;
    }

    public bool getAllCut(){return allCut;}

    public void EndLevel(string endSceneName)
    {
        if (allCut) return;
        allCut = true;

        float hitBeats    = accumulatedHits;
        float totalBeats = totalExpectedNotes;
        int percentScore  = Mathf.RoundToInt((hitBeats / totalBeats) * 100);

        Debug.Log($"[EndLevel] accumulatedHits={accumulatedHits} totalExpectedNotes={totalExpectedNotes}");
        PlayerPrefs.SetInt("beatPercent", percentScore);
        PlayerPrefs.SetString("scene", currScene);
        PlayerPrefs.SetInt($"best_{currScene}", percentScore);
        ScoreManager.previousScores[currScene] = percentScore;
        PlayerPrefs.Save();
        Debug.Log($"[FINAL SAVE] Beats {hitBeats}/{totalBeats} → {percentScore}%");
        SceneManager.LoadScene(endSceneName);
    }

}