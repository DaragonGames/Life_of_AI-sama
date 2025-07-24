using System;
using UnityEngine;

public static class AIManager
{
    private static PromptGenerator promptGenerator;
    public static RunWhisper whisperAPI;

    public static void Initialize()
    {
        ChatGPTConnector.GetApiKey();
        OpenGPTConnector.SetPath();
        promptGenerator = new PromptGenerator();
    }

    // Is triggered from Game Objects
    public static void TextRequest(string userInput)
    {
        // Sends request to Connector (GPT or LLama)
        // Currently hardcoded for GPT
        string[] prompt = promptGenerator.defaultPrompt(userInput);
        ChatGPTConnector.SendRequest(prompt);
        //OpenGPTConnector.SendRequest(prompt);
    }

    public static void TranscriptionRequest(AudioClip source)
    {
        whisperAPI.Transcribe(source);
    }

    // Is triggered from AI Clonnectors
    public static void TextResponse(string answer)
    {
        AIResponse?.Invoke(answer);
    }

    public static void TranscriptionResult(string text)
    {
        Debug.Log(text);
        AITranscription?.Invoke(text);
    }

    // Is Invoked for other Game Objects to Listen to
    public static event Action<string> AIResponse;
    public static event Action<string> AITranscription;

}
