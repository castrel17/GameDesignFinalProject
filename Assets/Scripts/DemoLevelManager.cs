using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.Properties;
using UnityEngine.SceneManagement;
/*In the demo level
-carrots x7
-potatos x7
-onions x7
*/
public class DemoLevelManager : MonoBehaviour
{
    public GameObject potatoPrefab;
    public GameObject carrotPrefab;
    public GameObject onionPrefab;

    public float slideSpeed = 5f;
    //private Vector3 targetPosition = new Vector3(0f, 0f, 0f);
    private Vector3 targetPosition = new Vector3(0f, -3.25f, 0f);

    private Vector3 offScreenPosition = new Vector3(-10f, -3.25f, 0f);

    public GameObject currentVegetable;
    private bool isSliding = false;

    public DemoSongManager songManager;
    public bool needVeg = true;

    public TextMeshProUGUI tutorialText;

    private int spawnIndex = 0;

    public Image peelFillBar;

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

   Vector3 centerPosDown = new Vector3(0f, -2f, 0f);

   public Slider streakSlider;
   public int fullCycles = 0;
   public int maxCycles = 21;  
   private int cyclesThisLoop = 0;

    void Start()
    {
    }

    void Update()
    {
        if (!songManager.gameOver && songManager.loopStarted)
        { 
            if(songManager.startStatus() && needVeg && spawnIndex > 0)
            {
                songManager.setBaseBool = true;
            }
            if (songManager.startStatus() && needVeg)
            {
                needVeg = false;
                spawnNew();
            }

            if (isSliding && currentVegetable != null)
            {
                Vector3 target = targetPosition;

                currentVegetable.transform.position = Vector3.MoveTowards(
                    currentVegetable.transform.position,
                    target,
                    slideSpeed * Time.deltaTime
                );

                if (currentVegetable.transform.position == target)
                {
                    if (isSliding)
                    {
                        isSliding = false;
                    }
                }
            }
            if (currentVegetable != null)
            {
                CheckVegetableProgress();
            }
        }
        else{
            if(Input.GetKeyDown(KeyCode.Tab)){
                SceneManager.LoadScene(0);
            }
            
        }
        UpdatePeelBar();
    }

    private void CheckVegetableProgress()
    {
        VegetablePeeler peeler = currentVegetable.GetComponent<VegetablePeeler>();
        VegetableCutting cutting = currentVegetable.GetComponent<VegetableCutting>();

        if (peeler != null && !peeler.IsFullyPeeled())
        {
        }
        else if (peeler != null && peeler.IsFullyPeeled() && cutting != null && !cutting.allCut)
        {
        }
        else if (cutting != null && cutting.allCut)
        {
            spawnIndex++;
            needVeg = true;
            if (cyclesThisLoop == 6)
            {
                songManager.loopStarted = false;
                cyclesThisLoop = 0;
                Debug.Log("Completed 2 cycles in this loop");
            }


        }

    }


    //scoring system
    /*
    -perfect = 100pts
    -too early/toolate = 50pts
    -miss = 0pts
    -streaks = 3 perfects = 100pts
    */
    public void spawnFeedback(int opt){ //0 = perfect, 1 = miss, 2 = too early, 3 =  too late
        if(opt == 0){
            feedback = Instantiate(Perfect, centerPos, Quaternion.identity);
            streak++;
            score += 100;
            scoreBar.updateScoreBar(2);
        }else if(opt == 1){
            feedback = Instantiate(Miss, centerPos, Quaternion.identity);
            streak = 0;
            goalNote.shake();
        }else if(opt == 2){
            feedback = Instantiate(TooEarly, centerPos, Quaternion.identity);
            streak = 0;
            score += 50;
            scoreBar.updateScoreBar(1);
        }
        else
        {
            feedback = Instantiate(TooLate, centerPos, Quaternion.identity);
            streak = 0;
            score += 50;
            scoreBar.updateScoreBar(1);
        }

        if (streak == 3){
            Debug.Log("bonus streak hit");
            //inc score and set streak to 0
            score += 100;
            //update score bar
            bonus = Instantiate(bonusStreak, centerPosDown, Quaternion.identity);
            bonus.SetActive(true);
            Destroy(bonus, 1.0f);
            streak = 0;
        }
        
        streakSlider.value = streak;
        feedback.SetActive(true); 
        Destroy(feedback, 1.0f);
        scoreText.text = "Score: " + score;
        Debug.Log("streak: " + streak);
    }
    public void spawnNew()
    {
        if (fullCycles >= maxCycles)
        {
            if (tutorialText != null)
            {
                tutorialText.text = "All vegetables done! Excellent work!";
            }
            Debug.Log("No more vegetables to spawn! " + fullCycles + " " + maxCycles);
            return;
        }

        int cycleIndex = fullCycles % 3;
        //carrot → potato → onion
        switch (fullCycles % 3)
        {
            case 0: //carrot
                SpawnCarrot();
                songManager.isCarrot = true;
                songManager.isPotato = false;
                songManager.isOnion = false;
                break;
                
            case 1: //potato
                SpawnPotato();
                songManager.isCarrot = false;
                songManager.isPotato = true;
                songManager.isOnion = false;
                break;

            case 2: //onion
                SpawnOnion();
                songManager.isCarrot = false;
                songManager.isPotato = false;
                songManager.isOnion = true;
                break;
        }

        fullCycles++;
        Debug.Log("full cycles: " + fullCycles);
        cyclesThisLoop++;

    }



    void SpawnPotato()
    {
        currentVegetable = Instantiate(potatoPrefab, offScreenPosition, Quaternion.identity);
        isSliding = true;
        Debug.Log("Spawned potato");
    }

    void SpawnCarrot()
    {
        currentVegetable = Instantiate(carrotPrefab, offScreenPosition, Quaternion.identity);
        isSliding = true;
        Debug.Log("Spawned carrot");
    }

    void SpawnOnion()
    {
        currentVegetable = Instantiate(onionPrefab, offScreenPosition, Quaternion.identity);
        isSliding = true;
        if (tutorialText != null)
        Debug.Log("Spawned onion");
    }

    void UpdatePeelBar()
    {
        if (peelFillBar == null) return;

        if (currentVegetable == null)
        {
            peelFillBar.gameObject.SetActive(false);
            return;
        }

        VegetablePeeler peeler = currentVegetable.GetComponent<VegetablePeeler>();
        if (peeler == null)
        {
            peelFillBar.gameObject.SetActive(false);
            return;
        }

        bool showBar = !peeler.IsFullyPeeled();
        peelFillBar.gameObject.SetActive(showBar);

        if (showBar)
        {
            float progress = peeler.GetHoldProgress();
            peelFillBar.fillAmount = progress;
        }
    }
}

