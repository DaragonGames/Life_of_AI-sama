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

        generalData = GameManager.instance.generalData;
    }

    public void OnDestroy()
    {
        AIManager.AIDialogueResponse -= memory.AddToConversation;
        DialogueManager.InternalDialogueResponse -= memory.AddToConversation;
    }

    public string[] defaultChatPrompt(string userInput, CharacterData data)
    {
        memory.AddToConversation(userInput);

        // Add Seting Text to Developer Prompt
        string areaText = generalData.areaDescriptions[GameManager.instance.currentArea];
        string dayTimeText = generalData.dayTimesDescriptions[GameManager.instance.currentDayTime];
        string developerMessage = "Setting: " + areaText + ", " + dayTimeText;

        // Add Character Infos to Developer Prompt
        developerMessage += data.relationship[0] + " Your Role:";
        foreach (string part in data.characterDescription)
        {
            developerMessage += " " + part;
        }

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
        

        developerMessage += "Options: ";
        for (int i = 0; i < options.Length; i++)
        {
            developerMessage += i + ":" + options[i];
        }       

        Debug.Log(generalData.checkingSystemPrompt + "  " +  developerMessage + "  " + userInput );

        return new string[] { generalData.checkingSystemPrompt, developerMessage, "User Input: " + userInput };
    }
}