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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Initialize as needed
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
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
    }

    public void spawnNew()
    {
        int rand = Random.Range(0, 3);
        if (rand == 0)
        {
            SpawnPotato();
        }
        else if (rand == 1)
        {
            SpawnCarrot();
        }
        else if (rand == 2)
        {
            SpawnOnion();
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
}
