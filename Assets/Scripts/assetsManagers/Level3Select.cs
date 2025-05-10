using UnityEngine;
using UnityEngine.SceneManagement;

public class Level3Select : MonoBehaviour
{
    public void OnMouseDown()
    {
        Debug.Log("Click");
        SceneManager.LoadScene("Level3");
    }
}
