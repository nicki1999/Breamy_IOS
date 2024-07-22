// The shaders have different properties and _ShadowTex is responsible for holding the image on the Light Projector
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ChangeProjectorTexture : MonoBehaviour
{
    // Reference to the projector component
    //private Projector projector;

    // The new texture to use
    //public Texture newTexture;

    //void Start()
    //{
    //    // Get the projector component
    //    projector = GetComponent<Projector>();


    //}

    public void ChangeTexture(string textureName)
    {
        //if (projector == null)
        //{
        //    projector = GetComponent<Projector>();
        //}
       
        // else {
        //    Debug.Log("Projector is null");
        //}
        // Load the texture based on its name
        Texture newTexture = Resources.Load<Texture>(textureName);

     if (newTexture != null)
    {
        Debug.Log("Texture loaded: " + newTexture.name);
    }
    else
    {
        Debug.LogWarning("Texture not found or failed to load: " + textureName);
    }

        // Check if the projector component exists and the new texture is not null
        if (newTexture != null)
        {
            Debug.Log("I'm working!" );
            GetComponent<DecalProjector>().material.EnableKeyword("Base_Map");
            // Set the projector's material's main texture to the new texture
            GetComponent<DecalProjector>().material.SetTexture("Base_Map", newTexture);
            //projector.material.SetTexture("_BaseMap", newTexture);
            //Texture texture = projector.material.GetTexture("_BaseMap");
            Texture texture = GetComponent<DecalProjector>().material.GetTexture("Base_Map");
            Debug.Log("ChangeProjectorTexture material is:" + texture.name);
            // Create a new GameObject to hold the projector as a child

        }
    }
    // position: x: 0.42, y: 0.22, z: 2.08
    //Rotation: x: 180, y:0, z: 180
    
}
