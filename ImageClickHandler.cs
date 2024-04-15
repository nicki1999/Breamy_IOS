using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Vuforia;

public class ImageClickHandler : MonoBehaviour
{
    // Reference to the button in the GUI
    public Button yourButton;

    // Name of the scene to load (set this in the Inspector)
    public string sceneToLoad = "";

    // Target orientation (set this in the Inspector, e.g., LandscapeLeft)
    public ScreenOrientation targetOrientation = ScreenOrientation.LandscapeLeft;


    void Start()
    {
        // Attach the function to be called when the button is clicked
        yourButton.onClick.AddListener(TaskOnClick);
    }

    // This is the method that will be called when the button is clicked
    void TaskOnClick()
    {
        // Unload all previous scenes except the current one
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);

            // Skip the current scene
            if (scene.name != sceneToLoad)
            {
                SceneManager.UnloadSceneAsync(scene.name);
            }
        }

        // Load the specified scene with LoadSceneMode.Single
        SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Single);

        // Change device orientation (if supported on the platform)
        // SetDeviceOrientation(targetOrientation);
    }


    //  void SetDeviceOrientation(ScreenOrientation orientation)
    // {
    //#if UNITY_ANDROID || UNITY_IOS
    //  Screen.orientation = orientation;
    //#endif
    //}
}
