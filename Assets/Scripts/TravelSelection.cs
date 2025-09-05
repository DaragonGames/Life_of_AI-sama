using System;
using System.Collections.Generic;
using UnityEngine;

public class TravelSelection : MonoBehaviour
{
    public GameObject buttonPrefab;

    private List<Areas> unlockedAreas = new List<Areas>()
    {
        Areas.classroom, Areas.cafeteria, Areas.clubroom,
        Areas.hallway, Areas.musicRoom, Areas.rooftop
    };

    private Dictionary<Areas, string> avaibility = new Dictionary<Areas, string>()
    {
        { Areas.classroom, "7777700" }, { Areas.cafeteria, "3333300" },
        { Areas.clubroom, "6666600" }, { Areas.hallway, "7777700" },
        { Areas.rooftop, "2222200" }, { Areas.musicRoom, "4444400" }
    };

    private List<string> unlockedCharacters = new List<string>() {"miko", "eve", "lia", "lena", "cari"};

    private bool CheckAvaibility(Areas area)
    {
        GameManager gm = GameManager.instance;
        string code = avaibility[area];
        int number = int.Parse(code[(int)gm.currentDay].ToString());
        return (number >> (int)gm.currentDayTime) % 2 > 0;
    }

    private List<Areas> GetAllAvaibleAreas()
    {
        List<Areas> list = new List<Areas>();
        foreach (Areas area in unlockedAreas)
        {
            if (CheckAvaibility(area))
            {
                list.Add(area);
            }
        }
        return list;
    }

    private void SetUI()
    {
        string[] debug = new string[5] {"miko", "eve", "lia", "lena", "cari"};

        int pos = 0;
        foreach (var area in GetAllAvaibleAreas())
        {
            string person = debug[UnityEngine.Random.Range(0, debug.Length)];
            GameObject obj = Instantiate(buttonPrefab, transform);
            TravelButton button = obj.GetComponent<TravelButton>();
            
            button.SetButton(area, person, !unlockedCharacters.Contains(person)); // TODO
            button.transform.localPosition = new Vector3(0, -120 * pos, 0);
            pos++;
        }
        transform.localPosition = new Vector3(-25, 60 * (pos - 1), 0);
    }

    void Start()
    {
        // DEBUG
        SetUI();
    }

}
