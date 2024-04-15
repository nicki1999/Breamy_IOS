using UnityEngine;

public class ObjectFinder : MonoBehaviour
{
    private void Start()
    {
        // Invoke the FindObjectWithTag method after a delay of 2 seconds (you can adjust this delay)
        InvokeRepeating("FindObjectWithName", 2f, 2f);

    }

    private void FindObjectWithName()
    {
        // Find all GameObjects with the specified tag
        GameObject objectWithName = GameObject.Find("Breast_Model");

        // Check if any objects were found with the specified tag
        // Check if the GameObject was found
        if (objectWithName != null)
        {
            // Print "found" in the console if the object is found
            Debug.Log("Found: " + objectWithName.name);
            // Attach the "test.cs" script to the GameObject
            if (objectWithName.GetComponent<MyPrefabInstantiator>() == null)
            {
                // If the GameObject doesn't already have the "test.cs" script attached, add it
                objectWithName.AddComponent<MyPrefabInstantiator>();
                Debug.Log("Added 'MyPrefabInstantiator' script to object: " + objectWithName.name);

            }
            else if (objectWithName.GetComponent<EnableGuideview>() == null)
            {
                // If the GameObject doesn't already have the "test.cs" script attached, add it
                objectWithName.AddComponent<EnableGuideview>();
                Debug.Log("Added 'EnableGuideview' script to object: " + objectWithName.name);
            }
            else
            {
                Debug.Log("Object already has 'Test' script attached: " + objectWithName.name);
                CancelInvoke("FindObjectWithName");

            }
        }

        else
        {
            // If the object was not found, print a message indicating so
            Debug.Log("No object with the name 'Breast_Model' found.");
        }

    }

        
}
