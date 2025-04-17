using UnityEngine;


public class HoldNote : MonoBehaviour
{
   private DemoLevelManager levelManager;
   private DemoSongManager songManager;
   private Collider2D trigger;
   private bool inZone = false;
   private bool heldPerfect = false;


   public float holdSeconds;
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


       // Convert beat duration to seconds based on BPM
       float secondsPerBeat = 60f / songManager.bpm;
       holdSeconds = beatDur * secondsPerBeat;
   }


   void Update()
   {
       if (!inZone)
       {
           float t = (songManager.getBeatsPosition() - myBeat) / (beatDur * 2);
           transform.position = Vector2.Lerp(startingPosition, endingPosition, t);


           if (transform.position.y >= endingPosition.y)
           {
               levelManager.spawnFeedback(1); // 1 = Miss
               Destroy(gameObject);
           }
       }
       else
       {
           if (Input.GetKey(KeyCode.Space))
           {
               holdTimer += Time.deltaTime;


               if (holdTimer >= holdSeconds && !heldPerfect)
               {
                   heldPerfect = true;
                   VegetablePeeler peeler = levelManager.currentVegetable.GetComponent<VegetablePeeler>();
                   if (peeler != null)
                   {
                       peeler.SendMessage("PeelOneSection", SendMessageOptions.DontRequireReceiver);
                   }
                   levelManager.spawnFeedback(0);  // 0 = Perfect
                   Destroy(gameObject);
               }
           }
       }
   }


   public void OnTriggerEnter2D(Collider2D collision)
   {
       if (collision.gameObject.CompareTag("Goal"))
       {
           inZone = true;
           trigger.enabled = true;
       }
   }


   public void OnTriggerExit2D(Collider2D collision)
   {
       if (collision.gameObject.CompareTag("Goal"))
       {
           if (!heldPerfect)
               levelManager.spawnFeedback(1);
           Destroy(gameObject);
       }
   }
}
