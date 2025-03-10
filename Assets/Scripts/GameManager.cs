using UnityEngine;

public class GameManager : MonoBehaviour
{
    public AudioSource theMusic;

    public bool startPlaying;

    public textChange tc;
    public PCGMusic pcg;
    void Start()
    {
        
    }

    void Update()
    {
        if(!startPlaying){
            if(Input.anyKeyDown){
                startPlaying = true;
                tc.hasStarted = true;
                pcg.hasStarted = true;
                theMusic.Play();
            }
        }
        
    }
}
