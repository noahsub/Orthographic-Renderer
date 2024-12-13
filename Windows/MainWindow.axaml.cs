////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// MainWindow.axaml.cs
// This file contains the logic for the MainWindow.
//
// Copyright (C) 2024 noahsub
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// IMPORTS
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using Avalonia.Controls;
using Avalonia.Input;
using Orthographic.Renderer.Managers;
using Orthographic.Renderer.Pages;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// NAMESPACE
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Orthographic.Renderer.Windows;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// MAIN WINDOW CLASS
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

/// <summary>
/// Represents the main window of the application.
/// </summary>
public partial class MainWindow : Window
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // INITIALIZATION
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();
        // Add the window to the list of open windows
        WindowManager.AddWindow(this);

        // If an update is available, show the update page
        if (
            new Version(DataManager.LatestVersion).CompareTo(
                new Version(DataManager.CurrentVersion)
            ) > 0
        )
        {
            // Set the content to the update page
            PageContent.Content = new UpdatePage();
        }
        // Otherwise, show the requirements page
        else
        {
            // Set the content to the requirements page
            // PageContent.Content = new RequirementsPage();
            // Switch to the RequirementsPage
            var mainWindow = (MainWindow)this.VisualRoot!;
            NavigationManager.SwitchPage(mainWindow, "RequirementsPage");
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // EVENTS
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// If the user clicks on the top of the window, move the window.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void InputElement_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        // if cursor is on within the first 30 pixels of the window, move the window
        if (e.GetCurrentPoint(this).Position.Y < 30)
        {
            BeginMoveDrag(e);
        }
    }

    /// <summary>
    /// Removes the window from the list of open windows when it is closed.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void TopLevel_OnClosed(object? sender, EventArgs e)
    {
        // Remove the window from the list of open windows
        WindowManager.RemoveWindow(this);
    }
}
