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
using Orthographic.Renderer.Pages;

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
                splashScreen.SetLoadingTextUiThread("DETECTING COMPUTER HARDWARE");
                await Task.Run(HardwareManager.SetupComputer);

                splashScreen.SetLoadingTextUiThread("GATHERING HARDWARE COMPONENTS AND SENSORS");
                await Task.Run(HardwareManager.CollectHardwareToMonitor);

                // Copy user files asynchronously
                splashScreen.SetLoadingTextUiThread("COPYING USER FILES");
                await Task.Run(FileManager.CopyUserFiles);

                splashScreen.SetLoadingTextUiThread("CONFIGURING BLENDER PATH");
                if (OperatingSystem.IsWindows8OrGreater)
                {
                    DataManager.BlenderPath = "Blender/Windows/blender.exe";
                }

                if (OperatingSystem.IsUnix)
                {
                    DataManager.BlenderPath = "Blender/Linux/blender";
                }

                // Get Render Hardware
                splashScreen.SetLoadingTextUiThread("DETECTING BLENDER COMPATIBLE HARDWARE [Stuck? Only NVIDIA GPUs are supported.]");
                if (!string.IsNullOrEmpty(DataManager.BlenderPath))
                {
                    await Task.Run(HardwareManager.GetRenderHardware);
                }

                // Check for updates asynchronously
                splashScreen.SetLoadingTextUiThread("CHECKING FOR UPDATES");
                DataManager.LatestVersion = await WebManager.GetLatestVersion();

                splashScreen.SetLoadingTextUiThread("CREATING USER INTERFACE");

                // Switch to the UI thread to update the UI
                Dispatcher.UIThread.Post(async () =>
                {
                    // Create application pages
                    await NavigationManager.CreatePage(new UpdatePage());
                    await NavigationManager.CreatePage(new RequirementsPage());
                    await NavigationManager.CreatePage(new HardwarePage());
                    await NavigationManager.CreatePage(new ModelPage());
                    await NavigationManager.CreatePage(new LightingPage());
                    await NavigationManager.CreatePage(new ViewsPage());
                    await NavigationManager.CreatePage(new RenderPage());

                    // IMPORTANT: MainWindow must be created after all pages are created, otherwise,
                    // the first page will be initialized twice
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
