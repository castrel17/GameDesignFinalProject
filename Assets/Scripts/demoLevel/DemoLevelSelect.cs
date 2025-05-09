using UnityEngine;
using UnityEngine.SceneManagement;
public class DemoLevelSelect : MonoBehaviour
{
    public void OnMouseDown()
    {
        Debug.Log("Click");
        SceneManager.LoadScene("Demo");
    }

}
