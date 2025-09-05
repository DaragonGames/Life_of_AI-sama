using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TravelButton : MonoBehaviour
{
    private Areas area;
    private string person;
    [SerializeField] private Image character;

    public void SelectDestination()
    {
        GameManager.instance.TravelToNewLocation(area, person);
    }

    public void SetButton(Areas area, string person, bool newCharacter)
    {
        this.area = area;
        this.person = person;
        string areaName = GameManager.instance.generalData.areaDescriptions[area];
        GetComponentInChildren<TMP_Text>().text = areaName;
        if (!newCharacter)
        {
            character.sprite = GameManager.instance.sprites.GetCharacter(person);
        }
    }
    


}
