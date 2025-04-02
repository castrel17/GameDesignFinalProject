using Unity.Hierarchy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class VegetableCutting : MonoBehaviour
{
    public GameObject[] slices;
    public GameObject pile;
    public int indexToSlice;
    public bool hit = false;
    public bool allCut = false;
    public enum Vegetables { Potato, Carrot, Onion, Tomato};
    public Vegetables vegetableType;
    //private bool peeled = false; //not needed anymore since I made seperate peeler (vegetablepeeler.cs))
    private MusicNote musicNote;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public float slideSpeed = 100.0f;
    private Vector3 offScreenPosition = new Vector3(100f, 0f, 0f);

    //these are only for the onion
    private int[][] horizontals;
    private int[][] verticals;
    private int horizontalIndex = 0;
    private int verticalIndex = 0;
    private List<GameObject> piles;
    //beat sequence variable
    public int[] beats;

    void Start()
    {
        indexToSlice = slices.Length - 1;
        if(vegetableType == Vegetables.Onion)
        {
            horizontals = new int[][]
            {
                new int[]{8,14 },
                new int[]{3,9,15,20},
                new int[]{0,4,10,16,21,25},
                new int[]{1,5,11,17,22,26 },
                new int[]{2,6,12,18,23 },
                new int[]{7,13,19,24}
            };

            verticals = new int[][]
            {
                new int[]{0,1,2},
                new int[]{3,4,5,6,7},
                new int[]{8,9,10,11,12,13 },
                new int[]{14,15,16,17,18,19},
                new int[]{20,21,22,23,24},
                new int[]{25,26,27 }
            };

        }
        piles = new List<GameObject>();
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
            foreach(var newPiles in piles)
            {
                Destroy(newPiles);
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
        VegetablePeeler peeler = GetComponent<VegetablePeeler>();
        if (peeler != null && !peeler.IsFullyPeeled())
        {
            Debug.Log("Cannot cut yet, potato not fully peeled!");
            return;
        }

        if (hit && indexToSlice == 3 && !allCut)
        {
            // cut in half horizontally
            slices[3].transform.position += (Vector3.up * 0.5f);
            slices[2].transform.position += (Vector3.up * 0.5f);
        }

        if (hit && indexToSlice == 2 && !allCut)
        {
            // cut vertically
            slices[3].transform.position += (Vector3.right * 0.5f);
            slices[2].transform.position += (Vector3.left * 0.5f);
            slices[1].transform.position += (Vector3.right * 0.5f);
            slices[0].transform.position += (Vector3.left * 0.5f);
        }

        if (indexToSlice > 1)
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
        //do horizontal cutting first
        if(hit && horizontalIndex < 6 && !allCut)
        {
            for (int i = horizontalIndex; i >= 0; i--)
            {
                foreach(int j in horizontals[i])
                {
                    slices[j].transform.position += (Vector3.up * 0.2f);
                }
            }
        }
        if (horizontalIndex < 6)
        {
            horizontalIndex++;
        }
        //if we finished the horizontal already start doing the vertical
        if(horizontalIndex >= 6 && verticalIndex < 6 && hit && !allCut)
        {
            //spawn onion pile
            float y = Random.Range(-2.0f, 2.0f);
            float x = Random.Range(2.7f, 4.0f);
            GameObject newPile = Instantiate(pile, new Vector3(x, y, 0f), Quaternion.identity);
            piles.Add(newPile);
        }
        if(horizontalIndex >= 6 && verticalIndex < 6)
        {
            verticalIndex++;
        }
        //if we are done cutting vertically and horizontally then set allcut to true
        if(horizontalIndex >= 6 && verticalIndex >= 6)
        {
            allCut = true;
            Debug.Log("all cut");
        }
    }
}
