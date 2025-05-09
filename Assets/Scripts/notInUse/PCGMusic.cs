using UnityEngine;

public class PCGMusic : MonoBehaviour
{
    public AudioClip kickSound;  
    public AudioClip snareSound; 
    public AudioClip hiHatSound; 
    public float bpm = 120f;    
    public float volume = 1f;   

    private float beatTime;      
    private float nextBeatTime;  

    private AudioSource audioSource;
    public bool hasStarted;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        beatTime = 60f / bpm;
        nextBeatTime = Time.time;
    }

    void Update()
    {
        if(hasStarted){
            if (Time.time >= nextBeatTime)
            {
                GenerateRhythm();
                nextBeatTime += beatTime;
            }
        }
    }

    void GenerateRhythm()
    {
        float beat = Time.time % beatTime;

        if (beat < beatTime * 0.25f) 
        {
            PlaySound(kickSound);
        }
        else if (beat < beatTime * 0.5f) 
        {
            PlaySound(snareSound);
        }
        else if (beat < beatTime * 0.75f) 
        {
            PlaySound(hiHatSound);
        }
        else 
        {
            PlaySound(hiHatSound);
        }
    }

    void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip, volume);
    }
}
