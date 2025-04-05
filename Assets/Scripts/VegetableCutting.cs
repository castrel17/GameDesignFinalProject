using Unity.Hierarchy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class VegetableCutting : MonoBehaviour
{
    public GameObject[] slices;
    public GameObject pile;
    public int numberOfCuts;
    public bool allCut = false;
    public enum Vegetables { Potato, Carrot, Onion, Tomato};
    public Vegetables vegetableType;
    //private bool peeled = false; //not needed anymore since I made seperate peeler (vegetablepeeler.cs))
    private MusicNote musicNote;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public float slideSpeed = 100.0f;
    private Vector3 offScreenPosition = new Vector3(100f, -2.0f, 0f);

    //these are only for the onion
    private int horizontalCuts = 0;
    private int verticalCuts = 0;

    private List<GameObject> piles;
    private Animator animator;

    //beat sequence variable
    public int[] beats;

    void Start()
    {
        animator = GetComponent<Animator>();
        piles = new List<GameObject>();
        if (vegetableType == Vegetables.Carrot)
        {
            numberOfCuts = 4;
        }
        else if(vegetableType == Vegetables.Potato)
        {
            numberOfCuts = 2;
            animator = GetComponent<Animator>();
        }
        else if(vegetableType == Vegetables.Onion)
        {
            verticalCuts = 6;
            horizontalCuts = 6;
        }

        
        
    }

    // Update is called once per frame
    void Update()
    {
        //once all cut move off the screen
        if(allCut)
        {
            //move vegetable off screen
            this.transform.position = Vector3.MoveTowards(this.transform.position, offScreenPosition, slideSpeed * Time.deltaTime);

            foreach (var newPiles in piles)
            {
                Destroy(newPiles);
            }
        }
    }
    public void slice()
    {
        //slicing logic for carrot
        if (vegetableType == Vegetables.Carrot)
        {
            carrotCutting();
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
    }
    private void potatoCutting()
    {
        VegetablePeeler peeler = GetComponent<VegetablePeeler>();
        if (peeler != null && !peeler.IsFullyPeeled())
        {
            Debug.Log("Cannot cut yet, potato not fully peeled!");
            return;
        }
        //trigger potato cutting animation and play sound
        SoundManager.Instance.PlayRandomSound();
        animator.SetTrigger("Next");

        if (numberOfCuts > 0)
        {
            numberOfCuts--;
        }
        else
        {
            allCut = true;
            Debug.Log("all cut");
        }
    }
    private void carrotCutting()
    {
        //trigger cutting and play sound
        SoundManager.Instance.PlayRandomSound();
        animator.SetTrigger("Next");
        

        //spawn carrot pile
        float y = Random.Range(-4.0f, 0.0f);
        float x = Random.Range(2.7f, 4.0f);
        GameObject newPile = Instantiate(pile, new Vector3(x, y, 0f), Quaternion.identity);
        piles.Add(newPile);

        if (numberOfCuts > 0)
        {
            numberOfCuts--;
        }
        else
        {
            allCut = true;
            Debug.Log("all cut");
        }
    }
    private void onionCutting()
    {
        //trigger cutting and play sound
        SoundManager.Instance.PlayRandomSound();
        animator.SetTrigger("Next");

        //do vertical cutting first
        if (verticalCuts > 0)
        {
            verticalCuts--;
        }
        //if we finished the horizontal already start doing the vertical
        if(verticalCuts == 0 && horizontalCuts > 0 && !allCut)
        {
            //spawn onion pile
            float y = Random.Range(-4.0f, 0.0f);
            float x = Random.Range(2.7f, 4.0f);
            GameObject newPile = Instantiate(pile, new Vector3(x, y, 0f), Quaternion.identity);
            piles.Add(newPile);
        }
        if(horizontalCuts > 0  && verticalCuts == 0)
        {
            horizontalCuts--;
        }
        //if we are done cutting vertically and horizontally then set allcut to true
        if(horizontalCuts == 0  && verticalCuts == 0)
        {
            allCut = true;
            Debug.Log("all cut");
        }
    }
}
