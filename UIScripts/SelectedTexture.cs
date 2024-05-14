using UnityEngine;
using UnityEngine.UI;

public class ButtonTextureDisplay : MonoBehaviour
{
    // Reference to the button component
    private Button button;
    public ChangeProjectorTexture projectorTextureChanger;

    private void Start()
    {
        // Get the Button component attached to this GameObject
        button = GetComponent<Button>();

        // Add a listener to the button's onClick event
        button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        // Display the name of the GameObject (which is assumed to be the texture name) in the console
        Debug.Log("Selected Texture: " + gameObject.name);
        projectorTextureChanger.ChangeTexture(gameObject.name);
    }
}
