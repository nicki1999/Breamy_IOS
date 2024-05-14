using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasSwitcherUpdate : MonoBehaviour
{
    public Canvas showCanvas;
    Canvas[] allCanvases; // Declare the array

    void Start()
    {
        // Find all canvases in the scene when the object is enabled
        allCanvases = FindObjectsOfType<Canvas>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SwitchCanvasOnClick()
    {
        // Disable all canvases
        foreach (Canvas canvas in allCanvases)
        {
            canvas.enabled = false;
        }
        // Enable the desired canvas
        showCanvas.enabled = true;
    }
}
