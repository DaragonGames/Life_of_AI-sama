using UnityEngine;

public class UiToggler : MonoBehaviour
{
    public RectTransform dialougeUI;
    public RectTransform travelUI;

    void Start()
    {
        GameManager.instance.InitiatingConversation += ToggleToDialogue;
    }

    void OnDestroy()
    {
        GameManager.instance.InitiatingConversation -= ToggleToDialogue;
    }

    void ToggleToDialogue(string empty)
    {
        travelUI.gameObject.SetActive(false);
        for (int i = 0; i < dialougeUI.childCount; i++)
        {
            dialougeUI.transform.GetChild(i).gameObject.SetActive(true);
        }
    }

}
