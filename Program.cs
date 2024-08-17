using Avalonia;
using System;
using System.Threading.Tasks;
using Orthographic.Renderer.Managers;

namespace Orthographic.Renderer;

class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        Task.Run(() => HardwareStatusManager.SetupComputer())
            .ContinueWith(task =>
            {
                if (HardwareStatusManager.HardwareMonitoringReady)
                {
                    HardwareStatusManager.CollectRenderHardware();
                    foreach (var hardware in HardwareStatusManager.RenderHardware)
                    {
                        Console.WriteLine(hardware);
                    }
                }
            });
        
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}