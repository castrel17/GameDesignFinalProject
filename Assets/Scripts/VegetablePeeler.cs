using UnityEngine;

public class VegetablePeeler : MonoBehaviour
{
    public GameObject[] peelObjects;
    public float holdTimeRequired = 1f;

    private float holdTimer = 0f;
    private int peelIndex = 0;
    private bool fullyPeeled = false;


    void Start()
    {
        foreach (var peelObj in peelObjects)
        {
            if (peelObj != null) peelObj.SetActive(true);
        }
    }

    void Update()
    {
        if (fullyPeeled) return;

        if (Input.GetKey(KeyCode.Space))
        {
            holdTimer += Time.deltaTime;

            if (holdTimer >= holdTimeRequired)
            {
                PeelOneSection();
                holdTimer = 0f;
            }
        }
        else
        {
            holdTimer = 0f;
        }
    }

    private void PeelOneSection()
    {
        if (peelIndex < peelObjects.Length)
        {
            if (peelObjects[peelIndex] != null)
            {
                peelObjects[peelIndex].SetActive(false);
                Debug.Log("Peeled section: " + peelObjects[peelIndex].name);
            }
            peelIndex++;

            if (peelIndex >= peelObjects.Length)
            {
                fullyPeeled = true;
                Debug.Log("Vegetable fully peeled!");
            }
        }
    }

    public bool IsFullyPeeled()
    {
        return fullyPeeled;
    }

    public float GetHoldProgress()
    {
        if (fullyPeeled) return 1f;
        return Mathf.Clamp01(holdTimer / holdTimeRequired);
    }
}
