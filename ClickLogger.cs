using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ClickLogger : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        GameObject clickedObject = eventData.pointerPress;

        if (clickedObject != null)
        {
            Debug.Log("Clicked on: " + clickedObject.name);
        }
        else
        {
            Debug.Log("Clicked on empty space.");
        }
    }
}
