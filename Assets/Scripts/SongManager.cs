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
    public bool started = false;

    private float nextBeatTime;
    private int beatCounter;

    public bool isPotato = true;
    public bool isCarrot = false;
    public bool isOnion = false;

    private bool spawnNote = false;

    public bool notePressedOnBeat = false;
    public int numBeats; //NEED TO CALCULATE THIS BASED ON THE SONG LENGTH RN IT IS HARDCODED

    public bool gameOver = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //calculate seconds per beat
        secondsPerBeat = 60f / bpm;
        numBeats = 96;
        for (int i = 0; i < numBeats; i++)  
        {   
            musicNoteBeats.Add(i*1);
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
                if (isPotato && musicNoteBeats[beatIndex] % 4 == 0) //spawns potato slow
                {
                    spawnNote = true;
                    Debug.Log("spawn note potato");   
                }

                if (isCarrot && musicNoteBeats[beatIndex] % 2 == 0) //spawns carrot medium
                {
                    spawnNote = true;
                    Debug.Log("spawn note carrot");
                }

                if (isOnion && musicNoteBeats[beatIndex] % 1 == 0) //spawns onion fast
                {
                    spawnNote = true;
                    Debug.Log("spawn note onion");
                }

                if(spawnNote){
                    MusicNote curr = Instantiate(note, this.transform);
                    curr.myBeat = musicNoteBeats[beatIndex];
                    curr.beatDur = 4f; 
                    curr.startingPosition = new Vector2(0f, -4f);
                    curr.endingPosition = new Vector2(0f, 4f);

                    Debug.Log("Spawning note at beat: " + musicNoteBeats[beatIndex]);

                    musicNotes.Enqueue(curr);
                    spawnNote = false;
                }
                beatIndex++;

            }

            if(beatIndex == numBeats){
                gameOver = true;
            }
            //if player pressed space and the queue is not empty dequeue the note and toggle it
            if (Input.GetKeyDown(KeyCode.Space) && musicNotes.Count > 0)
            {
                musicNotes.Dequeue().notePressed();
            }

            //Justify FOR TESTING
            // if (Input.GetKeyDown(KeyCode.Return))
            // {
            //     if(isOnion){
            //         isOnion = false;
            //         isPotato = true;
            //         Debug.Log("potato on onion off");

            //     }else{
            //         isOnion = true;
            //         isPotato = false;
            //         Debug.Log("onion on potato off");

            //     }
            // }
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

    public void StopMusic()
{
    AudioSource audioSource = GetComponent<AudioSource>();
    if (audioSource != null && audioSource.isPlaying)
    {
        audioSource.Stop();
        Debug.Log("Music stopped.");
    }

    started = false;
    gameOver = true; 
}

}
