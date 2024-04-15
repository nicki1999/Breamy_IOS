using UnityEngine;
using Vuforia;
public class SideLoadImageTarget : MonoBehaviour
{
    public Texture2D textureFile;
    public float printedTargetSize;
    public string targetName;
    void Start()
    {
       // Use Vuforia Application to invoke the function after Vuforia Engine is initialized
       VuforiaApplication.Instance.OnVuforiaStarted += CreateImageTargetFromSideloadedTexture;
    }
    void CreateImageTargetFromSideloadedTexture()
    {
        var mTarget = VuforiaBehaviour.Instance.ObserverFactory.CreateImageTarget(
            textureFile,
            printedTargetSize,
            targetName);
        // add the Default Observer Event Handler to the newly created game object
        mTarget.gameObject.AddComponent<DefaultObserverEventHandler>();
        Debug.Log("Instant Image Target created " + mTarget.TargetName);
    }
}