public class PromptGenerator
{
    private string systemMessage = "Answer in a single sentence if possible. You are a character in a visual novel videogame, talking to the player character.";
    private ShortTermMemory memory;

    public PromptGenerator()
    {
        memory = new ShortTermMemory();
        AIManager.AIDialogueResponse += memory.AddToConversation;
    }

    public void OnDestroy()
    {
        AIManager.AIDialogueResponse -= memory.AddToConversation;
    }

    public string[] defaultChatPrompt(string userInput, CharacterData data, string setting)
    {
        string developerMessage = "Setting: " + setting;
        developerMessage += data.relationship[0];
        developerMessage += "Your Role:"; 
        foreach (string part in data.characterDescription)
        {
            developerMessage += " " + part;
        }
        
        memory.AddToConversation(userInput);

        return new string[] { systemMessage, developerMessage, userInput, memory.GetSummary() };
    }
}