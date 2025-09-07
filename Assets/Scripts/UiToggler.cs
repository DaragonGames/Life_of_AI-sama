using UnityEngine;

public class UiToggler : MonoBehaviour
{
    public RectTransform dialougeUI;
    public RectTransform travelUI;
    public GameObject emptyPlaceUI;

    void Start()
    {
        GameManager.instance.InitiatingConversation += ToggleToDialogue;
        GameManager.instance.EnterEmptyPlace += ToggleToEmptyPlace;
        GameManager.instance.Progression += ToggleToTravel;
    }

    void OnDestroy()
    {
        GameManager.instance.InitiatingConversation -= ToggleToDialogue;
        GameManager.instance.EnterEmptyPlace -= ToggleToEmptyPlace;
        GameManager.instance.Progression -= ToggleToTravel;
    }

    void ToggleToEmptyPlace()
    {
        travelUI.gameObject.SetActive(false);
        emptyPlaceUI.SetActive(true);
    }

    void ToggleToDialogue(string empty)
    {
        travelUI.gameObject.SetActive(false);
    }

    void ToggleToTravel()
    {
        travelUI.gameObject.SetActive(true);
        emptyPlaceUI.SetActive(false);
        for (int i = 0; i < dialougeUI.childCount; i++)
        {
            dialougeUI.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

}
