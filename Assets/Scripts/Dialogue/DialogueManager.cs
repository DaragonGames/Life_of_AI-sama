using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    private CharacterData characterData;
    private PromptGenerator promptGenerator;
    private string debugSetting = "Miraku High School Classroom, morning before first class. ";

    void Start()
    {
        LoadCharacterData("Miko");
        promptGenerator = new PromptGenerator();
    }

    private void LoadCharacterData(string characterName)
    {
        string json = Resources.Load<TextAsset>(characterName).text;
        characterData = JsonUtility.FromJson<CharacterData>(json);
    }

    public void HandleUserInput(string input)
    {
        string[] prompt = promptGenerator.defaultChatPrompt(input, characterData, debugSetting);
        AIManager.TextRequest(prompt, false);
        return;
    }

}