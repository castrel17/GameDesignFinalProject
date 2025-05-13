using UnityEngine;
public class MusicNote : MonoBehaviour
{
    public Vector2 startingPosition;
    public Vector2 endingPosition;
    public float myBeat;
    public float travelTime; 

    private DemoLevelManager manager;
    private DemoSongManager songManager;
    private bool moving = true;
    private bool markedForDelete = false;


    void Start()
    {
        manager = GameObject.Find("GameManager").GetComponent<DemoLevelManager>();
        songManager = transform.parent.GetComponent<DemoSongManager>();
    }

    void Update()
    {
        if (moving)
        {
            float currentBeat = songManager.getBeatsPosition();
            float travelBeats = songManager.noteTravelBeats;

            float t = 1 - ((myBeat - currentBeat) / travelBeats);
            t = Mathf.Clamp01(t);
            transform.position = Vector2.Lerp(startingPosition, endingPosition, t);
            

            float missCutoffBeat = myBeat - travelBeats * 0.1f; 
            if (currentBeat > missCutoffBeat)
            {
                outOfBoundsSlicing();
                songManager.dequeueNote();
                manager.spawnFeedback(1); // Miss
                Destroy(gameObject);
            }
        }

        if (!moving && !markedForDelete)
        {
            if (transform.position.y < -1.0f || transform.position.y > 1.0f)
            {
                markedForDelete = true;
                outOfBoundsSlicing();
                manager.spawnFeedback(1); // Miss
                Destroy(gameObject, 0.75f);
            }
        }
    }

    public void notePressed()
    {
        moving = false;
        JudgeNote();
    }

    private void JudgeNote()
    {
        float currentBeat = songManager.getBeatsPosition();
        float travelBeats = songManager.noteTravelBeats;
        float idealHitBeat = myBeat - (travelBeats / 2f);  // player should hit when note is halfway through
        float beatDelta = Mathf.Abs(currentBeat - idealHitBeat);
        Debug.Log("ideal beat " + idealHitBeat);
        Debug.Log("beat at which the note was hit " + currentBeat);
        if (beatDelta < 0.2f)
        {
            manager.currentVegetable.GetComponent<VegetableCutting>().slice();
            manager.spawnFeedback(0); // Perfect
        }
        else if (beatDelta > 1f)
        {
            manager.currentVegetable.GetComponent<VegetableCutting>().slice();
            manager.spawnFeedback(1); // Miss
        }
        else if (currentBeat < idealHitBeat)
        {
            manager.currentVegetable.GetComponent<VegetableCutting>().slice();
            manager.spawnFeedback(2); // Too Early
        }
        else
        {
            manager.currentVegetable.GetComponent<VegetableCutting>().slice();
            manager.spawnFeedback(3); // Too Late
        }

        markedForDelete = true;
        Destroy(gameObject, 0.75f); // Destroy the note after handling the hit
    }

    private void outOfBoundsSlicing()
    {
        var cutting = manager.currentVegetable.GetComponent<VegetableCutting>();
        var peeler = manager.currentVegetable.GetComponent<VegetablePeeler>();

        if (cutting.vegetableType == VegetableCutting.Vegetables.Potato &&
            peeler != null && !peeler.IsFullyPeeled())
        {
            peeler.PeelOneSection();
        }
        else
        {
            cutting.slice();
        }
    }

    public void SetTravelTime(float time)
    {
        travelTime = time;
    }
}
