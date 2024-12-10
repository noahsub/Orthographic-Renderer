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
using Orthographic.Renderer.Interfaces;
using Orthographic.Renderer.Managers;
using Orthographic.Renderer.Windows;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// NAMASPACE
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Orthographic.Renderer.Pages;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// HARDARE PAGE CLASS
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

public partial class HardwarePage : UserControl, IPage
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // INITIALIZATION
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Initializes a new instance of the <see cref="HardwarePage"/> class.
    /// </summary>
    public HardwarePage()
    {
        Initialize();
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

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // IPAGE INTERFACE IMPLEMENTATION
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Initializes the HardwarePage.
    /// </summary>
    public void Initialize()
    {
        InitializeComponent();
    }

    /// <summary>
    /// When the page is first loaded by the user.
    /// </summary>
    public void OnFirstLoad()
    {
        // If there are no render devices, get the render hardware
        if (DataManager.RenderDevices.Count == 0)
        {
            HardwareManager.GetRenderHardware();
        }

        // Add each render device to the stack panel
        foreach (var device in DataManager.RenderDevices)
        {
            var item = new RenderHardwareItem(device);
            RenderHardwareStackPanel.Children.Add(item);
        }
        
        FrameworkLabel.Content = DataManager.RenderFramework;
    }

    /// <summary>
    /// When the page is navigated to.
    /// </summary>
    public void OnNavigatedTo()
    {
        return;
    }
}
