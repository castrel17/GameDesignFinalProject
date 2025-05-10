using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    public void OnMouseDown()
    {
        Debug.Log("Click");
        SceneManager.LoadScene("LevelSelect");
    }
}
