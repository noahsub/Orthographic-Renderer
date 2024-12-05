////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// App.axaml.cs
// This file is responsible for initializing the application
//
// Copyright (C) 2024 noahsub
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// IMPORTS
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Threading;
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
                splashScreen.SetLoadingTextUiThread("Initializing hardware setup...");
                await Task.Run(HardwareManager.SetupComputer);

                splashScreen.SetLoadingTextUiThread("Collecting hardware information...");
                await Task.Run(HardwareManager.CollectHardwareToMonitor);

                // Copy user files asynchronously
                splashScreen.SetLoadingTextUiThread("Copying user files...");
                await Task.Run(FileManager.CopyUserFiles);

                // Check for updates asynchronously
                splashScreen.SetLoadingTextUiThread("Checking for updates...");
                DataManager.LatestVersion = await WebManager.GetLatestVersion();

                splashScreen.SetLoadingTextUiThread("Configuring Blender path...");
                if (OperatingSystem.IsWindows8OrGreater)
                {
                    DataManager.BlenderPath = "Blender/Windows/blender.exe";
                }

                // Get Render Hardware
                splashScreen.SetLoadingTextUiThread("Retrieving render hardware...");
                if (!string.IsNullOrEmpty(DataManager.BlenderPath))
                {
                    await Task.Run(HardwareManager.GetRenderHardware);
                }

                splashScreen.SetLoadingTextUiThread("Creating application pages...");

                // Switch to the UI thread to update the UI
                Dispatcher.UIThread.Post(async () =>
                {
                    var mainWindow = new Windows.MainWindow();
                    desktop.MainWindow = mainWindow;

                    // Create application pages
                    await NavigationManager.CreatePage("UpdatePage");
                    await NavigationManager.CreatePage("RequirementsPage");
                    await NavigationManager.CreatePage("HardwarePage");
                    await NavigationManager.CreatePage("ModelPage");
                    await NavigationManager.CreatePage("LightingPage");
                    await NavigationManager.CreatePage("ViewsPage");
                    await NavigationManager.CreatePage("RenderPage");

                    mainWindow.Show();
                    splashScreen.Close();
                });
            });
        }

        base.OnFrameworkInitializationCompleted();
    }
}
