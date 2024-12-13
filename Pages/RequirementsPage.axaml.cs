////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// RequirementsPage.axaml.cs
// This file contains the logic for the RequirementsPage.
//
// Copyright (C) 2024 noahsub
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// IMPORTS
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using LibreHardwareMonitor.Software;
using Orthographic.Renderer.Constants;
using Orthographic.Renderer.Controls;
using Orthographic.Renderer.Interfaces;
using Orthographic.Renderer.Managers;
using Orthographic.Renderer.Windows;
using OperatingSystem = LibreHardwareMonitor.Software.OperatingSystem;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// NAMESPACE
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Orthographic.Renderer.Pages;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// REQUIREMENTS PAGE CLASS
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

/// <summary>
/// Represents the requirements page of the application.
/// </summary>
public partial class RequirementsPage : UserControl, IPage
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // GLOBALS
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Browsable file path selector for the custom Blender file path.
    /// </summary>
    private FilePathSelector _blenderFilePathSelector;

    /// <summary>
    /// Label for the default Blender file path.
    /// </summary>
    private Label _blenderFilePathLabel;

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // INITIALIZATION
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Initializes a new instance of the <see cref="RequirementsPage"/> class.
    /// </summary>
    public RequirementsPage()
    {
        Initialize();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // EVENTS
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Handles the click event of the Next button.
    /// </summary>
    private void NextButton_OnClick(object? sender, RoutedEventArgs e)
    {
        // Fix the path
        _blenderFilePathSelector.FixPath();
        // Check if the path is valid
        if (!_blenderFilePathSelector.CheckPath(FileType.Executable))
        {
            return;
        }

        // Get the path
        var path = _blenderFilePathSelector.GetPath();

        // Save the Blender path if it has changed
        if (!string.Equals(DataManager.BlenderPath, path))
        {
            DataManager.BlenderPath = path;
        }

        // Switch to the hardware page
        var mainWindow = (MainWindow)this.VisualRoot!;
        NavigationManager.SwitchPage(mainWindow, "HardwarePage");
    }

    /// <summary>
    /// Handles the click event of the Blender Install button.
    /// </summary>
    private void BlenderInstallButton_OnClick(object? sender, RoutedEventArgs e)
    {
        WebManager.OpenUrl("https://www.blender.org/download/lts/4-2/");
    }

    /// <summary>
    /// Handles the selection of the Blender file path.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void BlenderPathToggleButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (sender is ToggleButton toggleButton)
        {
            if (toggleButton == BundledBlenderToggleButton)
            {
                BundledBlenderToggleButton.IsChecked = true;
                CustomBlenderToggleButton.IsChecked = false;

                if (BlenderPathStackPanel.Children.Count > 1)
                {
                    // Remove existing Blender path
                    BlenderPathStackPanel.Children.RemoveAt(1);
                    // Add the default Blender path
                    BlenderPathStackPanel.Children.Add(_blenderFilePathLabel);
                    // Set the default Blender path to the file path selector
                    if (OperatingSystem.IsWindows8OrGreater)
                    {
                        DataManager.BlenderPath = "Blender/Windows/blender.exe";
                        _blenderFilePathSelector.SetPath("Blender/Windows/blender.exe");
                    }

                    if (OperatingSystem.IsUnix)
                    {
                        DataManager.BlenderPath = "Blender/Linux/blender";
                        _blenderFilePathSelector.SetPath("Blender/Linux/blender");
                    }
                }
            }
            else if (toggleButton == CustomBlenderToggleButton)
            {
                CustomBlenderToggleButton.IsChecked = true;
                BundledBlenderToggleButton.IsChecked = false;

                if (BlenderPathStackPanel.Children.Count > 1)
                {
                    // Remove existing Blender path
                    BlenderPathStackPanel.Children.RemoveAt(1);
                    // Add the default Blender path
                    BlenderPathStackPanel.Children.Add(_blenderFilePathSelector);
                }
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // IPAGE INTERFACE IMPLEMENTATION
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Initializes the RequirementsPage.
    /// </summary>
    public void Initialize()
    {
        InitializeComponent();

        _blenderFilePathSelector = new FilePathSelector();
        _blenderFilePathLabel = new Label();
        var colour = (Color)(Application.Current?.Resources["ControlsBackground"] ?? "#000000");

        _blenderFilePathLabel = new Label
        {
            Padding = new Thickness(10, 0, 0, 0),
            Background = new SolidColorBrush(colour),
            CornerRadius = new CornerRadius(
                Application.Current?.Resources["Radius"] is string radiusStr
                    ? float.Parse(radiusStr)
                    : 10
            ),
            Height = 30,
            VerticalContentAlignment = VerticalAlignment.Center,
        };
    }

    /// <summary>
    /// When the page is first loaded by the user.
    /// </summary>
    public void OnFirstLoad()
    {
        BundledBlenderToggleButton.IsChecked = true;
        BlenderPathStackPanel.Children.Add(_blenderFilePathLabel);
        _blenderFilePathLabel.Content = DataManager.BlenderPath;
        LoadBlenderPath();
    }

    /// <summary>
    /// When the page is navigated to.
    /// </summary>
    public void OnNavigatedTo()
    {
        LoadBlenderPath();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // HELPER FUNCTIONS
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Loads the Blender path from memory.
    /// </summary>
    private void LoadBlenderPath()
    {
        // Check if the Blender path is not empty
        if (DataManager.BlenderPath != String.Empty)
        {
            // Set the Blender path
            _blenderFilePathSelector.SetPath(DataManager.BlenderPath);
            // Mark the path as valid
            _blenderFilePathSelector.MarkValid();
        }
    }
}
