using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    private CharacterData characterData;
    private PromptGenerator promptGenerator;

    // State Variables 
    private bool smallTalkMode = true;
    public string unprocessedInput = "";
    List<ExpectedAnswer> options;
    string currentDialoguePartID = "";

    void Start()
    {
        GameManager.instance.InitiatingConversation += LoadCharacterData;
        promptGenerator = new PromptGenerator();
        AIManager.AIInternalResponse += ProcessResponseCheckResult;
    }

    void OnDestroy()
    {
        GameManager.instance.InitiatingConversation -= LoadCharacterData;
        AIManager.AIInternalResponse -= ProcessResponseCheckResult;
    }

    private void LoadCharacterData(string characterName)
    {
        string json = Resources.Load<TextAsset>(characterName).text;
        characterData = JsonUtility.FromJson<CharacterData>(json);
        characterData.dialougeParts = new Dictionary<string, DialogueNode>();
        foreach (DialogueNode part in characterData.dialougePartsSource)
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
        options = new List<ExpectedAnswer>();

        // Add default answers to List
        DialogueNode temp = characterData.dialougeParts["default"];
        foreach (ExpectedAnswer node in temp.allOptions)
        {
            if (node.conditionsRequired != null)
            {
                if (GameManager.instance.progression.CheckCondtions(node.conditionsRequired))
                {
                    continue;
                }
            }
            options.Add(node);
            foreach (string answer in node.possibleAnswer)
            {

                allPossibleAnswers.Add(answer);
            }            
        }

        // Add possible answers from Last Message to List
        if (currentDialoguePartID != "")
        {
            temp = characterData.dialougeParts[currentDialoguePartID];
            foreach (ExpectedAnswer node in temp.allOptions)
            {
                options.Add(node);
                foreach (string answer in node.possibleAnswer)
                {
                    allPossibleAnswers.Add(answer);
                }  
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
            GeneratePrewrittenResponse(id);  
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

    private void GeneratePrewrittenResponse(int id)
    {
        // Find the selected option
        int count = 0;
        ExpectedAnswer selected = options[0];
        foreach (ExpectedAnswer option in options)
        {
            count += option.possibleAnswer.Length;
            if (id < count)
            {
                selected = option;
                break;
            }
        }
        // Process the selected Answer
        string next = selected.leadsToID;
        string npcText = selected.npcResponse[Random.Range(0, selected.npcResponse.Length)];
        InternalDialogueResponse?.Invoke(npcText);
        SetState(next);
        // Check for unlocks
        if (selected.conditionsUnlocked != null)
        {
            foreach (string unlock in selected.conditionsUnlocked)
            {
                GameManager.instance.progression.UnlockSomething(unlock);
            }
        }
    }

    public static event System.Action<string> InternalDialogueResponse;
    public static event System.Action EndConversationEvent;








    private bool CheckConversationLength()
    {
        return promptGenerator.GetInteractionCount() > 10;
    }

    private void StartConversation()
    {
        // Return one basic Start Message or Generate one Random with Prompt
    }

    private void EndConversation()
    {
        ExpectedAnswer answer = characterData.dialougeParts["goodbye"].allOptions[0];
        Debug.Log(answer.npcResponse[Random.Range(0, answer.npcResponse.Length)]);
        InternalDialogueResponse?.Invoke(answer.npcResponse[Random.Range(0, answer.npcResponse.Length)]);
        SetState("");
        EndConversationEvent?.Invoke();
    }

}