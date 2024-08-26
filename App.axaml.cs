using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Orthographic.Renderer.Managers;

namespace Orthographic.Renderer;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var splashScreen = new Windows.SplashScreen();
            splashScreen.Show();

            Task.Run(async () =>
            {
                // Perform hardware setup and collection asynchronously
                await Task.Run(() => HardwareManager.SetupComputer());
                await Task.Run(() => HardwareManager.CollectHardwareToMonitor());

                // Switch to the UI thread to update the UI
                Dispatcher.UIThread.Post(() =>
                {
                    var mainWindow = new Windows.MainWindow();
                    desktop.MainWindow = mainWindow;
                    mainWindow.Show();
                    splashScreen.Close();
                });
            });
        }

        base.OnFrameworkInitializationCompleted();
    }
}
