using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private DialogueManager dialogueManager;
    private AIManager aIManager;

    public Areas currentArea = Areas.classroom;
    public DayTimes currentDayTime = DayTimes.morning;
    public Days currentDay = Days.monday;
    public Action<string> InitiatingConversation;
    public SpriteReferences sprites;
    public GeneralData generalData;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            dialogueManager = GetComponent<DialogueManager>();
            aIManager = GetComponent<AIManager>();
            DontDestroyOnLoad(gameObject);

            string json = Resources.Load<TextAsset>("GeneralData").text;
            GeneralDataSource sourceData = JsonUtility.FromJson<GeneralDataSource>(json);
            generalData = new GeneralData(sourceData);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void TravelToNewLocation(Areas area, string person)
    {
        currentArea = area;
        if (person != null)
        {
            InitiatingConversation?.Invoke(person);
        }        
    }

    public static void UserInput(string input)
    {
        instance.dialogueManager.HandleUserInput(input);
    }

}