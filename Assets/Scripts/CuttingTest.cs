using UnityEngine;

public class CuttingTest : MonoBehaviour
{
    public GameObject[] slices;
    public int indexToSlice;
    public bool hit = false;
    public bool allCut = false;
    private SongManager songManager;

    private MusicNote musicNote;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public float slideSpeed = 100.0f;
    private Vector3 offScreenPosition = new Vector3(100f, 0f, 0f);
    void Start()
    {
        indexToSlice = slices.Length - 1;
    }

    // Update is called once per frame
    void Update()
    {
        
        //once all cut move off the screen
        if (allCut){
            foreach (var slice in slices)
            {
                slice.transform.position = Vector3.MoveTowards(slice.transform.position, offScreenPosition, slideSpeed * Time.deltaTime);
            }
        }
    }
    public void slice()
    {
        if (hit && indexToSlice > 0 && !allCut)
        {
            for(int i = indexToSlice; i < slices.Length; i++)
            {
                slices[i].transform.position += Vector3.right;
            }
        }
        if(indexToSlice > 0)
        {
            indexToSlice--;
        }else{
            allCut = true;
            Debug.Log("all cut");
        }
    }
}
