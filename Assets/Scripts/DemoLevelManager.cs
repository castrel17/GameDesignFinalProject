using UnityEngine;
using TMPro;

public class DemoLevelManager : MonoBehaviour
{
    public GameObject potatoPrefab;
    public GameObject carrotPrefab;
    public GameObject onionPrefab;

    public float slideSpeed = 5f; 
    private Vector3 targetPosition = new Vector3(0f, 0f, 0f);
    private Vector3 offScreenPosition = new Vector3(-10f, 0f, 0f);

    public GameObject currentVegetable;
    private bool isSliding = false;

    public SongManager songManager;
    public bool needVeg = true;

    public TextMeshProUGUI tutorialText;

    private int spawnIndex = 0;

    void Start()
    {
        if (tutorialText != null)
        {
            tutorialText.text = "Press any key to start!";
        }
    }

    void Update()
    {
        if (!songManager.gameOver)
        {
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
                tutorialText.text = "All done cutting! Good job!";
            }

            needVeg = true;
            currentVegetable = null;

            spawnIndex++;
        }
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
}
