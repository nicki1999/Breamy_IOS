using System;
using UnityEngine;
using Vuforia;

public class MyPrefabInstantiator : DefaultObserverEventHandler
{
    GameObject mMyModelObject;

    
    protected override void OnTrackingFound()
    {
        Debug.Log("Target Found");

        // Instantiate the model prefab only if it hasn't been instantiated yet
        if (mMyModelObject == null)
            InstantiatePrefab();

        base.OnTrackingFound();
    }


    void InstantiatePrefab()
    {
        GameObject cubePrefab = Resources.Load<GameObject>("Prefabs/Cube");
        if (cubePrefab != null)
        {
            Debug.Log("Target found, adding content");
            mMyModelObject = Instantiate(cubePrefab, mObserverBehaviour.transform);
            mMyModelObject.SetActive(true);
        }
        else
        {
            Debug.LogError("Cube prefab not found in Resources/Prefabs directory");
        }
    }
}