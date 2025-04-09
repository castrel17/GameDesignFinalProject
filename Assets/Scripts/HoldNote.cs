using UnityEngine;

public class HoldNote : MonoBehaviour
{
    private DemoLevelManager levelManager;
    private DemoSongManager songManager;

    private bool inZone = false;
    private bool isHolding = false;
    private bool heldPerfect = false;
    private bool triggered = false;

    public float holdSeconds;
    public float holdTimer = 0f;

    public float beatDur;
    public float myBeat;
    public Vector2 startingPosition;
    public Vector2 endingPosition;

    private float hitYMin = -0.2f;
    private float hitYMax = 0.2f;

    void Start()
    {
        levelManager = GameObject.Find("GameManager").GetComponent<DemoLevelManager>();
        songManager = transform.parent.GetComponent<DemoSongManager>();

        float secondsPerBeat = 60f / songManager.bpm;
        holdSeconds = 2f; 
    }

    void Update()
    {
        float t = (songManager.getBeatsPosition() - myBeat) / (beatDur * 2);
        transform.position = Vector2.Lerp(startingPosition, endingPosition, t);

        if (!triggered && transform.position.y >= hitYMin && transform.position.y <= hitYMax)
        {
            Debug.Log("HoldNote Y-collision with goal zone detected.");
            triggered = true;
            inZone = true;
            isHolding = true;
        }

        if (transform.position.y > endingPosition.y && !heldPerfect)
        {
            Debug.Log("HoldNote missed — passed goal without perfect hold.");
            levelManager.spawnFeedback(1);
            Destroy(gameObject);
        }

        if (inZone && isHolding && !heldPerfect)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                holdTimer += Time.deltaTime;

                if (holdTimer >= holdSeconds)
                {
                    heldPerfect = true;
                    Debug.Log("HoldNote held successfully!");

                    levelManager.currentVegetable
                        .GetComponent<VegetablePeeler>()
                        ?.SendMessage("PeelOneSection", SendMessageOptions.DontRequireReceiver);

                    levelManager.spawnFeedback(0);
                    Destroy(gameObject);
                }
            }
            else
            {
                Debug.Log("Released early. Peel anyway.");
                heldPerfect = true;

                levelManager.currentVegetable
                    .GetComponent<VegetablePeeler>()
                    ?.SendMessage("PeelOneSection", SendMessageOptions.DontRequireReceiver);

                levelManager.spawnFeedback(0);
                Destroy(gameObject);
            }
        }
    }
}