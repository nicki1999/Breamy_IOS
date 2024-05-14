using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class VuforiaAnnuler : MonoBehaviour
{
    public int sceneID;

    void Start()
    {
        Button button = GetComponent<Button>();
        button.onClick.AddListener(() => OnButtonClick(sceneID));

        // Check if the button is active when the scene is loaded
        if (gameObject.activeSelf)
        {
            Debug.Log("Exit vuforia button is active.");
        }
        else
        {
            Debug.Log("Exit vuforia button is inactive.");
        }
    }

    public void OnButtonClick(int sceneID)
    {
        Debug.Log("Button Clicked");
        SceneManager.LoadScene(sceneID);

        // Implement your button click functionality here
    }
}
