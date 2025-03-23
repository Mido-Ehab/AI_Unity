// Required Unity and external libraries
using Newtonsoft.Json.Linq;          // For JSON parsing functionality
using UnityEngine;                   // Core Unity functionality
using TMPro;                         // TextMeshPro for better text rendering
                 
// MonoBehaviour class to handle JSON data display in Unity
public class JasonPicker : MonoBehaviour
{
    // Reference to the UI text component that will display our JSON content
    // [SerializeField] allows us to assign this in the Unity Inspector
    [SerializeField] private TextMeshProUGUI _jsonViewText;

    // Reference to our JSON file
    // [SerializeField] allows us to assign this in the Unity Inspector
    [SerializeField] private UnityEngine.Object _jsonFile;

    // Method to display raw JSON content
    public void DisplayJson()
    {
        // Simply converts the JSON file to string and displays it
    
        _jsonViewText.text = _jsonFile.ToString();
    }

    // Method to display specific JSON data (firstName from user object)
    public void DisplayJsonView()
    {
        // Parse the JSON string into a JObject for easy navigation
        JObject jsonObject = JObject.Parse(_jsonFile.ToString());
        
        // Extract the firstName from the user object in the JSON
        // The path is: root -> user -> firstName
        string firstName = jsonObject["user"]["firstName"].ToString();
        
        // Display the extracted firstName in the UI text component
        _jsonViewText.text = firstName;
    }

    // Unity's built-in method that runs when the script instance is being loaded
    void Start()
    {
        // Currently empty, but could be used for initialization
    }

    // Unity's built-in method that runs every frame
    void Update()
    {
        // Currently empty, but could be used for continuous updates
    }
}
