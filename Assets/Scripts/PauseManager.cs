using UnityEngine;
using UnityEngine.UI;   // ← needed for Button
// using TMPro;        // ← only if you reference TextMeshProUGUI

public class PauseManager : MonoBehaviour
{
    public Button pauseButton;
    public GameObject pauseMenuUI;
    private bool isPaused = false;
    private DemoSongManager songManager;

    void Start()
    {
        songManager = FindObjectOfType<DemoSongManager>();
        pauseButton.onClick.AddListener(TogglePause);
        pauseMenuUI.SetActive(false);
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        if (isPaused)
        {
        Time.timeScale = 0f;
        AudioListener.pause = true;
        songManager.PauseMusic();     // uses audioSource.Pause() and records dspTime
        pauseMenuUI.SetActive(true);
        }
        else
        {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        songManager.UnpauseMusic();   // uses audioSource.UnPause() and adjusts offsets
        pauseMenuUI.SetActive(false);
        }
    }
}
