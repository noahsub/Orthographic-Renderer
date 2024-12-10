﻿////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// DirectoryPathSelector.axaml.cs
// This file contains the logic for the DirectoryPathSelector control.
//
// Copyright (C) 2024 noahsub
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// IMPORTS
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using Orthographic.Renderer.Interfaces;
using Orthographic.Renderer.Managers;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// NAMESPACE
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Orthographic.Renderer.Controls;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// BROWSABLE DIRECTORY TEXT BOX CLASS
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

/// <summary>
/// A text box that allows the user to browse for a directory.
/// </summary>
public partial class DirectoryPathSelector : UserControl, IPathSelector
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // INITIALIZATION
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Initializes a new instance of the <see cref="DirectoryPathSelector"/> class.
    /// </summary>
    public DirectoryPathSelector()
    {
        InitializeComponent();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // EVENTS
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Allows the user to browse for a directory when the browse button is clicked.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void BrowseButton_OnClick(object? sender, RoutedEventArgs e)
    {
        // Get the top level window
        var topLevel = TopLevel.GetTopLevel(this);

        // Open the folder picker
        var directory = await topLevel.StorageProvider.OpenFolderPickerAsync(
            new FolderPickerOpenOptions { Title = "Select a Directory" }
        );

        // If no directory was selected, return
        if (directory.Count < 1)
        {
            return;
        }

        // Fix the path
        FixPath();
        // Check the path
        CheckPath();
    }
    
    private void PathTextBox_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        // Set the border color to transparent
        PathTextBox.BorderBrush = Brushes.Transparent;
        
        // Fix the path
        FixPath();
        
        // Check the path
        CheckPath();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // IPATHSELECTOR IMPLEMENTATION
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public void FixPath()
    {
        // Get the path from the text box
        var path = PathTextBox.Text;
        
        // Check if path is null or empty
        if (string.IsNullOrEmpty(path))
        {
            return;
        }
        
        // Reformat the path
        var reformattedPath = FileManager.ReformatPath(path);
        
        // Set the text box to the reformatted path
        SetPath(reformattedPath);
    }

    public bool CheckPath()
    {
        // Get the path from the text box
        var path = GetPath();
        
        // Check if the path is valid
        var isValid = FileManager.VerifyDirectoryPath(path);

        // If it is not valid, mark it as invalid
        if (!isValid)
        {
            MarkInvalid();
            return false;
        }
        
        // Check if the path requires elevated permissions
        var requiresElevation = FileManager.ElevatedPath(path);
        
        // If it does, mark it as invalid
        if (requiresElevation)
        {
            // Mark the path as invalid
            MarkInvalid();
            
            // Show the elevation required dialog
            DialogManager.ShowElevatedPermissionsWarningDialog();
            
            return false;
        }
        
        // Mark the path as valid
        MarkValid();
        return true;
    }

    public void MarkValid()
    {
        // Set the border color to green
        PathTextBox.BorderBrush = Brushes.MediumSpringGreen;
    }

    public void MarkInvalid()
    {
        // Set the border color to red
        PathTextBox.BorderBrush = Brushes.IndianRed;
        
        // Play the error sound
        SoundManager.PlayErrorSound();
    }

    public string GetPath()
    {
        // Get the path from the text box
        var path = PathTextBox.Text;
        
        // Check if path is null or empty, if it is, then return empty string
        if (string.IsNullOrEmpty(path))
        {
            return string.Empty;
        }

        // Return the path
        return path;
    }

    public void SetPath(string path)
    {
        // Set the text box to the path
        PathTextBox.Text = path;
    }
}
