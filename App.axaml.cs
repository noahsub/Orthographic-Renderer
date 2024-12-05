////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// App.axaml.cs
// This file is responsible for initializing the application
//
// Copyright (C) 2024 noahsub
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// IMPORTS
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using LibreHardwareMonitor.Hardware;
using Orthographic.Renderer.Managers;
using OperatingSystem = LibreHardwareMonitor.Software.OperatingSystem;

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
                splashScreen.SetLoadingTextUiThread("Setting up Computer");
                await Task.Run(HardwareManager.SetupComputer);
                splashScreen.SetLoadingTextUiThread("Collecting Hardware");
                await Task.Run(HardwareManager.CollectHardwareToMonitor);
                splashScreen.SetLoadingTextUiThread("Setting Render Engine");
                await Task.Run(() =>
                {
                    bool renderEngineSet = false;

                    foreach (var hardware in HardwareManager.HardwareToMonitor)
                    {
                        if (hardware.Type == HardwareType.GpuNvidia)
                        {
                            // deserialize data in 'nvidia_supported_devices.json' to a list of dynamic objects
                            var devices = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Tuple<string, float>>>(File.ReadAllText("Data/nvidia_supported_devices.json"));
                            foreach (var device in devices)
                            {
                                // if the device name matches the hardware name
                                if (hardware.Name.Contains(device.Item1))
                                {
                                    if (device.Item2 >= 5.0)
                                    {
                                        DataManager.RenderEngine = "CYCLES";
                                        DataManager.CycleDevice = "OPTIX";
                                        renderEngineSet = true;
                                        break;
                                    }
                
                                    if (device.Item2 >= 3.0)
                                    {
                                        DataManager.RenderEngine = "CYCLES";
                                        DataManager.CycleDevice = "CUDA";
                                        renderEngineSet = true;
                                        break;
                                    }
                
                                    if (device.Item2 < 3.0)
                                    {
                                        DataManager.RenderEngine = "BLENDER_EEVEE_NEXT";
                                        DataManager.CycleDevice = "";
                                        renderEngineSet = true;
                                        break;
                                    }
                                }
                            }
                        }

                        if (!renderEngineSet)
                        {
                            DataManager.RenderEngine = "BLENDER_EEVEE_NEXT";
                            DataManager.CycleDevice = "";
                        }
                    }
                    Debug.WriteLine(DataManager.RenderEngine);
                    Debug.WriteLine(DataManager.CycleDevice);
                });
                
                // Copy user files asynchronously
                splashScreen.SetLoadingTextUiThread("Copying User Files");
                await Task.Run(FileManager.CopyUserFiles);
                // Check for updates asynchronously
                splashScreen.SetLoadingTextUiThread("Checking for Updates");
                DataManager.LatestVersion = await WebManager.GetLatestVersion();

                splashScreen.SetLoadingTextUiThread("Creating Pages");

                // Switch to the UI thread to update the UI
                Dispatcher.UIThread.Post(async () =>
                {
                    var mainWindow = new Windows.MainWindow();
                    desktop.MainWindow = mainWindow;

                    // Create update page
                    await NavigationManager.CreatePage("UpdatePage");

                    // Create requirements page
                    await NavigationManager.CreatePage("RequirementsPage");

                    // Create model page
                    await NavigationManager.CreatePage("ModelPage");

                    // Create lighting page
                    await NavigationManager.CreatePage("LightingPage");

                    // Create views page
                    await NavigationManager.CreatePage("ViewsPage");

                    // Create render page
                    await NavigationManager.CreatePage("RenderPage");

                    mainWindow.Show();
                    splashScreen.Close();
                });
            });
        }

        base.OnFrameworkInitializationCompleted();
    }
}
