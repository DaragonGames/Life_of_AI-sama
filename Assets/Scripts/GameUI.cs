using TMPro;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    [SerializeField] private TMP_Text npcText;
    [SerializeField] private TMP_InputField inputText;

    void Start()
    {
        AIManager.AIResponse += UpdateNPCText;
        ChatGPTConnector.GetApiKey(); // Move this part
    }

    void Oestroy()
    {
        AIManager.AIResponse -= UpdateNPCText;
    }

    public void UpdateNPCText(string text)
    {
        npcText.text = text;
    }

    public void OnSubmit()
    {
        AIManager.TextRequest(inputText.text);
        npcText.text = "Waiting for Response";
        inputText.text = "";
    }

}