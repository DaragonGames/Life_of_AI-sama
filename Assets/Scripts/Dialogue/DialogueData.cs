using System.Collections.Generic;

public struct CharacterData
{
    public string name;
    public string[] relationship;
    public string[] characterDescription;

}

public struct GeneralData
{
    public string chatSystemPrompt;
    public Dictionary<Areas, string> areaDescriptions;
    public Dictionary<DayTimes, string> dayTimesDescriptions;

    public GeneralData(GeneralDataSource sourceData)
    {
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
    public string chatSystemPrompt;
    public string[] areas;
    public string[] dayTimes;
}

public enum Areas {classroom, cafeteria, gym, libary, clubroom, hallway, rooftop, courtyard}
public enum DayTimes {morning, lunchbreak, evening, weekend}