using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using HuggingFace.API;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine.Networking;
using System.Collections;

public class SpeechRecognitionTest : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button stopButton;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private TextMeshProUGUI responseText;

    [SerializeField]
    private string apiToken = "";

    private const string API_URL = "https://api-inference.huggingface.co/models/google/gemma-2-2b-it";

    private AudioClip clip;
    private byte[] bytes;
    private bool recording;

    private void Start()
    {
        startButton.onClick.AddListener(StartRecording);
        stopButton.onClick.AddListener(StopRecording);
        stopButton.interactable = false;
    }

    private void Update()
    {
        if (recording && Microphone.GetPosition(null) >= clip.samples)
        {
            StopRecording();
        }
    }

    private void StartRecording()
    {
        text.text = "Recording...";
        startButton.interactable = false;
        stopButton.interactable = true;
        clip = Microphone.Start(null, false, 10, 44100);
        recording = true;
    }

    private void StopRecording()
    {
        var position = Microphone.GetPosition(null);
        Microphone.End(null);
        var samples = new float[position * clip.channels];
        clip.GetData(samples, 0);
        bytes = EncodeAsWAV(samples, clip.frequency, clip.channels);
        recording = false;
        SendRecording();
    }

    private void SendRecording()
    {
        text.text = "Sending...";
        stopButton.interactable = false;
        HuggingFaceAPI.AutomaticSpeechRecognition(bytes, response => {
            text.color = Color.white;
            text.text = response;
            startButton.interactable = true;
            StartCoroutine(MakeRequest(response));
        }, error => {
            text.color = Color.red;
            text.text = error;
            startButton.interactable = true;
        });
    }

    private byte[] EncodeAsWAV(float[] samples, int frequency, int channels)
    {
        using (var memoryStream = new MemoryStream(44 + samples.Length * 2))
        {
            using (var writer = new BinaryWriter(memoryStream))
            {
                writer.Write("RIFF".ToCharArray());
                writer.Write(36 + samples.Length * 2);
                writer.Write("WAVE".ToCharArray());
                writer.Write("fmt ".ToCharArray());
                writer.Write(16);
                writer.Write((ushort)1);
                writer.Write((ushort)channels);
                writer.Write(frequency);
                writer.Write(frequency * channels * 2);
                writer.Write((ushort)(channels * 2));
                writer.Write((ushort)16);
                writer.Write("data".ToCharArray());
                writer.Write(samples.Length * 2);

                foreach (var sample in samples)
                {
                    writer.Write((short)(sample * short.MaxValue));
                }
            }
            return memoryStream.ToArray();
        }
    }

    [System.Serializable]
    private class RequestData
    {
        public string inputs;
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