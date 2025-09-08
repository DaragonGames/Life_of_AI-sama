using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterScheduler
{
    public List<PersonalSchedule> fixedSchedules = new List<PersonalSchedule>()
    {
        new PersonalSchedule("Miko", new List<Appointment>(){
            new Appointment(DayTimes.evening, Areas.musicRoom, Days.thursday),
            new Appointment(DayTimes.evening, Areas.musicRoom, Days.monday)
        }),
        new PersonalSchedule("Lena", new List<Appointment>(){
            new Appointment(DayTimes.evening, Areas.clubroom, Days.firday)
        }),
        new PersonalSchedule("Lia", new List<Appointment>(){
            new Appointment(DayTimes.evening, Areas.clubroom, Days.tuesday)
        }),
        new PersonalSchedule("Lia", new List<Appointment>(){
            new Appointment(DayTimes.evening, Areas.clubroom, Days.wednesday)
        })
    };

    public Dictionary<Areas, string> getSchedule()
    {
        Dictionary<Areas, string> matches = new Dictionary<Areas, string>();
        List<string> allCharacters = new List<string>() { "Miko", "Eve", "Lia", "Lena", "Cari" };

        // Get all fixed Schedules
        GameManager gm = GameManager.instance;
        foreach (PersonalSchedule ps in fixedSchedules)
        {
            foreach (Appointment ap in ps.appointments)
            {
                if (ap.time == gm.currentDayTime && ap.day == gm.currentDay)
                {
                    matches.Add(ap.area, ps.name);
                    allCharacters.Remove(ps.name);
                }
            }
        }

        // Rooftop
        string[] options = new string[] { "Eve", "Cari", null, null };
        string pick = options[Random.Range(0, options.Length)];
        if (pick != null)
        {
            matches.Add(Areas.rooftop, pick);
            allCharacters.Remove(pick);
        }

        // Caffee
        options = new string[] { "Miko", "Lena", "Cari", null };
        pick = options[Random.Range(0, options.Length)];
        if (pick != null && allCharacters.Contains(pick))
        {
            matches.Add(Areas.cafeteria, pick);
            allCharacters.Remove(pick);
        }

        // Classroom
        options = new string[] { "Miko", "Eve", "Lia", "Cari", null, null };
        pick = options[Random.Range(0, options.Length)];
        if (pick != null && allCharacters.Contains(pick))
        {
            matches.Add(Areas.classroom, pick);
            allCharacters.Remove(pick);
        }

        // Hallway
        options = allCharacters.ToArray();
        if (matches.Count > 0)
        {
            options.Append(null);
            options.Append(null);
        }
        pick = options[Random.Range(0, options.Length)];
        if (pick != null && allCharacters.Contains(pick))
        {
            matches.Add(Areas.hallway, pick);
            allCharacters.Remove(pick);
        }

        return matches;

    }
}

public struct PersonalSchedule
{
    public string name;
    public List<Appointment> appointments;
    public PersonalSchedule(string name, List<Appointment> appointments)
    {
        this.name = name;
        this.appointments = appointments;
    }
}

public struct Appointment
{
    public DayTimes time;
    public Days day;
    public Areas area;
    public Appointment(DayTimes time, Areas area, Days day)
    {
        this.time = time;
        this.day = day;
        this.area = area;
    }
}

public struct CharacterPlaceRelations
{
    public string name;
    public Areas area;
    public int odd;
    public CharacterPlaceRelations(string name, Areas area, int odd)
    {
        this.name = name;
        this.area = area;
        this.odd = odd;
    }
}