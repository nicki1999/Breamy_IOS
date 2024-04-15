using UnityEngine;
using UnityEngine.UI;

public class CanvasSwitcher : MonoBehaviour
{
    public Canvas canvas1;
    public Canvas canvas2;

    void Start()
    {
        // Disable Canvas 2 initially
        canvas2.enabled = false;

        // Find the button component in children of Canvas 1
        Button button = canvas1.GetComponentInChildren<Button>();

        // Add a listener to the button
        button.onClick.AddListener(SwitchCanvas);
    }

    void SwitchCanvas()
    {
        // Toggle the canvases
        canvas1.enabled = false;
        canvas2.enabled = true;
    }
}
