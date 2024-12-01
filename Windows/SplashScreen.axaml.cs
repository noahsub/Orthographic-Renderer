﻿////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// SplashScreen.axaml.cs
// This file contains the logic for the SplashScreen.
//
// Copyright (C) 2024 noahsub
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// IMPORTS
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using Orthographic.Renderer.Managers;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// NAMESPACE
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Orthographic.Renderer.Windows;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// SPLASH SCREEN CLASS
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

/// <summary>
/// Represents the splash screen of the application.
/// </summary>
public partial class SplashScreen : Window
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // INITIALIZATION
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Initializes a new instance of the <see cref="SplashScreen"/> class.
    /// </summary>
    public SplashScreen()
    {
        InitializeComponent();
        // Set the image
        var images = new List<string>
        {
            "Assets/Images/Backgrounds/computer.png", 
            "Assets/Images/Backgrounds/cubes.png",
            "Assets/Images/Backgrounds/motor.png"
        };
        
        var random = new Random();
        var image = images[random.Next(images.Count)];
        LoadingImage.Source = new Bitmap(image);
        // Set the version
        VersionLabel.Content = $"v{DataManager.CurrentVersion}";
        // Add the window to the list of open windows
        WindowManager.AddWindow(this);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // EVENTS
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

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

    public void SetLoadingText(string text)
    {
        LoadingLabel.Content = text;
    }

    public void SetLoadingTextUiThread(string text)
    {
        Dispatcher.UIThread.Post(() =>
        {
            LoadingLabel.Content = text;
        });
    }
}
