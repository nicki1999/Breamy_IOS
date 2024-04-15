using UnityEngine;
using Vuforia;

public class EnableGuideview : MonoBehaviour
{
    void Start()
    {
        ModelTargetBehaviour modelTargetBehaviour = GetComponent<ModelTargetBehaviour>();

        if (modelTargetBehaviour != null)
        {
            modelTargetBehaviour.GuideViewMode = ModelTargetBehaviour.GuideViewDisplayMode.GuideView2D;
            Debug.Log("GuideView set to 2D");
        }
        else
        {
            Debug.LogError("ModelTargetBehaviour component not found on this GameObject.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Your update logic (if any)
    }
}
