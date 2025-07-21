using TMPro;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    [SerializeField] private TMP_Text npcText;
    [SerializeField] private TMP_InputField inputText;

    void Start()
    {
        AIManager.AIResponse += UpdateNPCText;
        AIManager.AITranscription += UpdatePlayerText;
    }

    void OnDestroy()
    {
        AIManager.AIResponse -= UpdateNPCText;
        AIManager.AITranscription -= UpdatePlayerText;
    }

    public void UpdateNPCText(string text)
    {
        npcText.text = text;
    }

    public void UpdatePlayerText(string text)
    {
        if (inputText.text.Length == 0)
        {
            text = text.Remove(0,1);
        }
        inputText.text += text;
    }

    public void OnSubmit()
    {
        AIManager.TextRequest(inputText.text);
        npcText.text = "Waiting for Response";
        inputText.text = "";
    }

}