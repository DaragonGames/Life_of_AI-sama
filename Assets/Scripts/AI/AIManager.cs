using System;

public static class AIManager
{
    public static void TextRequest(string prompt)
    {
        // Is triggered from outside
        // Sends request to Connector (GPT or LLama)
        // Currently hardcoded for GPT
        ChatGPTConnector.SendRequest(prompt);
    }

    public static void TextResponse(string answer)
    {
        AIResponse?.Invoke(answer);
    }

    public static event Action<string> AIResponse;

}
