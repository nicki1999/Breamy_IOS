using System.Collections;
using UnityEngine;

public class MyPrefabInstantiator : DefaultObserverEventHandler
{
    GameObject mMyModelObject;
    ChangeProjectorTexture changeProjectorTexture;
    private Projector projector;


    protected override void OnTrackingFound()
    {
        Debug.Log("Target Found");

        // Instantiate the model prefab only if it hasn't been instantiated yet
        if (mMyModelObject == null)
            StartCoroutine(InstantiatePrefabWithDelay());

        base.OnTrackingFound();
    }

    IEnumerator InstantiatePrefabWithDelay()
    {
        // Wait for the next frame
        yield return null;

        InstantiatePrefab();
    }

    void InstantiatePrefab()
    {
        GameObject cubePrefab = Resources.Load<GameObject>("Prefabs/Cube");
        Transform projector = cubePrefab.transform.Find("BlobLightProjector");
        if (projector != null)
        {
            Projector projectorComponent = projector.GetComponent<Projector>();
            Texture texture = projectorComponent.material.GetTexture("_ShadowTex");
            Debug.Log("InstantiatePrefab MATERIAL IS: " + texture.name);
        }
        else
        {
            Debug.LogError("PROJECTOR IS NULL");
        }
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
