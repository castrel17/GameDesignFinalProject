using UnityEngine;

public class ToggleButton : MonoBehaviour
{
    [Header("Drag your Settings Panel here")]
    public GameObject settingsPanel;

    public void ToggleSettingsPanel()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(!settingsPanel.activeSelf);
    }

    public void CloseSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }
}