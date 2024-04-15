using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Threading.Tasks;
using System.Net.Http;
using System;
using System.Text;
using System.IO.Compression;
using System.IO;



public class Example : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(GetTokenAndCreateCredentials());
    }

    private IEnumerator GetTokenAndCreateCredentials()
    {
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
    }


    private bool tokenObtained = false;
    private bool assignScopes = false;
    private bool getUUID = false;
    private string token = "";
    private string datasetUUID = "";

    public IEnumerator GetToken()
    {
        string tokenUrl = "https://vws.vuforia.com/oauth2/token";
        using (UnityWebRequest www = UnityWebRequest.Post(tokenUrl, "{ \"grant_type\": \"password\"," +
            "\"username\": \"n7227009@gmail.com\"," +
            " \"password\": \"Geraltofrivai38*\"}",
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
                Debug.Log($"Form upload complete! wth token: {token}");
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
        // This is the address to the .obj file in IOS
        string reconstructedModel3DPath = Application.persistentDataPath + "/MyObject38.obj";
        System.Console.WriteLine($"Dataset path: {reconstructedModel3DPath}");

        if (!File.Exists(reconstructedModel3DPath))
        {
            System.Console.WriteLine("MyObject38.obj file does not exist at the specified path.");
        }
        else
        {
            System.Console.WriteLine("MyObject38.obj exists in the specified path");

            // Read the file contents
            byte[] modelData = File.ReadAllBytes(reconstructedModel3DPath);

            // Convert to Base64
            string base64ModelData = Convert.ToBase64String(modelData);

            // Now you can use base64ModelData in your JSON request body or wherever else you need it
            System.Console.WriteLine($"Base64-encoded model data: {base64ModelData}");



            string createModelTargetURL = "https://vws.vuforia.com/modeltargets/datasets";

            string requestBody = @"
{
    ""name"": ""mm_frame"",
    ""targetSdk"": ""10.18"",
    ""models"": [
        {
            ""name"": ""Breast_Model"",
            ""cadDataBlob"":""" + base64ModelData + @""",
            ""cadDataFormat"": ""OBJ"",

            ""views"": [
                {
                    ""name"": ""GuideView_0000"",
                    ""layout"": ""landscape"",
                    ""guideViewPosition"": {
                        ""translation"": [0.02,
                                            0.662,
                                            2.049],
                        ""rotation"": [          0,
                                                    0,
                                                       0,
                                                    1],
                        ""up"": [ 
                            0,
                            1,
                            0
                            ],
                            ""target"": [
                            0.2523028850555429,
                            1.039047672449232,
                            -0.0005546708659074362
                            ]
                    }
                },
                {
                    ""name"": ""GuideView_0001"",
                    ""layout"": ""landscape"",
                    ""guideViewPosition"": {
                        ""translation"": [-5.356123842284347,
                                            1.2814830944394011,
                                            9.479337351684363],
                        ""rotation"": [         -0.010612913421172264,
                                                -0.2639337355024507,
                                                -0.002904271253626666,
                                                0.9644780529078505],
                        ""up"": [
                            0,
                            1,
                            0
                            ],
                            ""target"": [
                            0.2523028850555429,
                            1.039047672449232,
                            -0.0005546708659074362
                            ]
                    }
                },
                {
                    ""name"": ""GuideView_0002"",
                    ""layout"": ""landscape"",
                    ""guideViewPosition"": {
                        ""translation"": [0.644289361241055,
                                            1.6215400757430447,
                                            5.950781918751998],
                        ""rotation"": [         -0.048632234280087464,
                                                0.03284026929590051,
                                                0.0015998547085566619,
                                                0.998275444437253],
                        ""up"": [
                            0,
                            1,
                            0
                            ],
                            ""target"": [
                            0.2523028850555429,
                            1.039047672449232,
                            -0.0005546708659074362
                            ]
                    }
                },
                {
                    ""name"": ""GuideView_0003"",
                    ""layout"": ""landscape"",
                    ""guideViewPosition"": {
                        ""translation"": [          1.0525598327714882,
                                                    2.2282305168773258,
                                                    12.14935034724472],
                        ""rotation"": [         -0.048632234280087464,
                                                0.03284026929590051,
                                                0.0015998547085566619,
                                                0.998275444437253],
                        ""up"": [
                            0,
                            1,
                            0
                            ],
                            ""target"": [
                            0.2523028850555429,
                            1.039047672449232,
                            -0.0005546708659074362
                            ]
                    }
                }
            ]
        }
    ],
    ""uniformScale"": 1
}";
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