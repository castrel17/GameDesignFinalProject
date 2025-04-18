using UnityEngine;

public class MusicNote : MonoBehaviour
{
    public Vector2 startingPosition;
    public Vector2 endingPosition;
    public float myBeat;
    public float beatDur;
    private DemoLevelManager manager;
    private DemoSongManager songManager;
    private Collider2D trigger;
    private bool moving = true;
    private bool markedForDelete = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        manager = GameObject.Find("GameManager").GetComponent<DemoLevelManager>();
        songManager = transform.parent.GetComponent<DemoSongManager>();
        trigger = GetComponent<Collider2D>();
        trigger.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(moving){
            //move note from start to end based on the beats
            transform.position = Vector2.Lerp(startingPosition, endingPosition, (songManager.getBeatsPosition() - myBeat) / (beatDur*2));
            //if the note goes out of bounds without the player pressing the button just delete the note
            if(transform.position.y == endingPosition.y)
            {
                outOfBoundsSlicing();
                songManager.dequeueNote();
                Destroy(gameObject);
                manager.spawnFeedback(1);
            }
        }
        //if the player presses so early that they don't collide destroy it
        if(!moving && transform.position.y < -1.0f && !markedForDelete)
        {
            markedForDelete = true;
            outOfBoundsSlicing();
            Debug.Log("Miss");
            manager.spawnFeedback(1);
            Destroy(gameObject, 0.75f);
        }
        //if the player presses so late that they don't collide destroy it
        if (!moving && transform.position.y > 1.0f && !markedForDelete)
        {
            markedForDelete = true;
            outOfBoundsSlicing();
            Debug.Log("Miss");
            manager.spawnFeedback(1);
            Destroy(gameObject, 0.75f);
        }

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if the note collides with the goal
        if (collision.gameObject.CompareTag("Goal"))
        {
            markedForDelete = true;
            float distance = Mathf.Abs(collision.transform.position.y - transform.position.y);
            if(distance < 0.2)
            {
                Debug.Log("Perfect");
                manager.currentVegetable.GetComponent<VegetableCutting>().slice();
                manager.spawnFeedback(0);

            }
            else if(distance > 1)
            {
                Debug.Log("Miss");
                manager.currentVegetable.GetComponent<VegetableCutting>().slice();
                manager.spawnFeedback(1);
            }
            else if(transform.position.y < 0)
            {
                Debug.Log("Too Early");
                manager.currentVegetable.GetComponent<VegetableCutting>().slice();
                manager.spawnFeedback(2);
            }
            else
            {
                Debug.Log("Too Late");
                manager.currentVegetable.GetComponent<VegetableCutting>().slice();
                manager.spawnFeedback(3);
            }
            //destroy the music note after a small delay
            Destroy(gameObject, 0.75f);
        }
        
    }

    private void outOfBoundsSlicing()
    {
        //if the veggie is a potato that isn't fully peeled peel a section
        if (manager.currentVegetable.GetComponent<VegetableCutting>().vegetableType == VegetableCutting.Vegetables.Potato && !manager.currentVegetable.GetComponent<VegetablePeeler>().IsFullyPeeled()) {
            manager.currentVegetable.GetComponent<VegetablePeeler>().PeelOneSection();
        }
        else //else slice
        {
            manager.currentVegetable.GetComponent<VegetableCutting>().slice();
        }
    }
    public void notePressed()
    {
        trigger.enabled = true;
        moving = false;
    }
}
