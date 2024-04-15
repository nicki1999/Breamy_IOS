using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class ChangeFlashlight : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetFlashlight(bool enabled)
    {
        VuforiaBehaviour.Instance.CameraDevice.SetFlash(enabled);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
