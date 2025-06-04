using UnityEngine;
using System.Collections;
using System.Text;
using UnityEngine.Networking;
using System.IO;

public class ChatGPTConnector : MonoBehaviour
{
    private static string apiKey;

    IEnumerator SendRequest(string request)
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
