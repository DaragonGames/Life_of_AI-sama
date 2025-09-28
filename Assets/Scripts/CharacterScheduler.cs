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

    private Dictionary<Areas, string> matches;
    private List<string> allCharacters;

    public Dictionary<Areas, string> getSchedule()
    {
        matches = new Dictionary<Areas, string>();
        allCharacters = new List<string>() { "Miko", "Eve", "Lia", "Lena", "Cari" };

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

        AddToMatch(new string[] { "Eve", "Cari"}, 0.5f, Areas.rooftop);
        AddToMatch(new string[] { "Miko", "Lena", "Cari"}, 0.33f, Areas.cafeteria);

        // Gurantee at least one person is avaible 
        bool rb = Random.value > 0.5f;
        float rc = 1.5f - 0.3f * allCharacters.Count; 
        float classroomChance = rb ? rc : 0;
        float hallwayChance = rb ? 0 : rc;
        AddToMatch(new string[] { "Miko", "Eve", "Lia", "Cari"}, classroomChance, Areas.classroom);
        AddToMatch(allCharacters.ToArray(), hallwayChance, Areas.hallway);

        return matches;
    }

    public void AddToMatch(string[] options, float nullChance, Areas area)
    {
        options =options.Intersect(allCharacters).ToArray();
        if (options.Length == 0)
        {
            return;
        }
        string pick = options[Random.Range(0, options.Length)];
        if (nullChance < Random.value && allCharacters.Contains(pick))
        {
            matches.Add(area, pick);
            allCharacters.Remove(pick);
        }
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