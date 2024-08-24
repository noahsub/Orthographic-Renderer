using System.Diagnostics;

namespace Orthographic.Renderer.Managers;

public class ProcessManager
{
    public static string RunProcess(string path, string arguments)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = path,
                Arguments = arguments,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            },
        };

        process.Start();
        var output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();

        return output;
    }
    
    public static bool RunProcessCheck(string path, string arguments)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = path,
                Arguments = arguments,
                RedirectStandardOutput = false,
                UseShellExecute = false,
                CreateNoWindow = true,
            },
        };

        process.Start();
        process.WaitForExit();
        
        Debug.WriteLine($"COMMAND: {path} {arguments}");

        if (process.ExitCode != 0)
        {
            return false;
        }

        return true;
    }
}
