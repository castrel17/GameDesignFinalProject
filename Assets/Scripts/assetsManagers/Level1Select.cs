using UnityEngine;
using UnityEngine.SceneManagement;

public class Level1Select : MonoBehaviour
{
    public void OnMouseDown()
    {
        Debug.Log("Click");
        SceneManager.LoadScene("Level1");
    }
}
