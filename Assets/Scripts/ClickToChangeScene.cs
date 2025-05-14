using UnityEngine;
using UnityEngine.SceneManagement;

public class ClickToChangeScene : MonoBehaviour
{
    public string sceneToLoad;

    void OnMouseDown()
    {
        Debug.Log("Sprite clicked. Loading scene: " + sceneToLoad);
        SceneManager.LoadScene(sceneToLoad);
    }
}