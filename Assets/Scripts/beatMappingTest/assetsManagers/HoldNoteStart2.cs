using UnityEngine;

public class HoldNoteStart2 : MonoBehaviour
{
    private float beat;
    private Vector2 startPos;
    private Vector2 endPos;
    private Level1SongManager songManager;
    private Level1Manager levelManager;
    public bool isHeld = false;

    public bool evaluated { get; private set; } = false;
    public bool isMissedStart { get; private set; } = false;

    public void Initialize(float beat, Vector2 start, Vector2 end, Level1SongManager songMgr)
    {
        this.beat = beat;
        this.startPos = start;
        this.endPos = end;
        this.songManager = songMgr;
        this.levelManager = GameObject.Find("GameManager").GetComponent<Level1Manager>();
    }

    void Update()
    {
        if (isHeld) return;
        if (songManager == null) return;

        float currentBeat = songManager.getBeatsPosition();
        float travelBeats = songManager.noteTravelBeats;

        // Align the center of the note to the beat
        float t = 1 - ((beat - currentBeat) / travelBeats);
        transform.position = Vector2.Lerp(startPos, endPos, Mathf.Clamp01(t));

        // Auto-miss when past the screen
        float missCutoffBeat = beat - travelBeats * 0.1f;
        if (!evaluated && currentBeat > missCutoffBeat)
        {
            levelManager.spawnFeedback(1); // Miss
            //dequeue music note
            songManager.dequeueNote();
            //peel potato 
            levelManager.currentVegetable.GetComponent<VegetablePeeler>().TriggerEndPeel();
            evaluated = true;
            isMissedStart = true;
            return;
        }
    }

    void Evaluate(float delta, float currentBeat, float idealBeat)
    {
        if (delta < 0.2f)
        {
            isHeld = true;
            levelManager.spawnFeedback(0); // Perfect
        }
        else if (delta > 1f)
        {
            levelManager.spawnFeedback(1); // Miss
            var peeler = levelManager.currentVegetable.GetComponent<VegetablePeeler>();
            peeler?.PeelOneSection();
            isMissedStart = true;

        }
        else if (currentBeat < idealBeat)
        {
            isHeld = true;
            levelManager.spawnFeedback(2); // Too Early
        }
        else
        {
            isHeld = true;
            levelManager.spawnFeedback(3); // Too Late
        }

        evaluated = true;
    }

    public void notePressed()
    {
        float currentBeat = songManager.getBeatsPosition();
        float idealBeat = beat - (songManager.noteTravelBeats / 2f);
        float delta = Mathf.Abs(currentBeat - idealBeat);

        Debug.Log(
               $"[HoldNoteStart] Pressed at beat={currentBeat:F2}, ideal={idealBeat:F2}, delta={delta:F2}"
               + $" → before Evaluate"
               );
        levelManager.currentVegetable.GetComponent<VegetablePeeler>()?.TriggerStartPeel();
        Evaluate(delta, currentBeat, idealBeat);

        songManager.activeHoldNote = GetComponentInParent<HoldNoteController2>();
    }
}