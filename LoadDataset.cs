using System.Collections.Generic;
using UnityEngine;
using Vuforia;
using System.Collections;

public class LoadDataSet : DefaultObserverEventHandler
{
    string targetName = "Breast_Model";

    protected override void Start()
    {
        UnityEngine.Debug.Log("Start method called");

        VuforiaApplication.Instance.OnVuforiaInitialized += OnVuforiaInitialized;
    }

    void OnVuforiaInitialized(VuforiaInitError error)
    {
        if (error == VuforiaInitError.NONE)
        {
            UnityEngine.Debug.Log("Vuforia Started");
            OnVuforiaStarted();
        }
        else
        {
            UnityEngine.Debug.LogError($"Vuforia initialization error: {error}");
        }
    }

    void OnVuforiaStarted()
    {
        UnityEngine.Debug.Log("Vuforia started");
        string dataSetPath = "";

#if UNITY_ANDROID && !UNITY_EDITOR
        dataSetPath = "jar:file://" + Application.dataPath + "!/assets/MTDataset.xml";
#elif UNITY_IOS && !UNITY_EDITOR
        dataSetPath = Application.persistentDataPath + "/ExtractedDataset38/MTDataset.xml";
#else
        dataSetPath = Application.streamingAssetsPath + "/MTDataset.xml";
#endif

        UnityEngine.Debug.Log($"Data set path: {dataSetPath}");

        if (!System.IO.File.Exists(dataSetPath))
        {
            UnityEngine.Debug.LogError("#LoadDataset.cs Dataset file does NOT exist at the specified path: " + dataSetPath);
            return;
        }
        else
        {
            UnityEngine.Debug.LogError("#LoadDataset.cs Dataset file DOES exist at the specified path: " + dataSetPath);
        }

        var mModelTarget = VuforiaBehaviour.Instance.ObserverFactory.CreateModelTarget(dataSetPath, targetName);

        if (mModelTarget != null)
        {
            mModelTarget.OnTargetStatusChanged += OnTargetStatusChanged;
        }
        else
        {
            UnityEngine.Debug.LogError("Failed to create Model Target.");
        }
    }

    void OnTargetStatusChanged(ObserverBehaviour behaviour, TargetStatus status)
    {
        UnityEngine.Debug.Log($"target status: {status.Status}");
    }
}
