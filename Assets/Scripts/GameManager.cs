using UnityEngine;

public class GameManager : MonoBehaviour
{
    public AudioSource theMusic;
    public bool startPlaying;

    public textChange tc;
    public PCGMusic pcg;

    void Update()
    {
        if(!startPlaying)
        {
            if(Input.anyKeyDown)
            {
                startPlaying = true;
                tc.hasStarted = true;
                pcg.hasStarted = true;
                theMusic.Play();
            }
        }
    }

    public void StopEverything()
    {
        theMusic.Stop();
        startPlaying = false;

        pcg.hasStarted = false;

        tc.hasStarted = false;

        Debug.Log("Music and beat prompts have been stopped.");
    }
}