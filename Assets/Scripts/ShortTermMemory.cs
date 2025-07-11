using System.Collections.Generic;
using UnityEngine;

public class ShortTermMemory : MonoBehaviour
{
    private string summary = "Hello Chat GPT, here is our conversation so far: ";
    private List<string> messages = new List<string>();

    public void AddToConversation(string message, string message2)
    {
        messages.Add(message);
        messages.Add(message2);
        Summarize();
    }

    public void Summarize()
    {
        summary += " User said: " + messages[messages.Count - 2];
        summary += " You said: " + messages[messages.Count - 1];
    }

    public string GetSummary()
    {
        return (messages.Count == 0) ? "" : summary;
    }

}
