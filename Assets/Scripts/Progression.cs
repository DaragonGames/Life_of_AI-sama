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
        {"miko", 1},{"eve", 1},{"lia", 1},{"lena", 1},{"cari", 1},
    };
        
    public List<Areas> unlockedAreas = new List<Areas>()
    {
        Areas.classroom, Areas.cafeteria, Areas.clubroom,
        Areas.hallway, Areas.musicRoom, Areas.rooftop
    };    
}
