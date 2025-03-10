using UnityEngine;

public class GameManager : MonoBehaviour
{
    public AudioSource theMusic;

    public bool startPlaying;

    public textChange tc;
    void Start()
    {
        
    }

    void Update()
    {
        if(!startPlaying){
            if(Input.anyKeyDown){
                startPlaying = true;
                tc.hasStarted = true;
                theMusic.Play();
            }
        }
        
    }
}
