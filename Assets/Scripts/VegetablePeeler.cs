using UnityEngine;

public class VegetablePeeler : MonoBehaviour
{
    public GameObject[] peelObjects;
    public float holdTimeRequired = 1f;

    private float holdTimer = 0f;
    private bool isHolding = false;
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

        if (isHolding)
        {
            holdTimer += Time.deltaTime;

            if (holdTimer >= holdTimeRequired)
            {
                PeelOneSection();
                holdTimer = 0f;
                isHolding = false; // force stop holding even if space is still down
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) && !isHolding)
        {
            isHolding = true;
            holdTimer = 0f; // reset timer at start of new hold
        }

        if (Input.GetKeyUp(KeyCode.Space) && isHolding)
        {
            if (holdTimer >= 0.2f)
            {
                PeelOneSection(); // allow early peel
            }
            isHolding = false;
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

            if (peelCount == 2)
            {
                fullyPeeled = true;
                Debug.Log("Vegetable fully peeled!");
            }
            peelCount++;
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