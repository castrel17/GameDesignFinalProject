using UnityEngine;
using UnityEngine.TerrainTools;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;
using NUnit.Framework;

public class DemoSongManager : MonoBehaviour
{
    public float bpm;
    public float noteTravelBeats = 8f;

    public HoldNoteController activeHoldNote = null;

    public GameObject holdNoteContainerPrefab;

    public TextMeshProUGUI countDown;
    public int countDownTime = 1;
    public MusicNote note;
    public AudioSource song;
    private float songPosition;
    private float beatsPosition;
    private float secondsPerBeat;
    private float songTime;

    private List<int> musicNoteBeats = new List<int>();
    private Queue<MonoBehaviour> musicNotes;
    private int beatIndex = 0;
    public bool started = false;
    private bool startMusic = false;
    private bool startedCountDown = false;
    private Animator countDownAnimator;

    public bool isPotato = false;
    public bool isCarrot = false;
    public bool isOnion = false;

    private bool spawnNote = false;

    public bool notePressedOnBeat = false;
    public int numBeats;

    public bool gameOver = false;

    private DemoLevelManager manager;
    private Animator animator;

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

    public TextMeshProUGUI instructionText;
    private bool waitingForStartInput = true;

    [Header("Note Size Control")]
    public Slider noteSizeSlider;
    private const float DEFAULT_SIZE = 1f;

    private double pauseStartDspTime = 0;
    private double totalPausedDuration = 0;


    void Start()
    {
        if (instructionText != null)
        {
            startedCountDown     = true;   
            waitingForStartInput = true;
            instructionText.gameObject.SetActive(true);
        }
        else
        {
            waitingForStartInput = false;
        }

        manager = GameObject.Find("GameManager").GetComponent<DemoLevelManager>();
        animator = GameObject.Find("Goal").GetComponent<Animator>();
        countDownAnimator = GameObject.Find("Countdown").GetComponent<Animator>();

        secondsPerBeat = 60f / bpm;
        numBeats = Mathf.FloorToInt(bpm * song.clip.length / 60f);
        for (int i = 0; i < numBeats; i++)
        {
            musicNoteBeats.Add(i * 1);
        }

        for (int i = 1; i <= 96; i += 1)
        {
            spawnBeats.Add(i);
        }

        musicNotes = new Queue<MonoBehaviour>();

        if (metronomeToggleButton != null)
        {
            metronomeToggleButton.onClick.AddListener(ToggleMetronome);
        }

        song.loop = true;

        if (noteSizeSlider != null)
        {
            UpdateAllNotesVisualScale(noteSizeSlider.value);
            noteSizeSlider.onValueChanged.AddListener(UpdateAllNotesVisualScale);
        }
    }

