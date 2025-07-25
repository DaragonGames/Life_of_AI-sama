using System;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    public static RunWhisper whisperAPI;

    void Start()
    {
        ChatGPTConnector.GetApiKey();
        OpenGPTConnector.SetPath();
        whisperAPI = GameObject.Find("SpeechRecorder")?.GetComponent<RunWhisper>();
    }

    // Is triggered from Game Objects
    public static void TextRequest(string[] prompt, bool internalRequest)
    {
        // Sends request to Connector (GPT or LLama)
        // Currently hardcoded for GPT
        ChatGPTConnector.SendRequest(prompt, internalRequest);
        //OpenGPTConnector.SendRequest(prompt); // uses String not string[]
    }

    public static void TranscriptionRequest(AudioClip source)
    {
        whisperAPI.Transcribe(source);
    }

    // Is triggered from AI Clonnectors
    public static void TextResponse(string answer, bool internalResponse)
    {
        if (internalResponse)
        {
            AIInternalResponse?.Invoke(answer);
        }
        else
        {
            AIDialogueResponse?.Invoke(answer);
        }        
    }

    public static void TranscriptionResult(string text)
    {
        AITranscription?.Invoke(text);
    }

    // Is Invoked for other Game Objects to Listen to
    public static event Action<string> AIDialogueResponse;
    public static event Action<string> AIInternalResponse;
    public static event Action<string> AITranscription;

}
