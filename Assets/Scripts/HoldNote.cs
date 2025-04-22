using UnityEngine;

public class HoldNote : MonoBehaviour
{
    public float startBeat;
    public float endBeat;
    public Vector2 startingPosition;
    public Vector2 endingPosition;

    private DemoLevelManager levelManager;
    private DemoSongManager songManager;

    private bool startEvaluated = false;
    private bool endEvaluated = false;
    private bool isHolding = false;

    void Awake()
    {
        Debug.Log("[HoldNote] Awake called");
    }

    void Start()
    {
        levelManager = GameObject.Find("GameManager").GetComponent<DemoLevelManager>();
        songManager = GetComponentInParent<DemoSongManager>();

        Debug.Log("[HoldNote] Start called. StartBeat: " + startBeat + ", EndBeat: " + endBeat);
    }

    void Update()
    {
        float currentBeat = songManager.getBeatsPosition();
        float travelBeats = songManager.noteTravelBeats;

        // ✅ Compute t so that t = 0.5 when currentBeat == startBeat
        float visualStart = startBeat - (travelBeats / 2f);
        float t = 1 - ((visualStart - currentBeat) / travelBeats);
        t = Mathf.Clamp01(t);
        transform.position = Vector2.Lerp(startingPosition, endingPosition, t);

        // 🧨 Auto despawn if missed completely
        float missCutoffBeat = endBeat + 0.5f;
        if (currentBeat > missCutoffBeat && (!startEvaluated))
        {
            if (!startEvaluated)
            {
                EvaluateStart(missed: true);
                startEvaluated = true;
            }

            if (!endEvaluated)
            {
                EvaluateEnd(missed: true);
                endEvaluated = true;
            }

            Destroy(gameObject);
            return;
        }

        // 🎯 Start evaluation
        if (!startEvaluated && Input.GetKeyDown(KeyCode.Space))
        {
            EvaluateStart();
            isHolding = true;
            startEvaluated = true;
        }

        if (!startEvaluated && currentBeat > startBeat + 0.5f)
        {
            EvaluateStart(missed: true);
            startEvaluated = true;
        }

        // 🎯 End evaluation
        if (!endEvaluated && Input.GetKeyUp(KeyCode.Space))
        {
            if (isHolding)
            {
                EvaluateEnd();
                isHolding = false;
                endEvaluated = true;
            }
        }

        if (!endEvaluated && currentBeat > endBeat + 0.5f)
        {
            EvaluateEnd(missed: true);
            endEvaluated = true;
        }

        if (startEvaluated && endEvaluated)
        {
            Destroy(gameObject);
        }
    }

    void EvaluateStart(bool missed = false)
    {
        float currentBeat = songManager.getBeatsPosition();
        float delta = Mathf.Abs(currentBeat - startBeat);

        levelManager.currentVegetable.GetComponent<VegetablePeeler>()?.TriggerStartPeel();

        if (missed || delta > 1f)
        {
            levelManager.spawnFeedback(1); // Miss
        }
        else if (delta < 0.2f)
        {
            levelManager.spawnFeedback(0); // Perfect
        }
        else if (currentBeat < startBeat)
        {
            levelManager.spawnFeedback(2); // Too Early
        }
        else
        {
            levelManager.spawnFeedback(3); // Too Late
        }
    }

    void EvaluateEnd(bool missed = false)
    {
        float currentBeat = songManager.getBeatsPosition();
        float delta = Mathf.Abs(currentBeat - endBeat);

        levelManager.currentVegetable.GetComponent<VegetablePeeler>()?.TriggerEndPeel();

        if (missed || delta > 1f)
        {
            levelManager.spawnFeedback(1); // Miss
        }
        else if (delta < 0.2f)
        {
            levelManager.spawnFeedback(0); // Perfect
        }
        else if (currentBeat < endBeat)
        {
            levelManager.spawnFeedback(2); // Too Early
        }
        else
        {
            levelManager.spawnFeedback(3); // Too Late
        }
    }

    public HoldNote()
    {
        Debug.Log("[HoldNote] Constructor called");
    }
}