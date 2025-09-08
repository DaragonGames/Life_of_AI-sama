using System.Collections.Generic;
using UnityEngine;

public class Progression
{
    public Dictionary<string, int> characterRelationship = new Dictionary<string, int>()
    {
        {"Miko", 0},{"Eve", 0},{"Lia", 0},{"Lena", 0},{"Cari", 0},
    };

    public Dictionary<string, int> charactersMeetCounter = new Dictionary<string, int>()
    {
        {"Miko", 0},{"Eve", 0},{"Lia", 0},{"Lena", 0},{"Cari", 0},
    };

    public List<Areas> unlockedAreas = new List<Areas>()
    {
        Areas.classroom, Areas.cafeteria,
        Areas.hallway, Areas.rooftop
    };

    public List<string> allUnlockedCondtions = new List<string>();

    public void UnlockSomething(string unlock)
    {
        if (allUnlockedCondtions.Contains(unlock))
        {
            return;
        }
        allUnlockedCondtions.Add(unlock);
        // Area Unlocks
        switch (unlock)
        {
            case "Area_Music_Room":
                unlockedAreas.Add(Areas.musicRoom);
                break;
            case "Area_Club_Room":
                unlockedAreas.Add(Areas.clubroom);
                break;
            case "Area_Rooftop_Room":
                unlockedAreas.Add(Areas.rooftop);
                break;
            case "Area_Cafeteria_Room":
                unlockedAreas.Add(Areas.cafeteria);
                break;
        }
    }

    public bool CheckCondtions(string[] conditions)
    {
        bool avaible = true;
        foreach (string condition in conditions)
        {
            avaible = avaible && allUnlockedCondtions.Contains(condition);
        }
        return avaible;
    }

    public void MeetCharacter(string name)
    {
        charactersMeetCounter[name]++;
        if (charactersMeetCounter[name] == 1)
        {
            characterRelationship[name]++;
        }
        if (charactersMeetCounter[name] == 3)
        {
            characterRelationship[name]++;
        }
    }

}
