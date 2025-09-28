using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public float dotSpeed = 0.5f;
    [SerializeField] private TMP_Text npcText;
    [SerializeField] private TMP_InputField inputText;
    [SerializeField] private Image background;
    [SerializeField] private Image character;
    [SerializeField] private GameObject continueButton;
    [SerializeField] private GameObject playerDialougeBox;
    private SpriteReferences sprites;
    private bool waiting = true;

    void Start()
    {
        AIManager.AIDialogueResponse += UpdateNPCText;
        AIManager.AITranscription += UpdatePlayerText;
        DialogueManager.InternalDialogueResponse += UpdateNPCText;
        GameManager.instance.InitiatingConversation += OnConversationStart;
        DialogueManager.EndConversationEvent += OnConversationEnd;
        GameManager.instance.EnterEmptyPlace += UpdateBackground;
        sprites = GameManager.instance.sprites;
    }

    void OnDestroy()
    {
        AIManager.AIDialogueResponse -= UpdateNPCText;
        AIManager.AITranscription -= UpdatePlayerText;
        DialogueManager.InternalDialogueResponse -= UpdateNPCText;
        GameManager.instance.InitiatingConversation -= OnConversationStart;
        DialogueManager.EndConversationEvent -= OnConversationEnd;
        GameManager.instance.EnterEmptyPlace -= UpdateBackground;
    }

    public void OnConversationStart(string name)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
        }
        continueButton.SetActive(false);
        playerDialougeBox.SetActive(true);
        npcText.text = "Waiting for Response ";
        StartCoroutine(waitingDisplay());
        UpdateVisuals(name);
    }

    public void OnConversationEnd()
    {
        continueButton.SetActive(true);
        playerDialougeBox.SetActive(false);
    }

    public void UpdateNPCText(string text)
    {
        npcText.text = text;
        waiting = false;
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
        if (inputText.text.Length == 0 || waiting)
        {
            return;
        }
        npcText.text = "Waiting for Response ";
        waiting = true;
        GameManager.UserInput(inputText.text);
        inputText.text = "";
        StartCoroutine(waitingDisplay());
    }

    public void UpdateVisuals(string name)
    {
        background.sprite = sprites.GetBackground(GameManager.instance.currentArea, GameManager.instance.currentDayTime);
        character.sprite = sprites.GetCharacter(name);
    }

    public void UpdateBackground()
    {
        background.sprite = sprites.GetBackground(GameManager.instance.currentArea, GameManager.instance.currentDayTime);
    }

    public IEnumerator waitingDisplay()
    {
        yield return new WaitForSeconds(dotSpeed);
        if (waiting)
        {
            StartCoroutine(waitingDisplay());
            npcText.text += ".";
        }
    }

}