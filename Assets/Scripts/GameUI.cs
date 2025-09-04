using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [SerializeField] private TMP_Text npcText;
    [SerializeField] private TMP_InputField inputText;
    [SerializeField] private Image background;
    [SerializeField] private Image character;
    [SerializeField] private SpriteReferences sprites;

    void Start()
    {
        AIManager.AIDialogueResponse += UpdateNPCText;
        AIManager.AITranscription += UpdatePlayerText;
        DialogueManager.InternalDialogueResponse += UpdateNPCText;
        GameManager.instance.InitiatingConversation += UpdateVisuals;
    }

    void OnDestroy()
    {
        AIManager.AIDialogueResponse -= UpdateNPCText;
        AIManager.AITranscription -= UpdatePlayerText;
        DialogueManager.InternalDialogueResponse -= UpdateNPCText;
        GameManager.instance.InitiatingConversation -= UpdateVisuals;
    }

    public void UpdateNPCText(string text)
    {
        npcText.text = text;
    }

    public void UpdatePlayerText(string text)
    {
        if (inputText.text.Length == 0)
        {
            text = text.Remove(0, 1);
        }
        inputText.text += text;
    }

    public void OnSubmit()
    {
        if (inputText.text.Length == 0)
        {
            return;
        }
        GameManager.UserInput(inputText.text);
        npcText.text = "Waiting for Response";
        inputText.text = "";
    }

    public void UpdateVisuals(string name)
    {
        background.sprite = sprites.GetBackground(GameManager.instance.currentArea, GameManager.instance.currentDayTime);
        character.sprite = sprites.GetCharacter(name);
    }

}