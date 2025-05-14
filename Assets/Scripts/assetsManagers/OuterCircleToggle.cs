using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OuterCircleToggle : MonoBehaviour
{
    [Tooltip("The ring to show/hide")]
    public GameObject outerRing;

    [Tooltip("The UI Button component on this GameObject")]
    public Button toggleButton;

    [Tooltip("The Text or TMP text child of that button")]
    public TMP_Text buttonText;

    private bool isVisible = true;

    void Start()
    {
        isVisible = outerRing.activeSelf;
        UpdateVisuals();

        toggleButton.onClick.AddListener(ToggleOuterRing);
    }

    public void ToggleOuterRing()
    {
        isVisible = !isVisible;
        outerRing.SetActive(isVisible);
        Debug.Log($"[Toggle] outerRing now {(isVisible ? "ON" : "OFF")}");
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        if (buttonText != null)
            buttonText.text = isVisible ? "Outer Ring: ON" : "Outer Ring: OFF";

        if (toggleButton != null)
        {
            var colors = toggleButton.colors;
            colors.normalColor     = isVisible ? Color.green : Color.red;
            colors.selectedColor   = colors.normalColor;
            colors.highlightedColor= colors.normalColor;
            toggleButton.colors    = colors;
        }
    }
}
