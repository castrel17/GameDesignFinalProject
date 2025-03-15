using UnityEngine;
using UnityEngine.TerrainTools;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class SongManager : MonoBehaviour
{
    public float bpm;
    public TextMeshProUGUI countDown;
    public int countDownTime = 3;
    public MusicNote note;
    private float songPosition;
    private float beatsPosition;
    private float secondsPerBeat;
    private float songTime;
   // private float[] musicNoteBeats = { 0 , 3, 7, 11 }; // hit every four beats for testing

    private List<float> musicNoteBeats = new List<float>();
    private Queue<MusicNote> musicNotes;
    private int beatIndex = 0;
    private bool started = false;

    private float nextBeatTime;
    private int beatCounter;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //calculate seconds per beat
        secondsPerBeat = 60f / bpm;

        for (int i = 0; i < 1000; i++)  
        {
            if(i!= 0)
            {
                musicNoteBeats.Add((i*4)-1);
                Debug.Log(i.ToString());
            }
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        //start the song when player presses down on mouse
        if (!started && Input.GetMouseButtonDown(0)) {
            Debug.Log("game started");
            StartCoroutine(CountDownToStart());
        }
        if (started)
        {
            songPosition = (float)(AudioSettings.dspTime - songTime);
            beatsPosition = songPosition / secondsPerBeat;

            if (beatIndex < musicNoteBeats.Count && musicNoteBeats[beatIndex] < beatsPosition)
            {
                    MusicNote curr = Instantiate(note, this.transform);
                    //set beat, starting position, and ending position for the note
                    curr.myBeat = musicNoteBeats[beatIndex];
                    curr.beatDur = 4f;
                    curr.startingPosition = new Vector2(0f, -4f);
                    curr.endingPosition = new Vector2(0f, 4f);
                    musicNotes.Enqueue(curr);
                    beatIndex++;
            }

            //if player pressed space and the queue is not empty dequeue the note and toggle it
            if (Input.GetKeyDown(KeyCode.Space) && musicNotes.Count > 0)
            {
                musicNotes.Dequeue().notePressed();
            }
        }
    }

    //countdown until music starts
    IEnumerator CountDownToStart()
    {
        while(countDownTime > 0)
        {
            countDown.text = countDownTime.ToString();
            Debug.Log(countDownTime.ToString());
            yield return new WaitForSeconds(1f);
            countDownTime--;
        }
        countDown.text = "Start!";
        yield return new WaitForSeconds(1f);
        countDown.gameObject.SetActive(false);

        //actually start
        started = true;
        songTime = (float)AudioSettings.dspTime;
        GetComponent<AudioSource>().Play();
        Debug.Log("game playable");
        musicNotes = new Queue<MusicNote>();
    }

    public bool startStatus(){
        return started;
    }

    public float getBeatsPosition()
    {
        return beatsPosition;
    }
}
