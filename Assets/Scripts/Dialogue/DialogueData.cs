using System;
using System.Collections.Generic;

public struct CharacterData
{
    public string name;
    public string[] relationship;
    public string[] characterDescription;
    public List<DialoguePart> dialougePartsSource;
    public Dictionary<string,DialoguePart> dialougeParts;
}

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
public class DialoguePart
{
    public string id;
    public string npcResponse;
    public List<ExpectedAnswer> allOptions;
}

public struct ExpectedAnswer
{
    public string possibleAnswer;
    public string leadsToID;
}

public enum Areas { classroom, cafeteria, gym, libary, clubroom, hallway, rooftop, courtyard }
public enum DayTimes { morning, lunchbreak, evening, weekend }
public enum Days {monday, tuesday, wednesday, thursday, firday, saturday, sunday}