    void Update()
    {
        if (waitingForStartInput)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                instructionText.gameObject.SetActive(false);
                waitingForStartInput = false;

                StartCoroutine(CountDownToStart());
            }
            return;
        }

        if (!started && !startedCountDown)
        {
            StartCoroutine(CountDownToStart());
            startedCountDown = true;
        }

        if (started)
        {
            if (startMusic)
            {
                song.Play();
                startMusic = false;
                StopAllCoroutines();
            }

            // songPosition = (float)(AudioSettings.dspTime - songTime);
            // beatsPosition = songPosition / secondsPerBeat;
            double effectiveDspTime = AudioSettings.dspTime - totalPausedDuration;
            songPosition = (float)(effectiveDspTime - songTime);
            beatsPosition = songPosition / secondsPerBeat;

            if (isOnion) { spawnInterval = 1; maxNotes = 11; }
            else if (isCarrot) { spawnInterval = 2; maxNotes = 4; }
            else if (isPotato) { spawnInterval = 4; maxNotes = 5; }

            if (beatIndex < musicNoteBeats.Count && musicNoteBeats[beatIndex] < beatsPosition)
            {

                int beatToSpawn = musicNoteBeats[beatIndex];
                if (beatToSpawn % spawnInterval == 0 && notesSpawned < maxNotes)
                {
                    MusicNote curr = null;

                    if (isPotato)
                    {
                        if (notesSpawned < 3)
                        {
                            var container = Instantiate(holdNoteContainerPrefab, transform);
                            foreach (var vs in container.GetComponentsInChildren<VisualScaler>())
                            {
                                vs.scaleFactor = noteSizeSlider != null
                                    ? noteSizeSlider.value
                                    : DEFAULT_SIZE;
                                vs.ApplyScale();
                            }
                            var controller = container.GetComponent<HoldNoteController>();

                            float startBeat = musicNoteBeats[beatIndex] + noteTravelBeats;
                            float endBeat = startBeat + 2f;

                            controller.Initialize(startBeat, endBeat, new Vector2(0f, -4f), new Vector2(0f, 4f), this);
                            musicNotes.Enqueue(controller.startNote);

                            Debug.Log("hold note spawned");
                        }
                        else
                        {
                            curr = Instantiate(note, transform);
                            var vs = curr.GetComponent<VisualScaler>();
                            if (vs != null)
                            {
                                vs.scaleFactor = noteSizeSlider != null
                                    ? noteSizeSlider.value
                                    : DEFAULT_SIZE;
                                vs.ApplyScale();
                            }
                            curr.myBeat = musicNoteBeats[beatIndex] + noteTravelBeats;
                            curr.startingPosition = new Vector2(0f, -4f);
                            curr.endingPosition = new Vector2(0f, 4f);
                            musicNotes.Enqueue(curr);
                        }
                    }
                    else
                    {
                        curr = Instantiate(note, transform);
                        var vs = curr.GetComponent<VisualScaler>();
                        if (vs != null)
                        {
                            vs.scaleFactor = noteSizeSlider != null
                                ? noteSizeSlider.value
                                : DEFAULT_SIZE;
                            vs.ApplyScale();
                        }
                        curr.myBeat = musicNoteBeats[beatIndex] + noteTravelBeats;
                        curr.startingPosition = new Vector2(0f, -4f);
                        curr.endingPosition = new Vector2(0f, 4f);
                        musicNotes.Enqueue(curr);
                    }

                    notesSpawned++;

                    if (loopCount >= 3 && curr != null && manager.whatLevel() != 1)
                    {
                        var sr = curr.GetComponent<SpriteRenderer>();
                        if (sr != null) sr.enabled = false;
                        else if (curr.GetComponent<MeshRenderer>() != null)
                            curr.GetComponent<MeshRenderer>().enabled = false;
                    }
                }
                

                beatIndex++;
                if (metronomeOn) metronome.Play();
                animator.SetTrigger("Next");
            }

            if (beatsPosition >= numBeats)
            {
                songTime += song.clip.length;
                loopCount++;
                loopStarted = true;
                if (loopCount >= 1) StopMusic();

                beatIndex = 0;
                notesSpawned = 0;
                //clear out old music note queue
                while (musicNotes.Count > 0)
                {
                    var oldNote = musicNotes.Dequeue();
                    Destroy(oldNote.gameObject);
                }
                musicNotes = new Queue<MonoBehaviour>();
            }

           if (Input.GetKeyDown(KeyCode.Space) && musicNotes.Count > 0)
            {
                var note = musicNotes.Peek(); // peek instead of dequeue
                if(note == null)
                {
                    Debug.Log("WHY");
                }
                if (note != null)
                {
                    Debug.Log("pressed");
                    note.SendMessage("notePressed", SendMessageOptions.DontRequireReceiver);
                    musicNotes.Dequeue(); // remove only after pressing
                }
            }
        }
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

    private void UpdateAllNotesVisualScale(float scale)
    {
        float s = Mathf.Clamp(scale, 0.1f, 2f);
        foreach (var vs in FindObjectsOfType<VisualScaler>())
        {
            vs.scaleFactor = s;
            vs.ApplyScale();
        }
    }

    IEnumerator CountDownToStart()
    {
        while (countDownTime > 0)
        {
            // countDown.text = countDownTime.ToString();
            yield return new WaitForSeconds(1f);
            countDownAnimator.SetTrigger("Next");
            countDownTime--;
        }

       // countDown.text = "Start!";
        yield return new WaitForSeconds(1f);
        countDownAnimator.SetTrigger("Next");
        countDown.gameObject.SetActive(false);

        started = true;
        songTime = (float)AudioSettings.dspTime;
        startMusic = true;
        loopStarted = true;
        musicNotes = new Queue<MonoBehaviour>();
    }

    public bool startStatus()
    {
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

    public void ResetNoteCounter()
    {
        notesSpawned = 0;
    }

    public void PauseMusic()
    {
        pauseStartDspTime = AudioSettings.dspTime;
        song.Pause();
    }

    public void UnpauseMusic()
    {
        totalPausedDuration += AudioSettings.dspTime - pauseStartDspTime;
        song.UnPause();
    }
}