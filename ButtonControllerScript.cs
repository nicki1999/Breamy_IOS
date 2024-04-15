using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using UnityEngine.UI;

public class ButtonControllerScript : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void startSwiftCodeKitController();

    //public void OnPressButton()
    //{
    //    Debug.Log("You clicked me!");
    //}




    // Reference to the button component
    private Button button;

    void Start()
    {
        // Get the Button component attached to the same GameObject
        button = GetComponent<Button>();

        // Add a listener to the button click event
        button.onClick.AddListener(LogButtonClick);
    }

    // Method to be called when the button is clicked
    void LogButtonClick()
    {
        Debug.Log("Button clicked!");
        startSwiftCodeKitController();
    }


}