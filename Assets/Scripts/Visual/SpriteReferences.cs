using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/SpriteReferences")]
public class SpriteReferences : ScriptableObject
{
    public Sprite miko, eve, cari, lena, lia;

    public Sprite[] classroom;
    public Sprite[] hallway;
    public Sprite[] roof;
    public Sprite[] cooffeeShop;
    public Sprite[] musicRoom;
    public Sprite[] clubRoom;

    public Sprite GetBackground(Areas area, DayTimes time)
    {
        switch (area)
        {
            case Areas.classroom:
                return classroom[(int)time];
            case Areas.hallway:
                return hallway[(int)time];
            case Areas.rooftop:
                return roof[(int)time];
            case Areas.cafeteria:
                return cooffeeShop[(int)time];
            case Areas.musicRoom:
                return musicRoom[(int)time];
            case Areas.clubroom:
                return clubRoom[(int)time];
            default:
                return classroom[(int)time];
        }
    }

    public Sprite GetCharacter(string name)
    {
        switch (name)
        {
            case "miko":
                return miko;
            case "eve":
                return eve;
            case "cari":
                return cari;
            case "lena":
                return lena;
            case "lia":
                return lia;
            default:
                return miko;
        }
    }
}
