using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Threading.Tasks;
using System.Net.Http;
using System;
using System.Text;
using System.IO.Compression;
using System.IO;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class Example : MonoBehaviour
{
    private bool tokenObtained = false;
    private bool assignScopes = false;
    private bool getUUID = false;
    private string token = "";
    private string datasetUUID = "";
    private bool datasetExistence = false;
    //private bool isLoading = false;
    public string requestStatus = "";
    private int loadingSceneID = 2;
    private int currentSceneID = 1;
    GameObject loadSceneObject;
    Canvas loadingCanvas;
    // This is the address to the .obj file in IOS
    string reconstructedModel3DPath = Application.persistentDataPath + "/MyObject38.obj";

    void Start()
    {
         loadSceneObject = GameObject.Find("LoadingCanvas");
         loadingCanvas = loadSceneObject.GetComponent<Canvas>();
        StartCoroutine(GetTokenAndCreateCredentials());
    }


    private IEnumerator GetTokenAndCreateCredentials()
    {
        // Step 0: check if the dataset exists
        DatasetExistence();
        if (datasetExistence == false)
        {
            //isLoading = true;
            loadingCanvas.enabled = true;

            //SceneManager.LoadScene(loadingSceneID);
            // Step 1: Get Token
            yield return StartCoroutine(GetToken());

            // Check the result of GetToken before proceeding
            if (tokenObtained)
            {
                // Step 2: Create Credentials
                yield return StartCoroutine(CreateCredential(token));
            }
            Debug.Log($"assignScopes value: {assignScopes}");
            if (assignScopes)
            {
                //step 3: train the model
                yield return StartCoroutine(CreateModelTargetDataset(token));
            }
            if (getUUID)
            {
                yield return MonitorAndDownloadModelTargetDataset(token, datasetUUID);
            }
            else
            {
                Debug.LogError("One of the functions in this function did not work!");
            }
            //isLoading = false;
            loadingCanvas.enabled = false;
            //SceneManager.UnloadSceneAsync(loadingSceneID);
        }
    }




    private void DatasetExistence()
    {


        //iOS: Application.persistentDataPath points to /var/mobile/Containers/Data/Application/<guid>/Documents.
        //The following are the streamingAssets directory

        string extractPath = Application.persistentDataPath + "/ExtractedDataset38";
        string MTDataset = Application.persistentDataPath + "/ExtractedDataset38/MTDataset.dat";


        if (!File.Exists(MTDataset))
        {
            System.Console.WriteLine("NO DATASET FILE");
            datasetExistence = false;
        }
        else
        {
            Debug.Log("MTDataset.dat IS ALREADY THERE");
            datasetExistence = true;
        }

    }

    //LOGING TO VUFORIA SERVERS
    public IEnumerator GetToken()
    {
        requestStatus = "LOGING TO VUFORIA SERVERS";
        string tokenUrl = "https://vws.vuforia.com/oauth2/token";
        using (UnityWebRequest www = UnityWebRequest.Post(tokenUrl, "{ \"grant_type\": \"password\"," +
            "\"username\": \"nickinajafi3@gmail.com\"," +
            " \"password\": \"Yenneferofvengerberg388\"}",
            "application/json"))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Response: " + www.downloadHandler.text);
                token = ExtractAccessToken(www.downloadHandler.text);
                tokenObtained = !string.IsNullOrEmpty(token); // Set the flag to indicate successful token acquisition
                Debug.Log($"Form upload complete! with token: {token}");
            }
        }
    }
    private string ExtractAccessToken(string response)
    {
        try
        {
            var json = JsonUtility.FromJson<AccessTokenResponse>(response);
            return json.access_token;
        }
        catch (Exception e)
        {
            Debug.LogError("Error extracting access token: " + e.Message);
            return null;
        }
    }
    [Serializable]
    private class AccessTokenResponse
    {
        public string access_token;
        // Add other fields if needed
    }


    private string ExtractUuidFromJson(string json)
    {
        try
        {
            // Parse the JSON and extract the "uuid" value
            var jsonObject = JsonUtility.FromJson<ModelCreationResponse>(json);
            return jsonObject.uuid;
        }
        catch (Exception e)
        {
            Debug.LogError("Error extracting UUID from JSON: " + e.Message);
            return null;
        }
    }

    [Serializable]
    private class ModelCreationResponse
    {
        public string uuid;
    }

    IEnumerator CreateCredential(string jwtToken)
    {
        Debug.Log("I am running!");
        if (string.IsNullOrEmpty(jwtToken))
        {
            Debug.LogError("JWT Token is missing.");
            yield break;
        }

        string createCredentialURL = "https://vws.vuforia.com/oauth2/clientcredentials";

        // Define the scopes you want for the client credentials
        var scopes = new string[]
        {
        "modeltargets.standardmodeltarget.all",
        "modeltargets.advancedmodeltarget.all"
        };

        // Prepare the request body with scopes
        string jsonBody = JsonUtility.ToJson(new CredentialRequest { scopes = scopes });

        using (UnityWebRequest www = new UnityWebRequest(createCredentialURL, "POST"))
        {
            // Set both Authorization and Content-Type headers
            www.SetRequestHeader("Authorization", "Bearer " + jwtToken);
            www.SetRequestHeader("Content-Type", "application/json");

            // Set the request body
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
            www.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

            Debug.Log($"JWT Token: {jwtToken}");

            // Log request details
            Debug.Log($"Credential Request URL: {www.url}");
            Debug.Log($"Credential Request Method: {www.method}");
            Debug.Log($"Credential Request Headers: {www.GetRequestHeader("Authorization")}, {www.GetRequestHeader("Content-Type")}");
            Debug.Log($"Credential Request Body: {jsonBody}");

            yield return www.SendWebRequest();

            // Check for errors
            if (www.result == UnityWebRequest.Result.Success)
            {
                // Parse the responseContent as needed
                var result = www.downloadHandler.text;
                Debug.Log("Success: " + result);
                assignScopes = !string.IsNullOrEmpty(result);
            }
            else
            {
                Debug.LogError("Failed to create client credentials. Response: " + www.error + ", " + www.downloadHandler.text);
            }
        }
    }

    [Serializable]
    private class CredentialRequest
    {
        public string[] scopes;
    }

    IEnumerator CreateModelTargetDataset(string jwtToken)
    {
        if (string.IsNullOrEmpty(jwtToken))
        {
            Debug.LogError("JWT Token is missing.");
            yield break;
        }

        string[] objLines = File.ReadAllLines(reconstructedModel3DPath);


        List<Vector3> vertices = new List<Vector3>();

        foreach (string line in objLines)
        {
            string[] parts = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (parts[0] == "v") // Check for vertex data
            {
                float x = float.Parse(parts[1]);
                float y = float.Parse(parts[2]);
                float z = float.Parse(parts[3]);
                Vector3 vertex = new Vector3(x, y, z);
                vertices.Add(vertex);
            }
        }

        if (vertices.Count > 0)
        {
            // Calculate dimensions
            float minX = vertices[0].x;
            float maxX = vertices[0].x;
            float minY = vertices[0].y;
            float maxY = vertices[0].y;
            float minZ = vertices[0].z;
            float maxZ = vertices[0].z;

            foreach (Vector3 vertex in vertices)
            {
                if (vertex.x < minX) minX = vertex.x;
                if (vertex.x > maxX) maxX = vertex.x;
                if (vertex.y < minY) minY = vertex.y;
                if (vertex.y > maxY) maxY = vertex.y;
                if (vertex.z < minZ) minZ = vertex.z;
                if (vertex.z > maxZ) maxZ = vertex.z;
            }

            float objWidth = maxX - minX;
            float objHeight = maxY - minY;
            //float objDepth = maxZ - minZ;

            Debug.Log("Width: " + objWidth);
            Debug.Log("Height: " + objHeight);
            //Debug.Log("Depth: " + objDepth);

            float desiredWidth = 0.53f;
            float desiredHeight = 0.34f;



            // Calculate scale factors to achieve desired dimensions
            float scaleX = desiredWidth / objWidth;
            float scaleY = desiredHeight / objHeight;
            float scaleZ = 1f; // Maintain original depth scale

            // Calculate diagonal distance (hypotenuse) of the bounding box
            float objDiagonal = Mathf.Sqrt(objWidth * objWidth + objHeight * objHeight);

            // Calculate depth proportional to the diagonal distance
            float depthScaleFactor = 0.5f; // Adjust this factor based on visual preference
            float objDepth = objDiagonal * depthScaleFactor;


            // Calculate camera distance to fit the entire model within the frame
            float fov = Mathf.PI / 4f; // Example: 45 degrees
            float cameraDistance = objDiagonal / (2f * Mathf.Tan(fov / 2f));


            // Output for debugging or visualization
            Debug.Log("Diagonal Distance: " + objDiagonal);
            Debug.Log("Depth: " + objDepth);
            Debug.Log("cameraDistance: " + cameraDistance);





            //Debug.Log("Scaled Width: " + scaleX);
            //Debug.Log("Scaled Height: " + scaleY);
            //Debug.Log("Scaled Depth: " + scaleZ);
            // Apply scaling to vertices (in memory)
            List<Vector3> scaledVertices = new List<Vector3>();
            foreach (Vector3 vertex in vertices)
            {
                float scaledX = vertex.x * scaleX;
                float scaledY = vertex.y * scaleY;
                float scaledZ = vertex.z * scaleZ;
                scaledVertices.Add(new Vector3(scaledX, scaledY, scaledZ));
            }
            List<string> updatedObjLines = new List<string>();
            int vertexIndex = 0;
            foreach (string line in objLines)
            {
                if (line.StartsWith("v "))
                {
                    Vector3 scaledVertex = scaledVertices[vertexIndex++];
                    string updatedLine = string.Format("v {0} {1} {2}", scaledVertex.x, scaledVertex.y, scaledVertex.z);
                    updatedObjLines.Add(updatedLine);
                }
                else
                {
                    updatedObjLines.Add(line);
                }
            }

            //File.WriteAllLines(reconstructedModel3DPath, updatedObjLines.ToArray());

            // Calculate uniform scale factor (maintain aspect ratio)
            float uniformScale = Mathf.Min(scaleX, scaleY, scaleZ);


            // Calculate camera position (example)
            Vector3 cameraPosition = Camera.main.transform.position;
            // Calculate translation to center model in FOV
            Vector3 modelPositionInFOV = cameraPosition + Camera.main.transform.forward * cameraDistance;
            modelPositionInFOV.y = cameraPosition.y - (objHeight / 2); // Centering the model vertically
            Debug.Log("Transition x:" + modelPositionInFOV.x);
            Debug.Log("Transition y:" + modelPositionInFOV.y);



            // Prepare the JSON request body with uniformScale
            // z = 1.049
            string requestBody = @"
{
    ""name"": ""mm_frame"",
    ""targetSdk"": ""10.18"",
    ""models"": [
        {
            ""name"": ""Breast_Model"",
            ""cadDataBlob"":""" + Convert.ToBase64String(File.ReadAllBytes(reconstructedModel3DPath)) + @""",
            ""cadDataFormat"": ""OBJ"",
            ""views"": [
                {
                    ""name"": ""GuideView_0000"",
                    ""layout"": ""portrait"",
                    ""guideViewPosition"": {
                    ""translation"": [0, " + cameraDistance/5 + @", " + cameraDistance + @"],
                        ""rotation"": [0, 0, 0, 1],
                        ""up"": [0, 1, 0]
                    }
                }
            ]
        }
    ],
      ""uniformScale"": " + uniformScale.ToString("F3") + @"

}";

            string createModelTargetURL = "https://vws.vuforia.com/modeltargets/datasets";
            using (UnityWebRequest www = new UnityWebRequest(createModelTargetURL, "POST"))
            {
                www.SetRequestHeader("Authorization", "Bearer" + jwtToken);
                www.SetRequestHeader("Content-Type", "application/json");

                byte[] bodyRaw = Encoding.UTF8.GetBytes(requestBody);
                www.uploadHandler = new UploadHandlerRaw(bodyRaw);
                www.downloadHandler = new DownloadHandlerBuffer();
                www.SetRequestHeader("Authorization", $"Bearer {jwtToken}");
                www.SetRequestHeader("Content-Type", "application/json");

                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.Success)
                {
                    Debug.Log("Model Created" + www.downloadHandler.text);
                    datasetUUID = ExtractUuidFromJson(www.downloadHandler.text);
                    getUUID = !string.IsNullOrEmpty(datasetUUID);
                }
                else
                {
                    Debug.LogError("Failed to create model target dataset. Response: " + www.error + ", " + www.downloadHandler.text);
                }

            }

        }
    }

    private IEnumerator MonitorAndDownloadModelTargetDataset(string jwtToken, string datasetUUID)
    {
        // Base URL for the Vuforia API
        string apiUrl = "https://vws.vuforia.com/modeltargets/datasets";

        // Monitoring the creation status
        string statusUrl = $"{apiUrl}/{datasetUUID}/status";

        while (true)
        {
            using (UnityWebRequest statusRequest = UnityWebRequest.Get(statusUrl))
            {
                // Add the Authorization header with the JWT token
                statusRequest.SetRequestHeader("Authorization", $"Bearer {jwtToken}");

                yield return statusRequest.SendWebRequest();

                if (statusRequest.result == UnityWebRequest.Result.Success)
                {
                    string statusContent = statusRequest.downloadHandler.text;
                    Debug.Log($"Status Response: {statusContent}");

                    // Parse the JSON response to check the status
                    // You may want to use a JSON library or manually parse the response
                    // Here, we're just looking for a "status" field
                    if (statusContent.Contains("\"status\":\"done\""))
                    {
                        // The dataset creation is done, proceed to download
                        string downloadUrl = $"{apiUrl}/{datasetUUID}/dataset";

                        using (UnityWebRequest downloadRequest = UnityWebRequest.Get(downloadUrl))
                        {
                            // Add the Authorization header with the JWT token
                            downloadRequest.SetRequestHeader("Authorization", $"Bearer {jwtToken}");

                            yield return downloadRequest.SendWebRequest();

                            if (downloadRequest.result == UnityWebRequest.Result.Success)
                            {


                                //iOS: Application.persistentDataPath points to /var/mobile/Containers/Data/Application/<guid>/Documents.
                                //The following are the streamingAssets directory


                                string dataSetPath = "";
                                #if UNITY_ANDROID && !UNITY_EDITOR
                                                                             dataSetPath = "jar:file://" + Application.dataPath + "!/assets/dataset38.zip";
                                #elif UNITY_IOS && !UNITY_EDITOR
                                                                             dataSetPath = Application.persistentDataPath + "/dataset38.zip";
                                #else
                                                                dataSetPath = Application.streamingAssetsPath + "/dataset38.zip";
                                #endif
                                byte[] datasetBytes = downloadRequest.downloadHandler.data;
                                System.IO.File.WriteAllBytes(dataSetPath, datasetBytes);

                                Debug.Log($"Dataset downloaded and saved as '{dataSetPath}'.");

                                // Use the StreamingAssets folder to store assets. At runtime, Application.streamingAssetsPath provides the path to the folder. Add the asset name to Application.streamingAssetsPath
                                //string extractPath = Application.streamingAssetsPath + "/ExtractedDataset38";
                                string extractPath = Application.persistentDataPath + "/ExtractedDataset38";
                                string MTDataset = Application.persistentDataPath + "/ExtractedDataset38/MTDataset.dat";


                                if (!File.Exists(MTDataset))
                                {
                                    System.Console.WriteLine("Dataset file does not exist at the specified path.");
                                    Unzip(dataSetPath, extractPath);
                                    Debug.Log($"Dataset extracted to '{extractPath}'.");
                                    //isLoading = false;
                                }
                                else
                                {
                                    Debug.Log("MTDataset.dat already exists");
                                }


                            }
                            else
                            {
                                // Print the response content for error details
                                string errorContent = downloadRequest.error;
                                Debug.LogError($"Failed to download the dataset. Error Content: {errorContent}");
                            }

                            // Break out of the loop after downloading
                            yield break;
                        }
                    }

                    else if (statusContent.Contains("\"status\": \"processing\""))
                    {
                        // The dataset is still processing, check again later
                        Debug.Log("Dataset creation is still processing. Check status later.");
                    }
                    else if (statusContent.Contains("\"status\": \"failed\""))
                    {
                        // The dataset creation has failed
                        Debug.LogError("Dataset creation failed.");
                        // You can extract additional error information from the response if needed

                        // Break out of the loop in case of failure
                        yield break;
                    }
                }
                else
                {
                    Debug.LogError("Failed to check the dataset creation status. Error: " + statusRequest.error);

                    // Break out of the loop in case of failure
                    yield break;
                }
            }

            // Wait for some time before checking the status again
            yield return new WaitForSeconds(5f); // Adjust the time interval as needed
        }
    }
    // Unzip method using System.IO.Compression
    private void Unzip(string zipFilePath, string extractPath)
    {
        ZipFile.ExtractToDirectory(zipFilePath, extractPath);
    }

}