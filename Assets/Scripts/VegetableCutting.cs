using UnityEngine;

public class VegetableCutting : MonoBehaviour
{
    public GameObject[] slices;
    public int indexToSlice;
    public bool hit = false;
    public bool allCut = false;
    private SongManager songManager;
    public enum Vegetables { Potato, Carrot, Onion, Tomato};
    public Vegetables vegetableType;
    private bool peeled = false;
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
        //slicing logic for carrot and tomato 
        if (vegetableType == Vegetables.Carrot || vegetableType == Vegetables.Tomato)
        {
            genericCutting();
        }
        //slicing logic for potato
        if(vegetableType == Vegetables.Potato)
        {
            potatoCutting();
        }
        //slicing logic for onion
        if(vegetableType == Vegetables.Onion)
        {
            onionCutting();
        }
        SoundManager.Instance.PlayRandomSound();
        
    }
    private void potatoCutting()
    {
        //if we are currently peeling the potato
        if (hit && indexToSlice > 3 && !allCut)
        {
            for (int i = indexToSlice; i < slices.Length; i++)
            {
                slices[i].transform.position += (Vector3.right * 1.5f);
            }
            if(indexToSlice == 4)
            {
                peeled = true;
            }
        }
        //if we are cutting the peeled potato
        //do horizontal middle cut
        if (hit && indexToSlice == 3 && !allCut)
        {
            //if the player failed to fully peel the skin move all of the skin over
            if (!peeled)
            {
                for (int i = 4; i < slices.Length; i++)
                {
                    slices[i].transform.position += (Vector3.right * 1.5f);
                }
                peeled = true;
            }
            //cut in half horizontally
            slices[3].transform.position += (Vector3.up * 0.5f);
            slices[2].transform.position += (Vector3.up * 0.5f);

        }
        //do vertical middle cut
        if (hit && indexToSlice == 2 && !allCut)
        {
            //if the player failed to fully peel the skin move all of the skin over
            if (!peeled)
            {
                for (int i = 4; i < slices.Length; i++)
                {
                    slices[i].transform.position += (Vector3.right * 1.5f);
                }
                peeled = true;
            }
            //cut top in half
            slices[3].transform.position += (Vector3.right * 0.5f);
            slices[2].transform.position += (Vector3.left * 0.5f);
            //cut  bottom in half
            slices[1].transform.position += (Vector3.right * 0.5f);
            slices[0].transform.position += (Vector3.left * 0.5f);

        }
        if (indexToSlice > 2)
        {
            indexToSlice--;
        }
        else
        {
            allCut = true;
            Debug.Log("all cut");
        }
    }
    private void genericCutting()
    {
        if (hit && indexToSlice > 0 && !allCut)
        {
            for (int i = indexToSlice; i < slices.Length; i++)
            {
                slices[i].transform.position += Vector3.right;
            }
        }
        if (indexToSlice > 0)
        {
            indexToSlice--;
        }
        else
        {
            allCut = true;
            Debug.Log("all cut");
        }
    }
    private void onionCutting()
    {

    }
}
