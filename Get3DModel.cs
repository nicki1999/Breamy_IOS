using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using UnityEngine.UI;

public class Get3DModel : MonoBehaviour
{
    private Button button;

    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(LogButtonClick);
    }

    void LogButtonClick()
    {
        string dataSetPath = Application.persistentDataPath + "/MyObject.usdz";
        System.Console.WriteLine($"Data set path: {dataSetPath}");

        if (!System.IO.File.Exists(dataSetPath))
        {
            System.Console.WriteLine("Dataset file does not exist at the specified path.");
        }
        else
        {
            System.Console.WriteLine("MyObject.usdz exists in the specified path");
        }
    }

    void Update()
    {

    }
}
