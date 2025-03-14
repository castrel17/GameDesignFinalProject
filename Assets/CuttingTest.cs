using UnityEngine;

public class CuttingTest : MonoBehaviour
{
    public GameObject[] slices;
    public int indexToSlice;
    public bool hit = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        indexToSlice = slices.Length - 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            slice();
        }
    }
    public void slice()
    {
        if (hit && indexToSlice > 0)
        {
            for(int i = indexToSlice; i < slices.Length; i++)
            {
                slices[i].transform.position += Vector3.right;
            }
        }
        if(indexToSlice > 0)
        {
            indexToSlice--;
        }
    }
}
