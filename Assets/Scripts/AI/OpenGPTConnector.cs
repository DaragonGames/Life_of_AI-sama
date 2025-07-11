using UnityEngine;
using System.IO;
using System.Diagnostics;

public static class OpenGPTConnector
{
    public static async void FromCHATGPT(string request)
    {
        // await req.SendWebRequest();
    }

    private static string path = "";

    public static void SendRequest(string request)
    {
        string arguments = request.Replace(' ', '_');


        ProcessStartInfo startInfo = new ProcessStartInfo(path, arguments)
        {
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using (Process process = Process.Start(startInfo))
        {
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            AIManager.TextResponse(output);
        }
    }

    public static void SetPath()
    {
        // File Path is different in Editor and Build, but the files can be in an extra Folder outside of the Project within the build folder
        path = Path.GetDirectoryName(Application.dataPath) + "/_Executable/";

        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            path += "file.exe";
        }
        else
        {
            path += "UnityAI";
        }
    }
    
}