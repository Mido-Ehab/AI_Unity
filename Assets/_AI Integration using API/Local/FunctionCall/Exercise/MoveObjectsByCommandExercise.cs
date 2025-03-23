using System.Collections.Generic;
using System.Reflection;
using LLMUnity;
using UnityEngine;
using UnityEngine.UI;

public class MoveObjectsByCommandExercise : MonoBehaviour
{
    public LLMCharacter llmCharacter;
    public InputField playerText;
    public RectTransform blueSquare;
    public RectTransform redSquare;

    [SerializeField]float speed = 50f;
    void Start()
    {
        playerText.onSubmit.AddListener(onInputFieldSubmit);
        playerText.Select();
    }

    string[] GetFunctionNames<T>()
    {
        List<string> functionNames = new List<string>();
        foreach (var function in typeof(T).GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)) 
            functionNames.Add(function.Name);
        return functionNames.ToArray();
    }

    string ConstructDirectionPrompt(string message) //----------------------------
    {
        string directionFunctions = string.Join(", ", GetFunctionNames<DirectionFunctions>());
        string colorFunctions = string.Join(", ", GetFunctionNames<ColorFunctions>());
        
        return $@"Analyze this command: ""{message}""
        Extract the direction and color mentioned.
        
        Available directions: {directionFunctions}
        Available colors: {colorFunctions}
        
        Respond in this exact format:
        Direction: [DirectionFunction]
        Color: [ColorFunction]
        
        If no direction or color is mentioned, use NoDirectionsMentioned or NoColorMentioned respectively.";
    }

    async void onInputFieldSubmit(string message) //------------------------
    {
        /* Example prompts and test cases for students:
         * 
         * Test inputs:
         * - "move the blue square up"
         * - "move red square to the right"
         * - "make the blue square go down"
         * - "move the red square left"
         * 
         * Expected AI responses examples:
         * - Direction: "MoveUp", "MoveRight", "MoveDown", "MoveLeft", "NoDirectionsMentioned"
         * - Color: "BlueColor", "RedColor", "NoColorMentioned"
         */

        // 1. Disable the input field
        playerText.interactable = false;

        try
        {
            // 2. Get direction and color from AI
            string prompt = ConstructDirectionPrompt(message);
            string response = await llmCharacter.Chat(prompt);

            // Parse the AI response
            string[] lines = response.Split('\n');
            string directionMethod = lines[0].Replace("Direction: ", "").Trim();
            string colorMethod = lines[1].Replace("Color: ", "").Trim();

            //  Convert AI responses to  Vector3 and Color using reflection
            if (directionMethod != "NoDirectionsMentioned" && colorMethod != "NoColorMentioned")
            {
                Color color = (Color)typeof(ColorFunctions).GetMethod(colorMethod).Invoke(null, null);
                Vector3 direction = (Vector3)typeof(DirectionFunctions).GetMethod(directionMethod).Invoke(null, null);

                //  Move the square 
                RectTransform objectToMove = GetObjectByColor(color);
                if (objectToMove != null)
                {
                    objectToMove.anchoredPosition += (Vector2)direction * speed; 
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error processing command: {e.Message}");
        }
        finally
        {
            // 5. Re-enable the input field
            playerText.interactable = true;
            playerText.text = "";
            playerText.Select();
        }
    }

    private RectTransform GetObjectByColor(Color color)
    {
        if (color == Color.blue)
            return blueSquare;
        else if (color == Color.red)
            return redSquare;
        
        return null;
    }

    public void CancelRequests()
    {
        llmCharacter.CancelRequests();
    }

    public void ExitGame()
    {
        Debug.Log("Exit button clicked");
        Application.Quit();
    }

    bool onValidateWarning = true;
    void OnValidate()
    {
        if (onValidateWarning && !llmCharacter.remote && llmCharacter.llm != null && llmCharacter.llm.model == "")
        {
            Debug.LogWarning($"Please select a model in the {llmCharacter.llm.gameObject.name} GameObject!");
            onValidateWarning = false;
        }
    }
} 