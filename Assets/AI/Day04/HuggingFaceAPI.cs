using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using TMPro;
using System.Text;
using Newtonsoft.Json;
using UnityEngine.UI;
using System.Collections.Generic;

namespace HuggingFace
{
    public class HuggingFaceAPI : MonoBehaviour
    {
        [Header("API Configuration")]
        [SerializeField] 
        [Tooltip("Enter your Hugging Face API token here")]
        private string apiToken = "";

        [Header("UI References")]
        [SerializeField] 
        [Tooltip("Input field for the API token")]
        private TMP_InputField tokenInputField;
        
        [SerializeField] 
        [Tooltip("Input field for the text to send to the AI")]
        private TMP_InputField queryInputField;

        [SerializeField] 
        [Tooltip("Text component to display the AI's response")]
        private TextMeshProUGUI responseText;

        [SerializeField] 
        [Tooltip("Button to send the request")]
        private Button sendButton;

        private const string API_URL = "https://api-inference.huggingface.co/models/google/gemma-2-2b-it";

        [System.Serializable]
        private class RequestData
        {
            public string inputs;
        }

        [System.Serializable]
        private class ApiResponse
        {
            public List<GeneratedText> generated_text { get; set; }
        }

        [System.Serializable]
        private class GeneratedText
        {
            public string text { get; set; }
        }

        private void Start()
        {
            InitializeUI();
            LoadSavedToken();
        }

        private void InitializeUI()
        {
            if (sendButton != null)
            {
                sendButton.onClick.AddListener(SendRequest);
            }
            else
            {
                Debug.LogError("Send button is not assigned!");
            }

            if (tokenInputField != null)
            {
                tokenInputField.onEndEdit.AddListener(SaveToken);
            }
            else
            {
                Debug.LogError("Token input field is not assigned!");
            }

            if (queryInputField == null)
            {
                Debug.LogError("Query input field is not assigned!");
            }

            if (responseText == null)
            {
                Debug.LogError("Response text component is not assigned!");
            }
        }

        private void LoadSavedToken()
        {
            if (PlayerPrefs.HasKey("HuggingFaceToken"))
            {
                apiToken = PlayerPrefs.GetString("HuggingFaceToken");
                if (tokenInputField != null)
                {
                    tokenInputField.text = apiToken;
                }
            }
        }

        private void SaveToken(string value)
        {
            apiToken = value;
            PlayerPrefs.SetString("HuggingFaceToken", value);
            PlayerPrefs.Save();
        }

        public void SendRequest()
        {
            if (string.IsNullOrEmpty(apiToken))
            {
                ShowError("Please enter your Hugging Face API token first!");
                return;
            }

            if (string.IsNullOrEmpty(queryInputField.text))
            {
                ShowError("Please enter some text to send to the AI!");
                return;
            }

            StartCoroutine(MakeRequest(queryInputField.text));
        }

        private void ShowError(string message)
        {
            if (responseText != null)
            {
                responseText.text = $"Error: {message}";
                Debug.LogWarning(message);
            }
        }

        private IEnumerator MakeRequest(string inputText)
        {
            if (responseText != null)
            {
                responseText.text = "Loading...";
            }

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
                
                request.SetRequestHeader("Content-Type", "application/json");
                request.SetRequestHeader("Authorization", $"Bearer {apiToken}");

                yield return request.SendWebRequest();

                if (responseText != null)
                {
                    if (request.result == UnityWebRequest.Result.Success)
                    {
                        try
                        {
                            string rawResponse = request.downloadHandler.text;
                            Debug.Log($"Raw API Response: {rawResponse}");

                            // Try parsing as a direct text response first
                            var directResponse = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(rawResponse);
                            if (directResponse != null && directResponse.Count > 0 && directResponse[0].ContainsKey("generated_text"))
                            {
                                string generatedText = directResponse[0]["generated_text"];
                                responseText.text = generatedText;
                                Debug.Log($"Generated Text: {generatedText}");
                            }
                            else
                            {
                                // Fallback to displaying raw response if parsing fails
                                responseText.text = rawResponse;
                            }
                        }
                        catch (JsonException ex)
                        {
                            Debug.LogError($"Error parsing JSON response: {ex.Message}");
                            ShowError("Error parsing API response");
                        }
                    }
                    else
                    {
                        string errorMessage = request.error;
                        if (request.responseCode == 401)
                        {
                            errorMessage = "Invalid API token. Please check your Hugging Face token.";
                        }
                        ShowError(errorMessage);
                    }
                }
            }
        }
    }
} 