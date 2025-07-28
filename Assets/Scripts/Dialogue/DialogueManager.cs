using System;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    private CharacterData characterData;
    private PromptGenerator promptGenerator;

    // State Variables 
    private bool smallTalkMode = true;
    private string unprocessedInput = "";
    List<ExpectedAnswer> options;
    string currentDialoguePartID = ""; 

    void Start()
    {
        LoadCharacterData("Miko");
        promptGenerator = new PromptGenerator();
        AIManager.AIInternalResponse += ProcessResponseCheckResult;
    }

    void Oestroy()
    {
        AIManager.AIInternalResponse -= ProcessResponseCheckResult;
    }

    private void LoadCharacterData(string characterName)
    {
        string json = Resources.Load<TextAsset>(characterName).text;
        characterData = JsonUtility.FromJson<CharacterData>(json);
        foreach (DialoguePart part in characterData.dialougePartsSource)
        {
            characterData.dialougeParts.Add(part.id, part);
        }
    }

    public void HandleUserInput(string input)
    {
        unprocessedInput = input;
        if (CheckConversationLength() && smallTalkMode)
        {
            EndConversation();
            return;
        }
        CheckForExpectedResponse(input);
    }
    
    private void CheckForExpectedResponse(string input)
    {
        List<string> allPossibleAnswers = new List<string>();

        // Add default answers to List
        DialoguePart temp = characterData.dialougeParts[default];
        foreach (ExpectedAnswer part in temp.allOptions)
        {
            options.Add(part);
            allPossibleAnswers.Add(part.possibleAnswer);
        }

        // Add possible answers from Last Message to List
        if (currentDialoguePartID != "")
        {
            temp = characterData.dialougeParts[default];
            foreach (ExpectedAnswer part in temp.allOptions)
            {
                options.Add(part);
                allPossibleAnswers.Add(part.possibleAnswer);
            }
        }

        string[] prompt = promptGenerator.checkingPrompt(input, allPossibleAnswers.ToArray());
        AIManager.TextRequest(prompt, true);
    }    
    
    public void ProcessResponseCheckResult(string s)
    {
        int id;
        try
        {
            id = int.Parse(s);
        }
        catch
        { 
            id = -1;
        }

        if (id > -1)
        {
            // Give pre written Answer
            string next = options[id].leadsToID;
            string npcText = characterData.dialougeParts[next].npcResponse;
            InternalDialogueResponse?.Invoke(npcText);
            SetState(next);             
        }
        else
        {
            if (!smallTalkMode && CheckConversationLength())
            {
                EndConversation();
            }
            else
            {
                GenerateSmallTalkResponse(unprocessedInput);
            }
            SetState("");            
        }
    }

    private void SetState(string id)
    {
        currentDialoguePartID = id;
        smallTalkMode = id=="";
        unprocessedInput = "";
        options = null;
    }

    private void GenerateSmallTalkResponse(string input)
    {
        string[] prompt = promptGenerator.defaultChatPrompt(input, characterData);
        AIManager.TextRequest(prompt, false);
    }

    public static event Action<string> InternalDialogueResponse;









    private bool CheckConversationLength()
    {
        return false; // Use this for endless Testing, Adjust later
    }

    private void StartConversation()
    {
        // Return one basic Start Message or Generate one Random with Prompt
    }


    private void EndConversation()
    {
        // Return one basic End Message or Generate one Random with Prompt
    }

}