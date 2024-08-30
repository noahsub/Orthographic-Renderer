////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// App.axaml.cs
// This file is responsible for initializing the application
//
// Author(s): https://github.com/noahsub
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// IMPORTS
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using LibreHardwareMonitor.Software;
using Orthographic.Renderer.Managers;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// NAMESPACE
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Orthographic.Renderer;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// APP CLASS
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

/// <summary>
/// Initializes the application.
/// </summary>
public partial class App : Application
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // INITIALIZATION
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Loads the application.
    /// </summary>
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    /// <summary>
    /// Startup logic for the application.
    /// </summary>
    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // if the operating system is linux
            if (OperatingSystem.IsUnix)
            {
                // get existing resource dictionary
                var resources = Current?.Resources;
                // set the global opacity to 1.0
                if (resources != null)
                {
                    resources["GlobalOpacity"] = 1.0;
                }
            }

            var splashScreen = new Windows.SplashScreen();
            splashScreen.Show();

            Task.Run(async () =>
            {
                // Perform hardware setup and collection asynchronously
                await Task.Run(HardwareManager.SetupComputer);
                await Task.Run(HardwareManager.CollectHardwareToMonitor);
                // Copy user files asynchronously
                await Task.Run(FileManager.CopyUserFiles);
                // Check for updates asynchronously
                DataManager.LatestVersion = await WebManager.GetLatestVersion();

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
