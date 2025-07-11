using UnityEngine;
using System.Text;
using UnityEngine.Networking;
using System.IO;
using System.Collections.Generic;

public static class ChatGPTConnector
{
  private static string apiKey;

  public static async void SendRequest(string request)
  {
    string json = "{\"model\":\"gpt-4o-mini\",\"messages\":[{\"role\":\"user\",\"content\":\"" + request + "\"}]}";

    UnityWebRequest req = new UnityWebRequest("https://api.openai.com/v1/chat/completions", "POST");
    byte[] body = Encoding.UTF8.GetBytes(json);
    req.uploadHandler = new UploadHandlerRaw(body);
    req.downloadHandler = new DownloadHandlerBuffer();
    req.SetRequestHeader("Content-Type", "application/json");
    req.SetRequestHeader("Authorization", "Bearer " + apiKey);

    await req.SendWebRequest();    
    string response = ExtractMessage(req.downloadHandler.text);
    AIManager.TextResponse(response);
  }

  public static void GetApiKey()
  {
    string path = Path.Combine(Application.persistentDataPath, "apiKey.txt");
    if (File.Exists(path))
    {
      apiKey = File.ReadAllText(path);
    }
  }

  public static void CreateKeyFile()
  {
    string key = "";
    string path = Path.Combine(Application.persistentDataPath, "apiKey.txt");
    File.WriteAllText(path, key);
    Debug.Log(path);
  }

  public static string ExtractMessage(string input)
  {
    Debug.Log(input);
    try
    {
      Dictionary<string, object> data = ProcessGptOutput(input);
      Dictionary<string, object> choices = (Dictionary<string, object>)data["choices"];
      Dictionary<string, object> message = (Dictionary<string, object>)choices["message"];
      return (string)message["content"];
    }
    catch
    {
      return "Someone tell Stefan there is a Problem with my AI";
    }
  }

  public static Dictionary<string, object> ProcessGptOutput(string input)
  {
    input = input.Replace("null", "±");
    Dictionary<string, object> output = new Dictionary<string, object>();
    string key = "";
    string value = "";
    // Openings counts Brackets and Quotationmarks that encapsule data and also therefore serves as state in the Key Value Pair Finding Process
    string openings = "";
    const int StateAfterKey = 2;
    const int StateReadingValue = 3;
    const int DeepNestingThreshold = 4;
    bool escapeCharacter = false;

    foreach (char c in input)
    {
      // Find Key
      if (openings.Length < StateAfterKey)
      {
        openings += (c == '"') ? c : "";
        key += (openings.Length == 1 && c != '"') ? c : "";
        continue;
      }

      // Find Start of Value
      if (openings.Length == StateAfterKey)
      {
        if (char.IsNumber(c) || c == '±')
        {
          value += c;
          openings += c;
        }
        if (c == '"' || c == '[' || c == '{')
        {
          openings += c;
        }
        continue;
      }

      // Find Value
      if (openings.Length >= StateReadingValue)
      {
        if (escapeCharacter||c == '\\')
        {
          value += c;
          escapeCharacter =!escapeCharacter;
          continue;
        }
        if (c == '"' || c == '[' || c == '{' || c == ']' || c == '}')
        {
          openings += c;
        }
        if (!isClosed(openings) || char.IsNumber(c))
        {
          value += c;
          continue;
        }
      }

      // Key Value Pair is found
      value = value.Replace("±", "null");
      object finaleOutput = value;
      if (openings.Length > DeepNestingThreshold)
      {
        finaleOutput = ProcessGptOutput(value);
      }
      if (char.IsNumber(openings[openings.Length - 1]))
      {
        finaleOutput = int.Parse(value);
      }
      if (value.Length == 0 || value == "null")
      {
        finaleOutput = null;
      }

      output.Add(key, finaleOutput);
      key = "";
      value = "";
      openings = "";
    }

    return output;
  }

  private static bool isClosed(string input)
  {
    if (countCharacter(input, '{') - countCharacter(input, '}') > 0)
    {
      return false;
    }
    if (countCharacter(input, '[') - countCharacter(input, ']') > 0)
    {
      return false;
    }
    if (countCharacter(input, '"') % 2 == 1)
    {
      return false;
    }
    return true;
  }

  private static int countCharacter(string input, char lookingFor)
  {
    int count = 0;
    foreach (char c in input)
    {
      if (c == lookingFor)
      {
        count++;
      }
    }
    return count;
  }

}