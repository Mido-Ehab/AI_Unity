using System.Collections.Generic;
using UnityEngine;
using TMPro;
using LLMUnity;

public class AI_actions : MonoBehaviour
{
    public LLMCharacter llmCharacter;
    public TMP_InputField playerText;
    public GameObject player; // Assign Player GameObject
    public GameObject target1; // Assign potential Target 1
    public GameObject target2; // Assign potential Target 2
    private GameObject currentTarget; // Holds the selected target

    void Start()
    {
        playerText.onSubmit.AddListener(onInputFieldSubmit);
        playerText.Select();
        currentTarget = target2; // Default target
    }

    string ConstructActionPrompt(string message)
    {
        string actions = "Shoot, Shield";
        string targets = "Enemy1, Enemy2"; // Define possible targets

        return $@"You are an AI assistant that extracts commands from user input.
    
    User command: ""{message}""

    Your task:
    - Identify the action from this list: {actions}
    - Identify the target from this list: {targets}

    If the command does not mention an action, respond with:
    Action: NoActionMentioned

    If the command does not mention a target, respond with:
    Target: NoTargetMentioned

    Always respond in this exact format:
    Action: [ActionName]
    Target: [TargetName]
    
    Example:
    User: ""Use shield now!""
    Response:
    Action: Shield
    Target: NoTargetMentioned";
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
            string target = lines[1].Replace("Target: ", "").Trim();

            currentTarget = GetTargetByName(target);

            if (player != null && currentTarget != null)
            {
                PlayerAI playerAI = player.GetComponent<PlayerAI>();

                if (playerAI != null)
                {
                    playerAI.SetTarget(currentTarget); // Ensure player knows which target to attack

                    if (action == "Shoot")
                    {
                        playerAI.Shoot(); // Player shoots at the selected target
                    }
                    else if (action == "Shield")
                    {
                        playerAI.CreateShield(); // Player activates shield
                    }
                    else
                    {
                        Debug.Log("No valid action detected.");
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

    private GameObject GetTargetByName(string targetName)
    {
        if (targetName == "Enemy1")
            return target1;
        else if (targetName == "Enemy2")
            return target2;

        Debug.Log("No valid target detected, using default.");
        return target1; // Default to target1 if no match
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
