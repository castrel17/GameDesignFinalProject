using UnityEngine;
using UnityEngine.TerrainTools;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class DemoSongManager : MonoBehaviour
{
    public float bpm;
    public TextMeshProUGUI countDown;
    public int countDownTime = 3;
    public MusicNote note;
    public AudioSource song;
    private float songPosition;
    private float beatsPosition;
    private float secondsPerBeat;
    private float songTime;
  
    private List<int> musicNoteBeats = new List<int>();
    private Queue<MusicNote> musicNotes;
    private int beatIndex = 0;
    public bool started = false;
    private bool startMusic = false;
    private bool startedCountDown = false;

    public bool isPotato = false;
    public bool isCarrot = false;
    public bool isOnion = false;

    private bool spawnNote = false;
    public bool setBaseBool = false;

    public bool notePressedOnBeat = false;
    public int numBeats; //NEED TO CALCULATE THIS BASED ON THE SONG LENGTH RN IT IS HARDCODED

    public bool gameOver = false;

    private DemoLevelManager manager;
    public int baseValue = 0;
    public int vegIndex = 0;
    void Start()
    {
        manager = GameObject.Find("GameManager").GetComponent<DemoLevelManager>();
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
        if (!started && Input.anyKey && !startedCountDown) {
            Debug.Log("game started");
            StartCoroutine(CountDownToStart());
            startedCountDown = true;
        }
        if (started)
        {
            //start the song and ed the coroutine
            if (startMusic)
            {
                Debug.Log("PLAYING MUSIC");
                song.Play();
                startMusic = false;
                StopAllCoroutines();
            }
            songPosition = (float)(AudioSettings.dspTime - songTime);
            beatsPosition = songPosition / secondsPerBeat;

            if (beatIndex < musicNoteBeats.Count && musicNoteBeats[beatIndex] < beatsPosition)
            {
                if (setBaseBool)
                {
                    setBaseBool = false;
                    setBase();

                }
                if (vegIndex < manager.currentVegetable.GetComponent<VegetableCutting>().beats.Length && manager.currentVegetable.GetComponent<VegetableCutting>().beats[vegIndex] + baseValue == musicNoteBeats[beatIndex])
                {
                    spawnNote = true;
                    vegIndex++;
                }
                if(vegIndex < manager.currentVegetable.GetComponent<VegetableCutting>().beats.Length)
                {
                    Debug.Log(manager.currentVegetable.GetComponent<VegetableCutting>().vegetableType + " : current beat " + (int) (manager.currentVegetable.GetComponent<VegetableCutting>().beats[vegIndex] + baseValue));
                }
                if (spawnNote){
                    MusicNote curr = Instantiate(note, this.transform);
                    curr.myBeat = musicNoteBeats[beatIndex];
                    //assign beat duration based on the current vegetable
                    if (isPotato)
                    {
                        curr.beatDur = 8;
                    }else if (isOnion)
                    {
                        curr.beatDur = 2;
                    }else
                    {
                        curr.beatDur = 4;
                    } 
                    curr.startingPosition = new Vector2(0f, -4f);
                    curr.endingPosition = new Vector2(0f, 4f);

                    Debug.Log("Spawning note at beat: " + musicNoteBeats[beatIndex]);

                    musicNotes.Enqueue(curr);
                    spawnNote = false;
                }
                beatIndex++;
                Debug.Log("Current Note: "+ musicNoteBeats[beatIndex]);
            }

            if(beatIndex == numBeats){
                gameOver = true;
            }

            //if player pressed space and the queue is not empty dequeue the note and toggle it
            if (Input.GetKeyDown(KeyCode.Space) && musicNotes.Count > 0)
            {
                musicNotes.Dequeue().notePressed();
            }
        }
    }

    public void setBase()
    {
        baseValue = musicNoteBeats[beatIndex];
        vegIndex = 0;
    }
    public void dequeueNote()
    {
        musicNotes.Dequeue();
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
        startMusic = true;
        //GetComponent<AudioSource>().Play();
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
