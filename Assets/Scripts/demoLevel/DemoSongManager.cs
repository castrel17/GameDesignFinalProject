using UnityEngine;
using UnityEngine.TerrainTools;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;
using NUnit.Framework;
using UnityEngine.SceneManagement;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using System.IO;
using UnityEngine.Networking; 


public class DemoSongManager : MonoBehaviour
{
    public float bpm;
    public float noteTravelBeats { get; set; } = 8f;
    public HoldNoteController activeHoldNote { get; set; }
    public GameObject holdNoteContainerPrefab;

    public TextMeshProUGUI countDown;
    public int countDownTime = 1;
    public MusicNote note;
    public AudioSource song;
    private float songPosition;
    private float beatsPosition;
    private float secondsPerBeat;
    private float songTime;

    private List<double> musicNoteBeats = new List<double>();
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
    
    [Header("Vegetable Types (Demo & Level 1 Only)")]
    private Animator animator;

    public AudioSource metronome;
    public bool metronomeOn = true;
    public Button metronomeToggleButton;
    public TextMeshProUGUI metronomeButtonText;

    public List<int> spawnBeats = new List<int>();
    private int spawnInterval = 1;
    private int notesSpawned = 100;
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
    
    [Header("Level Information")]
    public int level;
    public string midiFileName = "level2.mid"; 
    public bool useMidiFile = false;
    
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

        animator = GameObject.Find("Goal").GetComponent<Animator>();
        countDownAnimator = GameObject.Find("Countdown").GetComponent<Animator>();
        manager = GameObject.Find("GameManager").GetComponent<DemoLevelManager>();
        secondsPerBeat = 60f / bpm;
        
        // Determine whether to use MIDI or hardcoded beats based on level
        SetupBeatsForLevel();

        maxNotes = 10000;
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

    private void SetupBeatsForLevel()
    {
        musicNoteBeats.Clear();
        spawnBeats.Clear();

        if (level <= 1)
        {
            SetupHardcodedBeats();
        }
        else
        {
    #if UNITY_WEBGL && !UNITY_EDITOR
            StartCoroutine(LoadMidiFileWebGL());
    #else
            LoadMidiFileLocal();
    #endif
        }
    }


    private void SetupHardcodedBeats()
    {
        numBeats = Mathf.FloorToInt(bpm * song.clip.length / 60f);
        
        for (int i = 0; i < numBeats; i++)
        {
            musicNoteBeats.Add(i * 1);
        }
        
        for (int i = 1; i <= 96; i += 1)
        {
            spawnBeats.Add(i);
        }
        
        Debug.Log($"Setup hardcoded beats. Total beats: {numBeats}");
    }

