using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            AIManager.whisperAPI = GameObject.Find("SpeechRecorder").GetComponent<RunWhisper>();
            AIManager.Initialize();
        }
        else
        {
            Destroy(gameObject);
        }
    }

}
