using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public DialogueManager dialogueManager;
    private AIManager aIManager;

    public SpriteReferences sprites;
    [NonSerialized] public Areas currentArea = Areas.classroom;
    public DayTimes currentDayTime = DayTimes.morning;
    public Days currentDay = Days.monday;
    [NonSerialized] public Action<string> InitiatingConversation;
    [NonSerialized] public Action EnterEmptyPlace;
    [NonSerialized] public Action Progression;
    [NonSerialized] public GeneralData generalData;
    [NonSerialized] public Progression progression;

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
            progression = new Progression();
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
        else
        {
            EnterEmptyPlace?.Invoke();
        }     
    }

    public void Continue()
    {
        if (currentDayTime == DayTimes.evening)
        {
            currentDayTime = DayTimes.morning;
            currentDay = (Days)(((int)currentDay + 1) % 5);
        }
        else
        {
            currentDayTime = (DayTimes)(((int)currentDayTime + 1) % 3);
        }
        Progression.Invoke();
    }

    public static void UserInput(string input)
    {
        instance.dialogueManager.HandleUserInput(input);
    }

}