public class PromptGenerator
{
    private ShortTermMemory memory;

    public PromptGenerator()
    {
        memory = new ShortTermMemory();
        AIManager.AIResponse += memory.AddToConversation;        
    }

    public void OnDestroy()
    {
        AIManager.AIResponse -= memory.AddToConversation; 
    }

    public string defaultPrompt(string input)
    {
        string prompt = memory.GetSummary() + " Latest User Input: " + input;
        memory.AddToConversation(input);
        return prompt;
    }
}