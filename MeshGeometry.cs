using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGeometry : MonoBehaviour
{
    // Start is called before the first frame update
        void Start()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter != null)
        {
            Mesh mesh = meshFilter.mesh;
            Vector3[] vertices = mesh.vertices;
            foreach (Vector3 vertex in vertices)
            {
                Debug.Log($"Vertex: X:{vertex.x}, Y:{vertex.y}, Z:{vertex.z}");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
