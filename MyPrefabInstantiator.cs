using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Dummiesman;
using System.IO;
using UnityEditor;

public class MyPrefabInstantiator : DefaultObserverEventHandler
{
    GameObject mMyModelObject;
    public string prefabPath = "Prefabs/MyObject38"; // Path to your prefab under Assets folder

    //ChangeProjectorTexture changeProjectorTexture;
    //private Projector projector;
    //string reconstructedModel3DPath = Application.persistentDataPath + "/MyObject38.obj";



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
        //GameObject cubePrefab = Resources.Load<GameObject>("Prefabs/Cube");
        // This is the address to the .obj file in IOS & unity editor
        var reconstructedModel3DPath = Application.persistentDataPath + "/MyObject38.obj";
        if (System.IO.File.Exists(reconstructedModel3DPath))
        {
            // Define the target path in the Assets/Prefabs directory
            string targetPath = "Assets/Resources/Prefabs/MyObject38.obj"; // Adjust the filename if needed

            // Create the target directory if it doesn't exist
            string targetDirectory = System.IO.Path.GetDirectoryName(targetPath);
            if (!System.IO.Directory.Exists(targetDirectory))
            {
                System.IO.Directory.CreateDirectory(targetDirectory);
            }

            if (System.IO.File.Exists(targetPath))
            {
                // Delete the existing file
                FileUtil.DeleteFileOrDirectory(targetPath);
                Debug.Log("Existing file deleted at: " + targetPath);
            }
            // Use Unity's FileUtil to copy the file to the Prefabs folder
            FileUtil.CopyFileOrDirectory(reconstructedModel3DPath, targetPath);

            // Refresh the Asset Database so Unity recognizes the new file
            AssetDatabase.Refresh();

            Debug.Log("Object saved to Prefabs folder: " + targetPath);
        }
        else
        {
            Debug.LogError("File not found: " + reconstructedModel3DPath);
        }



        Debug.Log("Attempting to load from PersistentAssetsPath: " + reconstructedModel3DPath);
        GameObject cubePrefab = Resources.Load<GameObject>(prefabPath);
        if (cubePrefab != null)
        {
            // Instantiate the prefab
            GameObject obj = Instantiate(cubePrefab);

            // Optionally, save the instantiated object reference to a variable
            GameObject instantiatedObject = obj;

            // You can then use instantiatedObject for further manipulation or use
        }
        else
        {
            Debug.LogError("CubePrefab not found at path: " + prefabPath);
        }

        if (File.Exists(reconstructedModel3DPath))
        {
            // GameObject cubePrefab = new OBJLoader().Load(reconstructedModel3DPath);



            Transform projector = cubePrefab.transform.Find("Decal Projector");
            if (projector != null)
            {
                DecalProjector projectorComponent = projector.GetComponent<DecalProjector>();
                Texture texture = projectorComponent.material.GetTexture("Base_Map");
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
        else
        {
            Debug.LogError("File not found: " + reconstructedModel3DPath);
        }



    }
}
