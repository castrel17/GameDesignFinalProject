using UnityEngine;
using UnityEngine.TerrainTools;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI; 
using Unity.VisualScripting;

public class DemoSongManager : MonoBehaviour
{
    public float bpm;
    public TextMeshProUGUI countDown;
    public int countDownTime = 3;
    public MusicNote note;
    public HoldNote holdNote;
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
    public int numBeats; 

    public bool gameOver = false;

    private DemoLevelManager manager;
    public int baseValue = 0;
    public int vegIndex = 0;
    private Animator animator;
    private float timeSinceTrigger = 0;

    public AudioSource metronome;
    public bool metronomeOn = true;
    public Button metronomeToggleButton;
    public TextMeshProUGUI metronomeButtonText;



    public List<int> spawnBeats = new List<int>(); 
    private int spawnInterval = 1; 
    private int notesSpawned = 0;
    private int maxNotes = 0;
    private int loopCount = 0;
    public bool loopStarted = false;

    public GameObject goal; 
    private Vector3 originalScale; 
    public float scaleMultiplier = 1.5f; 


    void Start()
    {
        manager = GameObject.Find("GameManager").GetComponent<DemoLevelManager>();
        animator = GameObject.Find("Goal").GetComponent<Animator>();
        //calculate seconds per beat
        secondsPerBeat = 60f / bpm;
      //  Debug.Log(secondsPerBeat);
        numBeats = Mathf.FloorToInt(bpm * song.clip.length / 60f);
        for (int i = 0; i < numBeats; i++)  
        {   
            musicNoteBeats.Add(i*1);
        }   

        for (int i = 1; i <= 96; i += 1)
        {
            spawnBeats.Add(i);
        }
        musicNotes = new Queue<MusicNote>();
   //     Debug.Log("num beats: "+numBeats);


        if (metronomeToggleButton != null)
        {
            metronomeToggleButton.onClick.AddListener(ToggleMetronome);
        }
        song.loop = true;
    }

    // Update is called once per frame
    void Update()
    {
        // start the song when player presses down on mouse
        if (!started && !startedCountDown) {
            Debug.Log("game started");
            StartCoroutine(CountDownToStart());
            startedCountDown = true;
        }

        if (started)
        {
            // start the song and end the coroutine
            if (startMusic)
            {
                Debug.Log("PLAYING MUSIC");
                song.Play();
                startMusic = false;
                StopAllCoroutines();
            }

            songPosition = (float)(AudioSettings.dspTime - songTime);
            beatsPosition = songPosition / secondsPerBeat;

            if      (isOnion)  { spawnInterval = 1; maxNotes = 12; }
            else if (isCarrot) { spawnInterval = 2; maxNotes = 4;  }
            else if (isPotato){ spawnInterval = 4; maxNotes = 5;  }

            if (beatIndex < musicNoteBeats.Count && musicNoteBeats[beatIndex] < beatsPosition)
            {
                if (setBaseBool)
                {
                    setBaseBool = false;
                    setBase();
                }

                int currentBeat = musicNoteBeats[beatIndex] + (loopCount * numBeats);

                if (currentBeat % spawnInterval == 0 && notesSpawned < maxNotes)
                {
                    MusicNote curr = null;

                    if (isPotato)
                    {
                        if (notesSpawned < 3)
                        {
                            // spawn a HoldNote
                            var h = Instantiate(holdNote, transform);
                            h.myBeat           = musicNoteBeats[beatIndex];
                            h.beatDur          = 4;
                            h.startingPosition = new Vector2(0f, -4f);
                            h.endingPosition   = new Vector2(0f,  4f);
                        }
                        else
                        {
                            // spawn the last 2 MusicNotes
                            curr = Instantiate(note, transform);
                            curr.myBeat           = musicNoteBeats[beatIndex];
                            curr.beatDur          = 4;
                            curr.startingPosition = new Vector2(0f, -4f);
                            curr.endingPosition   = new Vector2(0f,  4f);
                            musicNotes.Enqueue(curr);
                        }
                    }

                    else
                    {
                        // onion & carrot unchanged
                        curr = Instantiate(note, transform);
                        curr.myBeat           = musicNoteBeats[beatIndex];
                        curr.beatDur          = isOnion ? 2 : 4;
                        curr.startingPosition = new Vector2(0f, -4f);
                        curr.endingPosition   = new Vector2(0f,  4f);
                        musicNotes.Enqueue(curr);
                    }

                    notesSpawned++;

                    // hide notes after demo loops
                    if (loopCount >= 3 && curr != null)
                    {
                        var sr = curr.GetComponent<SpriteRenderer>();
                        if (sr != null) sr.enabled = false;
                        else if (curr.GetComponent<MeshRenderer>() != null)
                            curr.GetComponent<MeshRenderer>().enabled = false;
                    }
                }

                beatIndex++;
                if (metronomeOn)
                {
                    metronome.Play();
                }

                animator.SetTrigger("Next");
            }

            // detect when song loops
            if (beatsPosition >= numBeats)
            {
                songTime      += song.clip.length;
                loopCount++;
                loopStarted    = true;
                if (loopCount >= 3) StopMusic();

                beatIndex      = 0;
                notesSpawned   = 0;
                musicNotes     = new Queue<MusicNote>();
                timeSinceTrigger = (beatsPosition < numBeats) 
                    ? beatsPosition 
                    : timeSinceTrigger;
            }

            // dequeue on player input
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
        notesSpawned = 0;
    }
    public void dequeueNote()
    {
        if (musicNotes.Count > 0)
            musicNotes.Dequeue();
        
    }

    public void ToggleMetronome()
    {
        metronomeOn = !metronomeOn;
        Debug.Log("Metronome is now: " + (metronomeOn ? "ON" : "OFF"));

        if (metronomeButtonText != null)
        {
            metronomeButtonText.text = "Metronome: " + (metronomeOn ? "ON" : "OFF");
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
        startMusic = true;
        loopStarted = true; 
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
