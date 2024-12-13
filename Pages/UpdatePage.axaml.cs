﻿////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// UpdatePage.axaml.cs
// This file contains the logic for the UpdatePage.
//
// Copyright (C) 2024 noahsub
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// IMPORTS
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Orthographic.Renderer.Interfaces;
using Orthographic.Renderer.Managers;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// NAMESPACE
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Orthographic.Renderer.Pages;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// UPDATE PAGE CLASS
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

/// <summary>
/// Represents the update page of the application.
/// </summary>
public partial class UpdatePage : UserControl, IPage
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // INITIALIZATION
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdatePage"/> class.
    /// </summary>
    public UpdatePage()
    {
        Initialize();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // EVENTS
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// When the later button is clicked, switch to the requirements page.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void LaterButton_OnClick(object? sender, RoutedEventArgs e)
    {
        // Switch to the requirements page
        var mainWindow = (Windows.MainWindow)this.VisualRoot!;
        NavigationManager.SwitchPage(mainWindow, "RequirementsPage");
    }

    /// <summary>
    /// When the update button is clicked, open the latest release page and exit the application.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void UpdateButton_OnClick(object? sender, RoutedEventArgs e)
    {
        // Open the latest release page
        WebManager.OpenUrl("https://github.com/noahsub/Orthographic-Renderer/releases/latest");
        // Exit the application
        Environment.Exit(0);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // IPAGE INTERFACE IMPLEMENTATION
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    
    /// <summary>
    /// Initializes the update page.
    /// </summary>
    public void Initialize()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Called when the page is first loaded by the user.
    /// </summary>
    public void OnFirstLoad()
    {
    }

    /// <summary>
    /// Called when the page is navigated to.
    /// </summary>
    public void OnNavigatedTo()
    {
    }
}
