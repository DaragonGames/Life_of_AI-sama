using UnityEngine;

public class PromptGenerator
{
    private ShortTermMemory memory;
    private GeneralData generalData;

    public PromptGenerator()
    {
        memory = new ShortTermMemory();
        AIManager.AIDialogueResponse += memory.AddToConversation;
        DialogueManager.InternalDialogueResponse += memory.AddToConversation;
        GameManager.instance.InitiatingConversation += ResetMemory;

        generalData = GameManager.instance.generalData;
    }

    public void ResetMemory(string empty)
    {
        memory.Reset();
    }

    public int GetInteractionCount()
    {
        return memory.GetInteractionCount();
    }

    public void OnDestroy()
    {
        AIManager.AIDialogueResponse -= memory.AddToConversation;
        DialogueManager.InternalDialogueResponse -= memory.AddToConversation;
        GameManager.instance.InitiatingConversation -= ResetMemory;
    }

    public string[] defaultChatPrompt(string userInput, CharacterData data)
    {
        memory.AddToConversation(userInput);

        // Add Seting Text to Developer Prompt
        string areaText = generalData.areaDescriptions[GameManager.instance.currentArea];
        string dayTimeText = generalData.dayTimesDescriptions[GameManager.instance.currentDayTime];
        string developerMessage = "Current Location: " + areaText + ", Current Time:" + dayTimeText + ", ";

        // Add Character Infos to Developer 
        int maxR = data.relationship.Length-1;
        int currentR = GameManager.instance.progression.characterRelationship[data.name];
        developerMessage += data.relationship[maxR < currentR ? maxR : currentR] + " Your Role:";
        foreach (string part in data.characterDescription)
        {
            developerMessage += " " + part;
        }
        developerMessage += generalData.generalInfoPretext + generalData.generalInfo;

        return new string[] { generalData.chatSystemPrompt, developerMessage, userInput, memory.GetSummary() };
    }

    public string[] checkingPrompt(string userInput, string[] options )
    {
        string developerMessage = "";
        string lastMessage = memory.LastMessage();
        if (lastMessage != null)
        {
            developerMessage += "Here is your last message for context: " + memory.LastMessage();
        }
        

        developerMessage += " Options:";
        for (int i = 0; i < options.Length; i++)
        {
            developerMessage += " " + i + "=" + options[i];
        }      

        Debug.Log(developerMessage);

        return new string[] { generalData.checkingSystemPrompt, developerMessage, "User Input: " + userInput };
    }
}