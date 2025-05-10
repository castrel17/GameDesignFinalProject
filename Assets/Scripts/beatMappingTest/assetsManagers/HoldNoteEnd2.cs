using UnityEngine;

public class HoldNoteEnd2 : MonoBehaviour
{
    private float beat;
    private Vector2 startPos;
    private Vector2 endPos;
    private Level1SongManager songManager;
    private Level1Manager levelManager;

    public bool evaluated { get; private set; } = false;

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
        if (songManager == null) return;

        float t = 1 - ((beat - songManager.getBeatsPosition()) / songManager.noteTravelBeats);
        transform.position = Vector2.Lerp(startPos, endPos, Mathf.Clamp01(t));

        if (!evaluated)
        {
            // Only evaluate key release if this is the currently active hold note
            if (songManager.activeHoldNote == GetComponentInParent<HoldNoteController>())
            {

                float currentBeat = songManager.getBeatsPosition();

                if (Input.GetKeyUp(KeyCode.Space))
                {
                    float delta = Mathf.Abs(currentBeat - beat);
                    float idealBeat = beat - (songManager.noteTravelBeats / 2f);
                    levelManager.currentVegetable.GetComponent<VegetablePeeler>()?.TriggerEndPeel();
                    Evaluate(delta, currentBeat, idealBeat);

                    songManager.activeHoldNote = null;
                }
                else if (currentBeat > beat + 0.5f)
                {
                    levelManager.currentVegetable.GetComponent<VegetablePeeler>()?.TriggerEndPeel();
                    levelManager.spawnFeedback(1); // Miss
                    evaluated = true;

                    // Clear hold even if they held too long
                    songManager.activeHoldNote = null;
                }
            }
        }
    }

    void Evaluate(float delta, float currentBeat, float idealBeat)
    {
        if (Mathf.Abs(currentBeat - idealBeat) < 0.2f)
            levelManager.spawnFeedback(0); // Perfect
        else if (Mathf.Abs(currentBeat - idealBeat) > 1f)
            levelManager.spawnFeedback(1); // Miss
        else if (currentBeat < idealBeat)
            levelManager.spawnFeedback(2); // Too Early
        else
            levelManager.spawnFeedback(3); // Too Late

        evaluated = true;
    }
}