using System.Collections.Generic;
using UnityEngine;
using TMPro;
using LLMUnity;

public class AI_actions : MonoBehaviour
{
    public LLMCharacter llmCharacter;
    public TMP_InputField playerText;
    public GameObject Wolf;
    public GameObject Wolf2;

    void Start()
    {
        playerText.onSubmit.AddListener(onInputFieldSubmit);
        playerText.Select();
    }

    string ConstructActionPrompt(string message)
    {
        string actions = "Shoot, Shield";
        string colors = "BlueColor, RedColor";

        return $@"Analyze this command: ""{message}""
        Extract the action and color mentioned.

        Available actions: {actions}
        Available colors: {colors}

        Respond in this exact format:
        Action: [ActionName]
        Color: [ColorName]

        If no action or color is mentioned, use NoActionMentioned or NoColorMentioned respectively.";
    }

    async void onInputFieldSubmit(string message)
    {
        playerText.interactable = false;

        try
        {
            string prompt = ConstructActionPrompt(message);
            string response = await llmCharacter.Chat(prompt);

            string[] lines = response.Split('\n');
            string action = lines[0].Replace("Action: ", "").Trim();
            string color = lines[1].Replace("Color: ", "").Trim();

            GameObject selectedCharacter = GetCharacterByColor(color);
            GameObject opponent = (selectedCharacter == Wolf) ? Wolf2 : Wolf;

            if (selectedCharacter != null)
            {
                Animator animator = selectedCharacter.GetComponent<Animator>();
                EnemyAI enemyAI = selectedCharacter.GetComponent<EnemyAI>(); // Get EnemyAI script

                if (animator != null && enemyAI != null)
                {
                    selectedCharacter.transform.LookAt(opponent.transform); // Look at opponent

                    if (action == "Shoot")
                    {
                        animator.SetTrigger("Shoot");
                        enemyAI.Shoot(); // Call Shoot from EnemyAI
                    }
                    else if (action == "Shield")
                    {
                        enemyAI.CreateShield(); // Call CreateShield from EnemyAI
                    }
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error processing command: {e.Message}");
        }
        finally
        {
            playerText.interactable = true;
            playerText.text = "";
            playerText.Select();
        }
    }

    private GameObject GetCharacterByColor(string color)
    {
        if (color == "BlueColor")
            return Wolf;
        else if (color == "RedColor")
            return Wolf2;
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

    // NEW METHOD TO HANDLE SPEECH INPUT
    public void ProcessSpeechInput(string message)
    {
        onInputFieldSubmit(message); // Calls the same function as text input
    }
}

