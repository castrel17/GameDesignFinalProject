using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.Properties;
using UnityEngine.SceneManagement;

//test commit
public class DemoLevelManager : MonoBehaviour
{
    public GameObject potatoPrefab;
    public GameObject carrotPrefab;
    public GameObject onionPrefab;

    public float slideSpeed = 5f;
    //private Vector3 targetPosition = new Vector3(0f, 0f, 0f);
    private Vector3 targetPosition = new Vector3(0f, -2.0f, 0f);

    private Vector3 offScreenPosition = new Vector3(-10f, -2.0f, 0f);

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

    public GoalNote goalNote;
    private int streak;

    public TextMeshProUGUI scoreText;
    private int score = 0;
   Vector3 centerPos = new Vector3(0f, 2f, 0f);

   Vector3 centerPosDown = new Vector3(0f, -2f, 0f);

   public Slider streakSlider;


    void Start()
    {
    }

    void Update()
    {
        if (!songManager.gameOver)
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
                currentVegetable.transform.position = Vector3.MoveTowards(
                    currentVegetable.transform.position,
                    targetPosition,
                    slideSpeed * Time.deltaTime
                );

                if (currentVegetable.transform.position == targetPosition)
                {
                    isSliding = false;
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
            if (tutorialText != null && tutorialText.text != "Great peeling! Now chop")
            {
                tutorialText.text = "Great peeling! Now chop";
            }
        }
        else if (cutting != null && cutting.allCut)
        {
            if (tutorialText != null)
            {
                tutorialText.text = "All done cutting! Good job! Hit tab to go to start";
            }

            needVeg = true;
            currentVegetable = null;

            spawnIndex++;
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
        }else if(opt == 1){
            feedback = Instantiate(Miss, centerPos, Quaternion.identity);
            streak = 0;
            goalNote.shake();
        }else if(opt == 2){
            feedback = Instantiate(TooEarly, centerPos, Quaternion.identity);
            streak = 0;
            score += 50;
        }else{
            feedback = Instantiate(TooLate, centerPos, Quaternion.identity);
            streak = 0;
            score += 50;
        } 

        if(streak == 3){
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
        if (spawnIndex >= 3)
        {
            if (tutorialText != null)
            {
                tutorialText.text = "All vegetables done! Excellent work!";
            }
            Debug.Log("No more vegetables to spawn!");
            songManager.StopMusic();
            return;
        }

        // vegetable to spawn based on spawnIndex
        // 0 -> Carrot
        // 1, -> Potato
        // 2   -> Onion
        if (spawnIndex < 1)
        {
            SpawnCarrot();
            songManager.isCarrot = true;
            songManager.isPotato = false;
            songManager.isOnion = false;
        }
        else if (spawnIndex < 2)
        {
            SpawnPotato();
            songManager.isCarrot = false;
            songManager.isPotato = true;
            songManager.isOnion = false;
        }
        else
        {
            SpawnOnion();
            songManager.isCarrot = false;
            songManager.isPotato = false;
            songManager.isOnion = true;
        }
    }

    void SpawnPotato()
    {
        currentVegetable = Instantiate(potatoPrefab, offScreenPosition, Quaternion.identity);
        isSliding = true;
        if (tutorialText != null)
        {
            tutorialText.text = "A potato! Hold space to peel until peeled!";
        }
        Debug.Log("Spawned potato");
    }

    void SpawnCarrot()
    {
        currentVegetable = Instantiate(carrotPrefab, offScreenPosition, Quaternion.identity);
        isSliding = true;
        if (tutorialText != null)
        {
            tutorialText.text = "A carrot has appeared! Space to chop";
        }
        Debug.Log("Spawned carrot");
    }

    void SpawnOnion()
    {
        currentVegetable = Instantiate(onionPrefab, offScreenPosition, Quaternion.identity);
        isSliding = true;
        if (tutorialText != null)
        {
            tutorialText.text = "An onion has appeared! chop chop";
        }
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

