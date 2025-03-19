using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    public AudioSource audioSource; // Assign this in the Inspector
    public AudioClip[] soundClips; // Assign your 3 sounds here

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
        audioSource.PlayOneShot(randomClip);
    }
}