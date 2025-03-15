using UnityEngine;
using UnityEngine.TerrainTools;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;

public class SongManager : MonoBehaviour
{
    public float bpm;
    public MusicNote note;
    private float songPosition;
    private float beatsPosition;
    private float secondsPerBeat;
    private float songTime;
    private float[] musicNoteBeats = { 0 , 3, 7, 11 }; // hit every four beats for testing
    private Queue<MusicNote> musicNotes;
    private int beatIndex = 0;
    private bool started = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //calculate seconds per beat
        secondsPerBeat = 60f / bpm;
        
    }

    // Update is called once per frame
    void Update()
    {
        //start the song when player presses down on mouse
        if (!started && Input.GetMouseButtonDown(0)) {
            started = true;
            songTime = (float)AudioSettings.dspTime;
            GetComponent<AudioSource>().Play();
            musicNotes = new Queue<MusicNote>();
        }
        if (started)
        {
            songPosition = (float)(AudioSettings.dspTime - songTime);
            beatsPosition = songPosition / secondsPerBeat;
            //check if it's time to hit
            if (beatIndex < musicNoteBeats.Length && musicNoteBeats[beatIndex] < beatsPosition)
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
    public float getBeatsPosition()
    {
        return beatsPosition;
    }
}
