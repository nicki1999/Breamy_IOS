using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackArrow : MonoBehaviour
{
    // Start is called before the first frame update
    public Canvas source;
    public Canvas target;

    void Start()
    {
        Button backButton = FindBackButtonInChildren(transform, "BackArrow");
        if (backButton != null)
        {
            backButton.onClick.AddListener(SwitchCanvas);
        }
        else
        {
            Debug.LogError("Button with the name 'BackArrow' not found in the canvas hierarchy.");
        }
    }

    void SwitchCanvas()
    {
        source.enabled = false;
        target.enabled = true;
    }
    Button FindBackButtonInChildren(Transform parent, string buttonName)
    {
        Button button = parent.GetComponentInChildren<Button>();

        if (button != null && button.name == buttonName)
        {
            return button;
        }

        // Iterate through children recursively
        for (int i = 0; i < parent.childCount; i++)
        {
            button = FindBackButtonInChildren(parent.GetChild(i), buttonName);

            if (button != null)
            {
                return button;
            }
        }

        return null;
    }
}
