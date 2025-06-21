using UnityEngine;
using System.Collections;
using System.Text;
using UnityEngine.Networking;
using System.IO;

public class ChatGPTConnector : MonoBehaviour
{
    public static string memory = "Our conversation so far has been: ";
    private static string apiKey;

    public static IEnumerator SendRequest(string request)
    {
        string json = "{\"model\":\"gpt-4o-mini\",\"messages\":[{\"role\":\"user\",\"content\":\"" + request + "\"}]}";

        UnityWebRequest req = new UnityWebRequest("https://api.openai.com/v1/chat/completions", "POST");
        byte[] body = Encoding.UTF8.GetBytes(json);
        req.uploadHandler = new UploadHandlerRaw(body);
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");
        req.SetRequestHeader("Authorization", "Bearer " + apiKey);

        yield return req.SendWebRequest();
        yield return req.downloadHandler.text;
    }

    public static void GetApiKey()
    {
        string path = Path.Combine(Application.persistentDataPath, "apiKey.txt");
        if (File.Exists(path))
        {
            apiKey = File.ReadAllText(path);
        }
    }

    public static string ExtractMessage(string input)
    {
        string[] firstParts = input.Split("content\": \"");
        string[] secondParts = firstParts[1].Split("\",");
        return secondParts[0];
    }

    void CreateKeyFile()
    {
        string key = "";
        string path = Path.Combine(Application.persistentDataPath, "apiKey.txt");
        File.WriteAllText(path, key);
        Debug.Log(path);
    }

    /*
    * This Line might be usefull later on
    * string jsonString = Resources.Load<TextAsset>("Data").text;
    */

}




/*
An Example Answer from Chat-GPT
{
  "id": "chatcmpl-BfEMp5QqjBvw2wvtXYKjz7B9vrE0j",
  "object": "chat.completion",
  "created": 1749166375,
  "model": "gpt-4o-mini-2024-07-18",
  "choices": [
    {
      "index": 0,
      "message": {
        "role": "assistant",
        "content": "It looks like you might be testing the system. How can I assist you today?",
        "refusal": null,
        "annotations": []
      },
      "logprobs": null,
      "finish_reason": "stop"
    }
  ],
  "usage": {
    "prompt_tokens": 9,
    "completion_tokens": 17,
    "total_tokens": 26,
    "prompt_tokens_details": {
      "cached_tokens": 0,
      "audio_tokens": 0
    },
    "completion_tokens_details": {
      "reasoning_tokens": 0,
      "audio_tokens": 0,
      "accepted_prediction_tokens": 0,
      "rejected_prediction_tokens": 0
    }
  },
  "service_tier": "default",
  "system_fingerprint": "fp_34a54ae93c"
}
*/