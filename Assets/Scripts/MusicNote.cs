using UnityEngine;

public class MusicNote : MonoBehaviour
{
    public Vector2 startingPosition;
    public Vector2 endingPosition;
    public float myBeat;
    public float beatDur;
    private SongManager songManager;
    private Collider2D trigger;
    private bool moving = true;
    private bool hit = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        songManager = transform.parent.GetComponent<SongManager>();
        trigger = GetComponent<Collider2D>();
        trigger.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(moving){
            //move note from start to end based on the beats
            transform.position = Vector2.Lerp(startingPosition, endingPosition, (songManager.getBeatsPosition() - myBeat) / (beatDur*2));
        }
        //if the player presses so early that they don't collide destroy it
        if(!moving && transform.position.y < -1.0f){
            Debug.Log("Miss");
            Destroy(gameObject, 0.75f);
        }
        //if the player presses so late that they don't collide destroy it
        if (!moving && transform.position.y > 1.0f)
        {
            Debug.Log("Miss");
            Destroy(gameObject, 0.75f);
        }

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if the note collides with the goal
        if (collision.gameObject.CompareTag("Goal"))
        {
            float distance = Mathf.Abs(collision.transform.position.y - transform.position.y);
            if(distance < 0.2)
            {
                Debug.Log("Perfect");
                hit = true;
            }else if(distance > 1)
            {
                Debug.Log("Miss");
                hit = false;
            }else if(transform.position.y < 0)
            {
                Debug.Log("Too Early");
                hit = true;
            }
            else
            {
                Debug.Log("Too Late");
                hit = true;
            }
            //destroy the music note after a small delay
            Destroy(gameObject, 0.75f);
        }
        
    }

    public bool hitStatus()
    {
        return hit;
    }
    public void notePressed()
    {
        trigger.enabled = true;
        moving = false;
    }
}
