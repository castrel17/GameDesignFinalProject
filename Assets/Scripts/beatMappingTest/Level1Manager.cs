using Melanchall.DryWetMidi;
using Melanchall.DryWetMidi.Interaction;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Linq;
using System.Collections.Generic;

public class Level1Manager : MonoBehaviour
{
    public GameObject potatoPrefab;
    public GameObject carrotPrefab;
    public GameObject onionPrefab;

    public float slideSpeed = 5f;
    private Vector3 targetPosition = new Vector3(0f, -3.25f, 0f);
    private Vector3 offScreenPosition = new Vector3(-10f, -3.25f, 0f);

    public GameObject currentVegetable;
    private bool isSliding = false;

    public BeatMappingManager songManager;
    public bool needVeg = true;

    public TextMeshProUGUI tutorialText;

    public GameObject TooEarly;
    public GameObject TooLate;
    public GameObject Perfect;
    public GameObject Miss;
    public GameObject bonusStreak;
    public GameObject feedback;
    public GameObject bonus;

    public ScoreBar scoreBar;
    public GoalNote goalNote;
    private int streak;

    public TextMeshProUGUI scoreText;
    private int score = 0;
    Vector3 centerPos = new Vector3(0f, 2f, 0f);
    Vector3 centerPosDown = new Vector3(0f, 2f, 0f);

    public Slider streakSlider;

    private List<Melanchall.DryWetMidi.Interaction.Note> notes;

    public int level;

    void Start()
    {
        notes = songManager.GetNotesFromMidi();
    }

    void Update()
    {
        if (!songManager.gameOver && songManager.loopStarted)
        {
            if (songManager.startStatus() && needVeg)
            {
                needVeg = false;
                spawnNew();
                songManager.ResetNoteCounter();
            }

            if (isSliding && currentVegetable != null)
            {
                currentVegetable.transform.position = Vector3.MoveTowards(
                    currentVegetable.transform.position,
                    targetPosition,
                    slideSpeed * Time.deltaTime
                );

                if (CheckVegetableProgress())
                {
                    isSliding = false;
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.Tab))
        {
            SceneManager.LoadScene(0);
        }
    }

    public void spawnNew()
    {
        var notesForCarrot = notes.Where(note => note.NoteName == Melanchall.DryWetMidi.MusicTheory.NoteName.C4).ToList();
        var notesForPotato = notes.Where(note => note.NoteName == Melanchall.DryWetMidi.MusicTheory.NoteName.DSharp4).ToList();
        var notesForOnion = notes.Where(note => note.NoteName == Melanchall.DryWetMidi.MusicTheory.NoteName.F4).ToList();

        if (notesForCarrot.Count > 0)
        {
            SpawnCarrot(notesForCarrot);
            songManager.isCarrot = true;
            songManager.isPotato = false;
            songManager.isOnion = false;
        }
        else if (notesForPotato.Count > 0)
        {
            SpawnPotato(notesForPotato);
            songManager.isCarrot = false;
            songManager.isPotato = true;
            songManager.isOnion = false;
        }
        else if (notesForOnion.Count > 0)
        {
            SpawnOnion(notesForOnion);
            songManager.isCarrot = false;
            songManager.isPotato = false;
            songManager.isOnion = true;
        }
    }

    void SpawnCarrot(List<Melanchall.DryWetMidi.Interaction.Note> notesForCarrot)
    {
        currentVegetable = Instantiate(carrotPrefab, offScreenPosition, Quaternion.identity);
        isSliding = true;
        AssignChopActions(notesForCarrot);
    }

    void SpawnPotato(List<Melanchall.DryWetMidi.Interaction.Note> notesForPotato)
    {
        currentVegetable = Instantiate(potatoPrefab, offScreenPosition, Quaternion.identity);
        isSliding = true;
        AssignChopActions(notesForPotato);
    }

    void SpawnOnion(List<Melanchall.DryWetMidi.Interaction.Note> notesForOnion)
    {
        currentVegetable = Instantiate(onionPrefab, offScreenPosition, Quaternion.identity);
        isSliding = true;
        AssignChopActions(notesForOnion);
    }

    void AssignChopActions(List<Melanchall.DryWetMidi.Interaction.Note> notesForVegetable)
    {
        foreach (var note in notesForVegetable)
        {
            Debug.Log($"Chop action for note: {note.NoteName}");
        }
    }

    bool CheckVegetableProgress()
    {
        if (currentVegetable == null) return false;

        var peeler = currentVegetable.GetComponent<VegetablePeeler>();
        var cutter = currentVegetable.GetComponent<VegetableCutting>();

        if (peeler != null && !peeler.IsFullyPeeled()) return false;
        if (cutter != null && !cutter.allCut) return false;

        return true;
    }
}
