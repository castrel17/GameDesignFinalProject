using UnityEngine;

public class VegetablePeeler : MonoBehaviour
{
    public GameObject[] peelObjects;
    public float holdTimeRequired = 1f;

    private float holdTimer = 0f;
    private int peelCount = 0;
    private bool fullyPeeled = false;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
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
            //if the player actually pressed down on the timer still peel
            if(holdTimer >= 0.2f)
            {
                PeelOneSection();
            }
            holdTimer = 0f;
        }
    }

    private void PeelOneSection()
    {
        if (peelCount < 3)
        {
            //trigger potato peeling animation
            animator.SetTrigger("Next");
            Debug.Log("Peeled section: ");

            peelCount++;

            if (peelCount == 2)
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