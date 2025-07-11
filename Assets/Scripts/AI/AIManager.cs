using System;
using UnityEngine;

public static class AIManager
{
    private static PromptGenerator promptGenerator;

    public static void Initialize()
    {
        ChatGPTConnector.GetApiKey();
        promptGenerator = new PromptGenerator();
    }

    // Is triggered from Game Objects
    public static void TextRequest(string userInput)
    {
        // Sends request to Connector (GPT or LLama)
        // Currently hardcoded for GPT
        string prompt = promptGenerator.defaultPrompt(userInput);
        ChatGPTConnector.SendRequest(prompt);
    }

    // Is triggered from AI Clonnectors
    public static void TextResponse(string answer)
    {
        AIResponse?.Invoke(answer);
    }

    // Is Invoked for other Game Objects to Listen to
    public static event Action<string> AIResponse;

}
