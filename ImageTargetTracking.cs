using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ImageTargetTracking : MonoBehaviour
{
    public ARTrackedImageManager trackedImageManager;

    void Start()
    {
        // Get the ARTrackedImageManager component
        trackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var trackedImage in eventArgs.added)
        {
            if (trackedImage.trackingState == TrackingState.Tracking)
            {
                Debug.Log("Image Target is being tracked: " + trackedImage.referenceImage.name);
                // Perform actions or logic when the target is being tracked
            }
        }

        foreach (var trackedImage in eventArgs.updated)
        {
            if (trackedImage.trackingState == TrackingState.Tracking)
            {
                Debug.Log("Image Target is being tracked: " + trackedImage.referenceImage.name);
                // Perform actions or logic when the target is being tracked
            }
        }
    }
}
