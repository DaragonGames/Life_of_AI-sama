using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private DialogueManager dialogueManager;
    private AIManager aIManager;

    public Areas currentArea = Areas.classroom;
    public DayTimes currentDayTime = DayTimes.morning;

    void Start()
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

    public static void UserInput(string input)
    {
        instance.dialogueManager.HandleUserInput(input);
    }

}