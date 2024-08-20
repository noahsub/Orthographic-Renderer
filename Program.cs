using System;
using System.Threading.Tasks;
using Avalonia;
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
        Task.Run(() => HardwareManager.SetupComputer())
            .ContinueWith(_ =>
            {
                HardwareManager.CollectHardwareToMonitor();
            });

        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp() =>
        AppBuilder.Configure<App>().UsePlatformDetect().WithInterFont().LogToTrace();
}
