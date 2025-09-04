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
    public Action<string> InitiatingConversation;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            dialogueManager = GetComponent<DialogueManager>();
            aIManager = GetComponent<AIManager>();
            DontDestroyOnLoad(gameObject);
            
        }
        else
        {
            Destroy(gameObject);
        }
    }

    IEnumerator Start()
    {
        yield return new WaitForSeconds(0.1f);
        InitiatingConversation?.Invoke("miko");
    }

    public static void UserInput(string input)
    {
        instance.dialogueManager.HandleUserInput(input);
    }

}