    void Update()
    {
        if (waitingForStartInput)
        {
            if (Input.GetMouseButtonDown(0))
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

            double effectiveDspTime = AudioSettings.dspTime - totalPausedDuration;
            songPosition = (float)(effectiveDspTime - songTime);
            beatsPosition = songPosition / secondsPerBeat;
            // For demo and level 1, handle vegetable-specific logic
            if (level <= 1)
            {
                if (isOnion) { spawnInterval = 1; maxNotes = 11; }
                else if (isCarrot) { spawnInterval = 2; maxNotes = 4; }
                else if (isPotato) { spawnInterval = 4; maxNotes = 5; }
            }

            // Different note spawning logic for different levels
            if (level <= 1)
            {
                // Original vegetable-specific logic for demo and level 1
                if (beatIndex < musicNoteBeats.Count && musicNoteBeats[beatIndex] < beatsPosition && !manager.needVeg && !manager.getAllCut())
                {
                    int beatToSpawn = (int)musicNoteBeats[beatIndex];
                    if (beatToSpawn % spawnInterval == 0 && notesSpawned < maxNotes)
                    {
                        Debug.Log("note spawned value " + notesSpawned);
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

                                float startBeat = (float)musicNoteBeats[beatIndex] + noteTravelBeats;
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
                                curr.myBeat = (float)musicNoteBeats[beatIndex] + noteTravelBeats;
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
                            curr.myBeat = (float)musicNoteBeats[beatIndex] + noteTravelBeats;
                            curr.startingPosition = new Vector2(0f, -4f);
                            curr.endingPosition = new Vector2(0f, 4f);
                            musicNotes.Enqueue(curr);
                        }

                        notesSpawned++;
                    }
                    
                    beatIndex++;
                    if (metronomeOn) metronome.Play();
                    animator.SetTrigger("Next");
                }
            }
            else
            {
                float spawnThreshold = 4.0f;
                while (beatIndex < musicNoteBeats.Count && musicNoteBeats[beatIndex] <= beatsPosition + spawnThreshold)
                {
                    int beatToSpawn = (int)musicNoteBeats[beatIndex];
                    if (beatToSpawn % spawnInterval == 0 && notesSpawned < maxNotes)
                    {
                        MusicNote curr = null;
                        
                        curr = Instantiate(note, transform);
                        var vs = curr.GetComponent<VisualScaler>();
                        if (vs != null)
                        {
                            vs.scaleFactor = noteSizeSlider != null
                                ? noteSizeSlider.value
                                : DEFAULT_SIZE;
                            vs.ApplyScale();
                        }

                        curr.myBeat = (float)musicNoteBeats[beatIndex] + spawnThreshold;// + noteTravelBeats;
                        curr.startingPosition = new Vector2(0f, -4f);
                        curr.endingPosition = new Vector2(0f, 3.8f); //changed to match speeds better
                        musicNotes.Enqueue(curr);
                        
                        notesSpawned++;
                    }
                    
                    beatIndex++;
                    if (metronomeOn) metronome.Play();
                    animator.SetTrigger("Next");
                }
            }

            if (beatsPosition >= numBeats)
            {
                songTime += song.clip.length;
                loopCount++;
                loopStarted = true;
                if (loopCount >= 1) {
                    if (level == 0)
                        SceneManager.LoadScene("EndDemo");
                    else if (level == 1)
                        SceneManager.LoadScene("EndScene");
                    else if (level == 2 || level == 3)
                        SceneManager.LoadScene("EndScene");
                        
                    StopMusic();
                    return;
                }

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
                var note = musicNotes.Peek(); 
                if(note == null)
                {
                    Debug.Log("WHY");
                }
                if (note != null)
                {
                    Debug.Log("pressed");
                    note.SendMessage("notePressed", SendMessageOptions.DontRequireReceiver);
                    musicNotes.Dequeue(); 
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
            yield return new WaitForSeconds(1f);
            countDownAnimator.SetTrigger("Next");
            countDownTime--;
        }
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

    IEnumerator LoadMidiFileWebGL()
    {
        string midiPath = Path.Combine(Application.streamingAssetsPath, midiFileName);

        UnityWebRequest www = UnityWebRequest.Get(midiPath);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("WebGL: Failed to load MIDI file: " + www.error);
            SetupHardcodedBeats();
            yield break;
        }

        byte[] midiData = www.downloadHandler.data;

        try
        {
            using (var stream = new MemoryStream(midiData))
            {
                ProcessMidiFile(MidiFile.Read(stream));
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("WebGL: Failed to read MIDI: " + e.Message);
            SetupHardcodedBeats();
        }
    }

    void LoadMidiFileLocal()
    {
        string midiPath = Path.Combine(Application.streamingAssetsPath, midiFileName);

        if (!File.Exists(midiPath))
        {
            Debug.LogError("Local: MIDI file not found: " + midiPath);
            SetupHardcodedBeats();
            return;
        }

        try
        {
            var midiFile = MidiFile.Read(midiPath);
            ProcessMidiFile(midiFile);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Local: Failed to read MIDI file: " + e.Message);
            SetupHardcodedBeats();
        }
    }

    void ProcessMidiFile(MidiFile midiFile)
    {
        var tempoMap = midiFile.GetTempoMap();
        var notes = midiFile.GetNotes();

        Debug.Log($"MIDI file loaded. Total notes found: {notes.Count}");

        musicNoteBeats.Clear();
        foreach (var note in notes)
        {
            musicNoteBeats.Add(ConvertNoteToBeat(note, tempoMap));
        }

        musicNoteBeats.Sort();
        if (musicNoteBeats.Count > 0)
        {
            numBeats = Mathf.CeilToInt((float)musicNoteBeats[^1]) + 1;

            for (int i = 0; i < musicNoteBeats.Count; i++)
                spawnBeats.Add(i);
        }
        else
        {
            Debug.LogWarning("No usable notes, falling back to hardcoded beats.");
            SetupHardcodedBeats();
        }
    }





    float ConvertNoteToBeat(Melanchall.DryWetMidi.Interaction.Note note, TempoMap tempoMap)
    {
        var metricTime = TimeConverter.ConvertTo<MetricTimeSpan>(note.Time, tempoMap);
        double timeInSeconds = metricTime.TotalMicroseconds / 1_000_000.0;
        Debug.Log("Note beat " + (float)(timeInSeconds / secondsPerBeat));
        return (float)(timeInSeconds / secondsPerBeat);
    }

    public List<double> GetMusicNoteBeats()
    {
        return musicNoteBeats;
    }
    public int getNumBeats(){return musicNoteBeats.Count;}
}