using System.Collections;
using TMPro;
using UnityEngine;

public class DialougeManager : MonoBehaviour
{
    void Start()
    {
        ChatGPTConnector.GetApiKey();
    }

    private int i = 0;
    public TMP_Text text1, text2, text3;
    public TMP_InputField inputText;

    public void OnSubmit()
    {
        text1.text = text3.text;
        text2.text = inputText.text;
        text3.text = "Waiting for response... " + i;
        i++;
        StartCoroutine(HandleResponse(inputText.text));
        inputText.text = "";
    }

    IEnumerator HandleResponse(string UserInput)
    {
        string s = "";
        string prompt = ChatGPTConnector.memory + "This is the latest user message: " + UserInput + " " + "Please respond to the message consider the context as needed.";
        if (i==1)
        { prompt = UserInput; }
        yield return Run<string>(ChatGPTConnector.SendRequestOld(prompt), (output) => s = output);
        s = ChatGPTConnector.ExtractMessage(s);
        text3.text = s;
        
        ChatGPTConnector.memory += "User Message: " + UserInput + " ";
        ChatGPTConnector.memory += "Your Message: " + s + " ";
    }

    public static IEnumerator Run<T>(IEnumerator target, System.Action<T> output)
    {
        object result = null;
        while (target.MoveNext())
        {
            result = target.Current;
            yield return result;
        }
        output((T)result);
    }
}


