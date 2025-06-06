using UnityEngine;
using System.IO;
using System.Diagnostics;

public class ExternalApplicationExecutor
{
    // File Path is different in Editor and Build, but the files can be in an extra Folder outside of the Project within the build folder
    public static void DebugRun(string input, bool python)
    {
        string pythonPath, output, exePath;
        string filepath = Path.GetDirectoryName(Application.dataPath) + "/_Executable/";
        input = input.Replace(' ', '_');

        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            pythonPath = "C:/Program Files/Python312/python.exe";
            exePath = filepath + "file.exe";
        }
        else
        {
            pythonPath = "/usr/bin/python3.12";
            exePath = filepath  + "UnityAI";
        }

        if (python)
        {
            string pythonFilePath = filepath + "UnityAI.py";
            string arguments = pythonFilePath + " " + input;
            output = Run(pythonPath, arguments);
        }
        else
        {            
            output = Run(exePath, input);
        }

        UnityEngine.Debug.Log(output);
    }

    public static string Run(string path, string arguments)
    {
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
            return output;
        }
    }

}
