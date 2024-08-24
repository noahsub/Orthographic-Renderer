namespace Orthographic.Renderer.Managers;

public class ProcessManager
{
    public static string RunProcess(string path, string arguments)
    {
        var process = new System.Diagnostics.Process
        {
            StartInfo = new System.Diagnostics.ProcessStartInfo
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
        var process = new System.Diagnostics.Process
        {
            StartInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = path,
                Arguments = arguments,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            },
        };

        process.Start();
        process.WaitForExit();
        
        // if process completed successfully return true otherwise return false
        return process.ExitCode == 0;
    }
}
