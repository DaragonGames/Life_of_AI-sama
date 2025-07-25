public class PromptGenerator
{
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

    public string[] defaultPrompt(string userM)
    {
        string systemM = "Answer in a single sentence if possible. You are a character in a visual novel videogame. ";
        string developerM ="Setting: You are in a Highschool talking to the new transfer student for the first time. ";

        developerM += "Your role: Miko a female classmate of the player. Your kind and helpfull, but also shy and clumsy.";

        memory.AddToConversation(userM);
        return new string[] {systemM, developerM, userM, memory.GetSummary() };
    }
}