using System.Collections.Generic;
using UnityEngine;

public class ShortTermMemory
{
    private List<string> messages = new List<string>();

    public void AddToConversation(string message)
    {
        messages.Add(message);
    }

    public string GetSummary()
    {
        if (messages.Count == 0)
        {
            return "";
        }

        string summary = "";

        for (int i = 0; i < messages.Count; i++)
        {
            summary += (i % 2 == 0) ? " You said: " : " User said: ";
            summary += messages[i];
        }
        Debug.Log(summary);
        return summary;
    }

    public void Reset()
    {
        messages = new List<string>();
    }

    public string LastMessage()
    {
        if (messages.Count == 0)
        {
            return null;
        }
        return messages[messages.Count - 1];
    }

    public int GetInteractionCount()
    {
        return messages.Count;
    }

}
