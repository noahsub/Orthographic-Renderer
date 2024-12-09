////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// HardwarePage.axaml.cs
// This file contains the logic for the HardwarePage.
//
// Copyright (C) 2024 noahsub
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// IMPORTS
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Newtonsoft.Json.Linq;
using Orthographic.Renderer.Controls;
using Orthographic.Renderer.Managers;
using Orthographic.Renderer.Windows;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// NAMASPACE
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Orthographic.Renderer.Pages;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// HARDARE PAGE CLASS
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

public partial class HardwarePage : UserControl
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // INITIALIZATION
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Initializes a new instance of the <see cref="HardwarePage"/> class.
    /// </summary>
    public HardwarePage()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Method that is called when the page is navigated to.
    /// </summary>
    public void Load()
    {
        // Clear the RenderHardwareStackPanel.
        RenderHardwareStackPanel.Children.Clear();

        // Get the total number of render capable devices.
        var totalDevices =
            DataManager.OptixDevices.Count
            + DataManager.CudaDevices.Count
            + DataManager.CpuDevices.Count;

        // If there are no devices, get the render hardware.
        if (totalDevices == 0)
        {
            HardwareManager.GetRenderHardware();
        }

        // Iterate through the Optix devices.
        foreach (var optixDevice in DataManager.OptixDevices)
        {
            // Create a list of engines and add OPTIX.
            var engines = new List<string> { "OPTIX" };

            // Iterate through the CUDA devices.
            foreach (var cudaDevice in DataManager.CudaDevices.ToList())
            {
                // If the CUDA device is the same as the OPTIX device, add CUDA and remove the device from the list.
                if (cudaDevice == optixDevice)
                {
                    // Add CUDA to the engines list.
                    engines.Add("CUDA");
                    // Remove the CUDA device from the list.
                    DataManager.CudaDevices.Remove(cudaDevice);
                    // Break the loop and continue to the next OPTIX device.
                    break;
                }
            }

            // Add the render hardware item to the RenderHardwareStackPanel.
            RenderHardwareStackPanel.Children.Add(
                new RenderHardwareItem(optixDevice, "GPU", engines)
            );
        }

        // Iterate through the CUDA devices.
        foreach (var cudaDevice in DataManager.CudaDevices)
        {
            // Add CUDA to the engines list.
            var engines = new List<string> { "CUDA" };

            // Add the render hardware item to the RenderHardwareStackPanel.
            RenderHardwareStackPanel.Children.Add(
                new RenderHardwareItem(cudaDevice, "GPU", engines)
            );
        }

        // Iterate through the CPU devices.
        foreach (var cpuDevice in DataManager.CpuDevices)
        {
            // Add EEVEE to the engines list.
            var engines = new List<string> { "CPU CYCLES" };

            // Add the render hardware item to the RenderHardwareStackPanel.
            RenderHardwareStackPanel.Children.Add(
                new RenderHardwareItem(cpuDevice, "CPU", engines)
            );
        }

        // Optix devices are prioritized over CUDA devices.
        if (DataManager.OptixDevices.Count != 0)
        {
            EngineLabel.Content = "OPTIX";
        }
        // CUDA devices are prioritized over CPU devices.
        else if (DataManager.CudaDevices.Count != 0)
        {
            EngineLabel.Content = "CUDA";
        }
        // If there are no devices, use CPU CYCLES.
        else
        {
            EngineLabel.Content = "CPU CYCLES";
            WarningLabel.IsVisible = true;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // EVENTS
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Navigation to the previous page.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void BackButton_OnClick(object? sender, RoutedEventArgs e)
    {
        // Switch to the ModelPage
        var mainWindow = (MainWindow)this.VisualRoot!;
        // Navigate to the RequirementsPage
        NavigationManager.SwitchPage(mainWindow, "RequirementsPage");
    }

    /// <summary>
    /// Navigation to the next page.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void NextButton_OnClick(object? sender, RoutedEventArgs e)
    {
        // Switch to the RenderPage
        var mainWindow = (MainWindow)this.VisualRoot!;
        // Navigate to the RenderPage
        NavigationManager.SwitchPage(mainWindow, "ModelPage");
    }
}
