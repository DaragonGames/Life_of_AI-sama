using System;
using System.Collections.Generic;
using UnityEngine;

public class TravelSelection : MonoBehaviour
{
    public GameObject buttonPrefab;

    private Dictionary<Areas, string> avaibility = new Dictionary<Areas, string>()
    {
        { Areas.classroom, "7777700" }, { Areas.cafeteria, "3333300" },
        { Areas.clubroom, "6666600" }, { Areas.hallway, "7777700" },
        { Areas.rooftop, "2222200" }, { Areas.musicRoom, "4444400" }
    };

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
        foreach (Areas area in GameManager.instance.progression.unlockedAreas)
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
        for (int i = transform.childCount; i > 0; i--)
        {
            Destroy(transform.GetChild(i - 1).gameObject);
        }

        var matches = cd.getSchedule();

        int pos = 0;
        foreach (var area in GetAllAvaibleAreas())
        {
            string person = null;
            if (matches.ContainsKey(area))
            {
                person = matches[area];
            }
            GameObject obj = Instantiate(buttonPrefab, transform);
            TravelButton button = obj.GetComponent<TravelButton>();

            button.SetButton(area, person);
            button.transform.localPosition = new Vector3(0, -120 * pos, 0);
            pos++;
        }
        transform.localPosition = new Vector3(-25, 60 * (pos - 1), 0);
    }

    private CharacterScheduler cd = new CharacterScheduler();

    void Start()
    {
        // DEBUG
        SetUI();
        //GameObject obj = Instantiate(buttonPrefab, transform);
        //obj.GetComponent<TravelButton>().SetButton(Areas.classroom, "miko");
        GameManager.instance.Progression += SetUI;
    }

    void OnDestroy()
    {
        GameManager.instance.Progression -= SetUI;
    }

}
