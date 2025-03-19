using UnityEngine;

public class DemoLevelManager : MonoBehaviour
{
    // Prefabs for each vegetable
    //public GameObject tomatoPrefab;
    public GameObject potatoPrefab;
    public GameObject carrotPrefab;
    public GameObject onionPrefab;

    // Speed and positions for sliding
    public float slideSpeed = 5f; 
    private Vector3 targetPosition = new Vector3(0f, 0f, 0f);  
    private Vector3 offScreenPosition = new Vector3(-10f, 0f, 0f); 

    public GameObject currentVegetable; 
    private bool isSliding = false;

    public SongManager songManager;

    public bool needVeg = true;

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Initialize as needed
    }

    // Update is called once per frame
    void Update()
    {
        if(!songManager.gameOver){
            if(songManager.startStatus() && needVeg){
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
                VegetableCutting vegetableCutting = currentVegetable.GetComponent<VegetableCutting>();
                if (vegetableCutting != null && vegetableCutting.allCut)
                {
                    needVeg = true;
                    currentVegetable = null;
                }
            }
        }
    }

    public void spawnNew()
    {
        int rand = Random.Range(0, 3);
        if (rand == 0)
        {
            SpawnPotato();
            songManager.isPotato = true;
            songManager.isCarrot= false;
            songManager.isOnion= false;
        }
        else if (rand == 1)
        {
            SpawnCarrot();
            songManager.isPotato = false;
            songManager.isCarrot= true;
            songManager.isOnion= false;
        }
        else if (rand == 2)
        {
            SpawnOnion();
            songManager.isPotato = false;
            songManager.isCarrot= false;
            songManager.isOnion= true;
        }
    }

    //void SpawnTomato()
    //{
    //    currentVegetable = Instantiate(tomatoPrefab, offScreenPosition, Quaternion.identity);
    //    isSliding = true;
    //}

    void SpawnPotato()
    {
        currentVegetable = Instantiate(potatoPrefab, offScreenPosition, Quaternion.identity);
        isSliding = true;
        Debug.Log("spawn potato");
    }

    void SpawnCarrot()
    {
        currentVegetable = Instantiate(carrotPrefab, offScreenPosition, Quaternion.identity);
        isSliding = true;
        Debug.Log("spawn carrot");
    }

    void SpawnOnion()
    {
        currentVegetable = Instantiate(onionPrefab, offScreenPosition, Quaternion.identity);
        isSliding = true;
        Debug.Log("spawn onion");
    }
}
