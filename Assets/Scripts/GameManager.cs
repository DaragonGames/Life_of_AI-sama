using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private DialogueManager dialogueManager;
    private AIManager aIManager;

    public SpriteReferences sprites;
    [NonSerialized] public Areas currentArea = Areas.classroom;
    [NonSerialized] public DayTimes currentDayTime = DayTimes.morning;
    [NonSerialized] public Days currentDay = Days.monday;
    [NonSerialized] public Action<string> InitiatingConversation;
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
    }

    public static void UserInput(string input)
    {
        instance.dialogueManager.HandleUserInput(input);
    }

}