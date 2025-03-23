using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using TMPro;
using System.Text;
using Newtonsoft.Json;
using UnityEngine.UI;
using System;
using System.Linq;

public class MyAPI : MonoBehaviour
{
    // Your Hugging Face API token field in inspector
    [SerializeField] 
    [Tooltip("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")]
    private string apiToken = "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";  // Leave empty by default for security

    // Optional: Add a field to input token through UI
    [SerializeField] private TMP_InputField tokenInputField;
    
    // Reference to UI elements
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TextMeshProUGUI outputText;
    [SerializeField] private Button submitButton;

    // API endpoint for the Gemma model
    private const string API_URL = "https://api-inference.huggingface.co/models/google/gemma-2-2b-it";

    [System.Serializable]
    private class RequestData
    {
        public string inputs;
    }

    void Start()
    {
        // Add listener to the submit button if assigned
        if (submitButton != null)
        {
            submitButton.onClick.AddListener(SendRequest);
        }

        // If token input field exists, add listener to update token
        if (tokenInputField != null)
        {
            tokenInputField.onEndEdit.AddListener((value) => {
                apiToken = value;
                PlayerPrefs.SetString("HuggingFaceToken", value);
                PlayerPrefs.Save();
            });

            // Load saved token if exists
            if (PlayerPrefs.HasKey("HuggingFaceToken"))
            {
                apiToken = PlayerPrefs.GetString("HuggingFaceToken");
                tokenInputField.text = apiToken;
            }
        }
    }

    public void SendRequest()
    {
        // Validate token first
        if (string.IsNullOrEmpty(apiToken))
        {
            outputText.text = "Please enter your Hugging Face API token first!";
            Debug.LogWarning("API token is missing!");
            return;
        }

        if (string.IsNullOrEmpty(inputField.text))
        {
            Debug.LogWarning("Input text is empty!");
            return;
        }

        StartCoroutine(MakeRequest(inputField.text));
    }

    private IEnumerator MakeRequest(string inputText)
    {
        // Show loading state
        outputText.text = "Loading...";
        
        RequestData requestData = new RequestData
        {
            inputs = inputText
        };

        string jsonData = JsonConvert.SerializeObject(requestData);

        using (UnityWebRequest request = new UnityWebRequest(API_URL, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            
            // Set headers with your token
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", $"Bearer {apiToken}");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string response = request.downloadHandler.text;
                outputText.text = response;
                Debug.Log($"API Response: {response}");
            }
            else
            {
                string errorMessage = request.error;
                // Check if it's an authentication error
                if (request.responseCode == 401)
                {
                    errorMessage = "Invalid API token. Please check your Hugging Face token.";
                }
                outputText.text = $"Error: {errorMessage}";
                Debug.LogError($"Error: {errorMessage}");
            }
        }
    }
 
}