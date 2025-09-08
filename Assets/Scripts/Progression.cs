using System.Collections.Generic;
using UnityEngine;

public class Progression
{
    public Dictionary<string, int> characterRelationship = new Dictionary<string, int>()
    {
        {"miko", 1},{"eve", 1},{"lia", 1},{"lena", 1},{"cari", 1},
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
    
}
