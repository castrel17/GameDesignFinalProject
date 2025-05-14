using UnityEngine;
//this is for the PCG sounds
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    public AudioSource audioSource; // Assign this in the Inspector
    public AudioClip[] soundClips; // Assign your 3 sounds here

    public AudioClip peelSound;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayRandomSound()
    {
        int randomIndex = Random.Range(0, soundClips.Length);
        AudioClip randomClip = soundClips[randomIndex];

        Debug.Log("Playing sound: " + randomClip.name);
        audioSource.PlayOneShot(randomClip, 0.75f);
    }

    public void peelSFX()
    {
        if (peelSound != null)
        {
            Debug.Log("Playing peel sfx");
            audioSource.PlayOneShot(peelSound, 0.75f);
        }
        else
        {
            Debug.LogWarning("Peel sound not assigned.");
        }
    }
}