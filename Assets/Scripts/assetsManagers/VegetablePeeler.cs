using UnityEngine;

public class VegetablePeeler : MonoBehaviour
{
    public GameObject[] peelObjects;
    private int peelCount = 0;
    private bool fullyPeeled = false;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void TriggerStartPeel()
    {
        Debug.Log("Start peel visual maybe?");
        // Optional: play visual cue or shake
    }

    public void TriggerEndPeel()
    {
        PeelOneSection();
    }

    public void PeelOneSection()
    {
        if (peelCount < 3)
        {
            SoundManager.Instance.peelSFX();
            animator.SetTrigger("Next");
            Debug.Log("Peeled section");

            peelCount++;
            if (peelCount == 3)
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
}