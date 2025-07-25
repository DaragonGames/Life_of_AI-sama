using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    private PromptGenerator promptGenerator;

    void Start()
    {
        promptGenerator = new PromptGenerator();
    }

    private bool smallTalkMode = true;

    public void HandleUserInput(string input)
    {
        string[] prompt = promptGenerator.defaultPrompt(input);
        AIManager.TextRequest(prompt, false);
    }
}