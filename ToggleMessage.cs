using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ToggleMessage : MonoBehaviour
{
    [SerializeField] private Toggle toggle;
    [SerializeField] private TMP_Text messageText;

    // This method is called when the toggle value changes
    public void OnToggleValueChanged(bool isOn)
    {
        if (isOn)
        {
            messageText.text = "Toggle is ON!";
        }
        else
        {
            messageText.text = "Toggle is OFF!";
        }
    }
}
