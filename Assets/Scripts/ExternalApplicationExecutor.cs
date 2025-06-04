using UnityEngine;
using System.Diagnostics;

public class ExternalApplicationExecutor
{
    // File Path is different in Editor and Build, but the files can be in an extra Folder outside of the Project within the build folder
    public static void DebugRun(string input, bool python)
    {
        input = input.Replace(' ', '_');
        string output;

        if (python)
        {
            string pythonFilePath = Application.dataPath + "/Temporary/file.py";
            string pythonPath = "C:/Program Files/Python312/python.exe";
            string arguments = pythonFilePath + " " + input;
            output = Run(pythonPath, arguments);
        }
        else
        {
            string exePath = Application.dataPath + "/Temporary/file.exe";
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
