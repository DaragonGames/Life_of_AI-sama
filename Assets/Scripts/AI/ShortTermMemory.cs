using System.Collections.Generic;

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
            summary += (i % 2 == 0) ? " User said: " : " You said: ";
            summary += messages[i];
        }

        return summary;
    }

    public string LastMessage()
    {
        if (messages.Count == 0)
        {
            return null;
        }
        return messages[messages.Count - 1];
    }

}
