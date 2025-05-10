using UnityEngine;
using UnityEngine.SceneManagement;

public class Level2Select : MonoBehaviour
{
    public void OnMouseDown()
    {
        Debug.Log("Click");
        SceneManager.LoadScene("Level2");
    }
}
