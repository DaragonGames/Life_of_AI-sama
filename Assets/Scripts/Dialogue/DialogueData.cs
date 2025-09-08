using System;
using System.Collections.Generic;

[Serializable]
public struct CharacterData
{
    public string name;
    public string[] relationship;
    public string[] characterDescription;
    public List<DialogueNode> dialougePartsSource;
    public Dictionary<string, DialogueNode> dialougeParts;
}

[Serializable]
public struct GeneralData
{
    public string checkingSystemPrompt;
    public string chatSystemPrompt;
    public Dictionary<Areas, string> areaDescriptions;
    public Dictionary<DayTimes, string> dayTimesDescriptions;

    public GeneralData(GeneralDataSource sourceData)
    {
        checkingSystemPrompt = sourceData.checkingSystemPrompt;
        chatSystemPrompt = sourceData.chatSystemPrompt;
        areaDescriptions = new Dictionary<Areas, string>();
        dayTimesDescriptions = new Dictionary<DayTimes, string>();

        for (int i = 0; i < sourceData.areas.Length; i++)
        {
            areaDescriptions.Add((Areas)i, sourceData.areas[i]);
        }

        for (int i = 0; i < sourceData.dayTimes.Length; i++)
        {
            dayTimesDescriptions.Add((DayTimes)i, sourceData.dayTimes[i]);
        }
    }
}

public struct GeneralDataSource
{
    public string checkingSystemPrompt;
    public string chatSystemPrompt;
    public string[] areas;
    public string[] dayTimes;
}

[Serializable]
public class DialogueNode
{
    public string id;
    public List<ExpectedAnswer> allOptions;

    public DialogueNode(string id, List<ExpectedAnswer> allOptions)
    {
        this.id = id;
        this.allOptions = allOptions;
    }
}

[Serializable]
public struct ExpectedAnswer
{
    public string[] possibleAnswer;
    public string leadsToID;
    public string[] npcResponse;
    public string[] conditionsRequired;
    public string[] conditionsUnlocked;
    public ExpectedAnswer(string[] possibleAnswer, string leadsToID, string[] npcResponse, string[] conditionsRequired, string[] conditionsUnlocked)
    {
        this.possibleAnswer = possibleAnswer;
        this.leadsToID = leadsToID;
        this.npcResponse = npcResponse;
        this.conditionsRequired = conditionsRequired;
        this.conditionsUnlocked = conditionsUnlocked;
    }
}

public enum Areas { classroom, cafeteria, clubroom, hallway, rooftop, musicRoom }
public enum DayTimes { morning, afternoon, evening }
public enum Days {monday, tuesday, wednesday, thursday, firday, saturday, sunday}