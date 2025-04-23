using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Level2Manager : MonoBehaviour
{
    // prefabs
    public GameObject potatoPrefab, carrotPrefab, onionPrefab;
    // single-veg positions (same as before)
    // private Vector3 singleOff = new Vector3(-10f, -3.25f, 0f);
    // private Vector3 singleTarget = new Vector3(  0f, -3.25f, 0f);
    public Transform singleSpawnPoint;
    public Transform singleTargetPoint;


    // dual-veg positions: left vs right lanes
    //public Vector3 leftOff, rightOff, leftTarget, rightTarget;
    //public Transform leftSpawnPoint, rightSpawnPoint, leftTargetPoint, rightTargetPoint;
    public Transform leftSpawnPoint;
    public Transform rightSpawnPoint;
    public Transform leftTargetPoint;
    public Transform rightTargetPoint;

    // current veggies in play
    private GameObject currentVegLeft, currentVegRight;
    private bool needLeft = true, needRight = true;

    // song & UI
    public DemoSongManager songManager;
    public TextMeshProUGUI tutorialText, scoreText;
    public Slider streakSlider;
    public ScoreBar scoreBar;
    public GoalNote goalNote;
    public GameObject Perfect, Miss, TooEarly, TooLate, bonusStreak;
    private int score = 0, streak = 0;

    // cycle tracking
    public int level = 2;
    private int fullCycles = 0, maxCycles;
    private bool dualMode = false;  

    public float slideSpeed = 5f;
    Vector3 centerPos     = new Vector3(0f,  2f, 0f);
    Vector3 centerPosDown = new Vector3(0f, -2f, 0f);
    GameObject feedback, bonus;


    void Start()
    {
        maxCycles = (level == 0) ? 21 : 50;
        // initialize your left/right positions however you like, e.g.:
        // leftOff    = new Vector3(-8f, -3.25f, 0f);
        // rightOff   = new Vector3( 8f, -3.25f, 0f);
        // leftTarget = new Vector3(-2f, -3.25f, 0f);
        // rightTarget= new Vector3( 2f, -3.25f, 0f);
    }

    void Update()
    {
        if (!songManager.gameOver && songManager.loopStarted)
        {
            // 1) single-veg mode
            if (!dualMode && songManager.startStatus() && needLeft)
            {
                needLeft = false;
                //SpawnSingle(fullCycles % 3, singleOff, singleTarget, out currentVegLeft);
                SpawnSingle(fullCycles % 3,
                            singleSpawnPoint.position,
                            singleTargetPoint.position,
                            out currentVegLeft);
                songManager.ResetNoteCounter();
            }

            // slide & check single-veg
            if (!dualMode && currentVegLeft != null)
            {
                SlideAndCheck(currentVegLeft,
                                singleTargetPoint.position,
                                ref needLeft);
                if (!needLeft && ++fullCycles == 3)
                    dualMode = true;      // flip into dual mode after carrot→potato→onion
            }

            // 2) dual-veg mode
            if (dualMode)
            {
                if (needLeft && needRight)
                {
                    // spawn two random veggies
                    int typeL = Random.Range(0,5);       // 0: C×C,1:C×P,2:P×O,3:O×O,4:O×C
                    int typeR = (typeL + 1) % 3;         // or decode both from a table

                    SpawnSingle(typeL,
                                leftSpawnPoint.position,
                                leftTargetPoint.position,
                                out currentVegLeft);

                    SpawnSingle(typeR,
                                rightSpawnPoint.position,
                                rightTargetPoint.position,
                                out currentVegRight);

                    needLeft  = needRight = false;
                    songManager.ResetNoteCounter();
                }

                // slide each
                SlideAndCheck(currentVegLeft,
                                leftTargetPoint.position,
                                ref needLeft);
                SlideAndCheck(currentVegRight,
                            rightTargetPoint.position,
                            ref needRight);

                // once both done, increment cycles and maybe end
                if (!needLeft && !needRight)
                {
                    fullCycles++;
                    needLeft  = needRight = true;
                    if (fullCycles >= maxCycles) tutorialText.text = "All veggies done!";
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.Tab))
            SceneManager.LoadScene(0);
    }

    void SpawnSingle(int type, Vector3 offPos, Vector3 tgtPos, out GameObject veg)
    {
        veg = null;
        switch (type)
        {
            case 0: veg = Instantiate(carrotPrefab, offPos, Quaternion.identity); break;
            case 1: veg = Instantiate(potatoPrefab, offPos, Quaternion.identity); break;
            case 2: veg = Instantiate(onionPrefab,  offPos, Quaternion.identity); break;
        }
        // tell songManager which vegetable type for note‐spawning logic 
        songManager.isCarrot = (type == 0);
        songManager.isPotato = (type == 1);
        songManager.isOnion  = (type == 2);
    }

    void SlideAndCheck(GameObject veg, Vector3 target, ref bool needFlag)
    {
        if (veg == null) return;
        veg.transform.position = Vector3.MoveTowards(
            veg.transform.position,
            target,
            slideSpeed * Time.deltaTime
        );
        if (IsProcessed(veg))
        {
            Destroy(veg, 0.2f);
            needFlag = true;
        }
    }

    bool IsProcessed(GameObject veg)
    {
        var peeler = veg.GetComponent<VegetablePeeler>();
        var cutter = veg.GetComponent<VegetableCutting>();
        return (peeler  == null || peeler.IsFullyPeeled())
            && (cutter == null || cutter.allCut);
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
}
