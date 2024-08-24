using System.Diagnostics;

namespace Orthographic.Renderer.Managers;

public class WebManager
{
    public static void OpenUrl(string url)
    {
        Process.Start(new ProcessStartInfo { FileName = url, UseShellExecute = true });
    }
}
