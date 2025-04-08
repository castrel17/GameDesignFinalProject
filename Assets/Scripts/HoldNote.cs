using UnityEngine;

public class HoldNote : MonoBehaviour
{
    private DemoLevelManager levelManager;
    private DemoSongManager songManager;
    private Collider2D trigger;
    private bool inZone = false;
    private bool heldPerfect = false;

    // Total time required to hold (in seconds), manually set as 2 in start()
    public float holdSeconds;
    // Accumulated time the player holds the button.
    public float holdTimer = 0f;

    public float beatDur;
    public float myBeat;
    public Vector2 startingPosition;
    public Vector2 endingPosition;

    void Start()
    {
        levelManager = GameObject.Find("GameManager").GetComponent<DemoLevelManager>();
        songManager = transform.parent.GetComponent<DemoSongManager>();
        trigger = GetComponent<Collider2D>();
        trigger.isTrigger = true;
        trigger.enabled = false;

        // Calculate holdSeconds based on the note's beat duration and BPM.
        float secondsPerBeat = 60f / songManager.bpm;
        //holdSeconds = beatDur * secondsPerBeat;
        holdSeconds = 2f;
        Debug.Log("HoldNote initialized. Required holdSeconds: " + holdSeconds);
    }

    void Update()
    {
        // While the note is not yet in the goal zone, move it up like Music Note
        if (!inZone)
        {
            float t = (songManager.getBeatsPosition() - myBeat) / (beatDur * 2);
            transform.position = Vector2.Lerp(startingPosition, endingPosition, t);

            if (transform.position.y >= endingPosition.y)
            {
                Debug.Log("HoldNote reached end without collision.");
                levelManager.spawnFeedback(1);
                Destroy(gameObject);
            }
        }
        else
        {
            // In the goal zone.
            if (Input.GetKey(KeyCode.Space))
            {
                holdTimer += Time.deltaTime;
                Debug.Log("HoldTimer increased: " + holdTimer);

                // When hold time is met, register it.
                if (holdTimer >= holdSeconds && !heldPerfect)
                {
                    heldPerfect = true;
                    Debug.Log("HoldTimer complete. Note should now be registered as hit.");
                    VegetablePeeler peeler = levelManager.currentVegetable.GetComponent<VegetablePeeler>();
                    if (peeler != null)
                    {
                        peeler.SendMessage("PeelOneSection", SendMessageOptions.DontRequireReceiver);
                    }
                    levelManager.spawnFeedback(0);
                    Destroy(gameObject);
                }
            }
            else
            {
                // When space is not held we log the fact that player is not holding space.
                Debug.Log("Player is not holding space. holdTimer remains at: " + holdTimer);
                // Note: holdTimer is not reset here.
            }
        }
    }

    // When any part of the note collides with the Goal, start the hold phase.
    // THIS DOES NOT WORK AT ALL
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Goal"))
        {
            inZone = true;
            trigger.enabled = true;
            Debug.Log("HoldNote entered Goal. Starting hold phase.");
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Goal"))
        {
            if (!heldPerfect)
            {
                Debug.Log("HoldNote exited Goal without being held long enough.");
                levelManager.spawnFeedback(1);
            }
            Destroy(gameObject);
        }
    }
}
