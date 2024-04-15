using UnityEngine;
using UnityEngine.Android;

public class FrontCameraController : MonoBehaviour
{
    public Camera frontCamera;

    private void Start()
    {
        // Request camera permission
        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            Permission.RequestUserPermission(Permission.Camera);
        }

        // Ensure only the front camera is active
        if (Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            EnableFrontCamera();
        }
        else
        {
            Debug.LogWarning("Camera permission not granted.");
        }
    }

    private void EnableFrontCamera()
    {
        if (frontCamera != null)
        {
            // Disable all other cameras in the scene
            Camera[] allCameras = Camera.allCameras;
            foreach (Camera camera in allCameras)
            {
                if (camera != frontCamera)
                {
                    camera.gameObject.SetActive(false);
                }
            }

            // Enable the front camera
            frontCamera.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError("Front camera reference is missing. Assign the front camera in the Inspector.");
        }
    }
}
