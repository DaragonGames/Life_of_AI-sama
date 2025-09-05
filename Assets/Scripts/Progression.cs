using System.Collections.Generic;
using UnityEngine;

public class Progression
{
    public Progression()
    {
        // TODO Load Save File
    }

    public Dictionary<string, int> characterRelationship = new Dictionary<string, int>()
    {
        {"miko", 0},{"eve", 0},{"lia", 0},{"lena", 0},{"cari", 0},
    };
        
    public List<Areas> unlockedAreas = new List<Areas>()
    {
        Areas.classroom, Areas.cafeteria, Areas.clubroom,
        Areas.hallway, Areas.musicRoom, Areas.rooftop
    };    
}
