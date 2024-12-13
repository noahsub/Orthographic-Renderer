////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
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
        if (topLevel != null)
        {
            var directory = await topLevel.StorageProvider.OpenFolderPickerAsync(
                new FolderPickerOpenOptions { Title = "Select a Directory" }
            );

            // If no directory was selected, return
            if (directory.Count < 1)
            {
                return;
            }

            // Set the path text box to the selected directory
            var path = directory[0].Path.AbsolutePath;
            SetPath(path);
        }

        FixPath();
    }

    /// <summary>
    /// Detects when the text in the path text box is changed.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
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

    /// <summary>
    /// Fixes the path in the text box.
    /// </summary>
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

    /// <summary>
    /// Checks if the path in the text box is valid.
    /// </summary>
    /// <returns></returns>
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

        // Mark the path as valid
        MarkValid();
        return true;
    }

    /// <summary>
    /// Marks the path as valid.
    /// </summary>
    public void MarkValid()
    {
        // Set the border color to green
        PathTextBox.BorderBrush = Brushes.MediumSpringGreen;
    }

    /// <summary>
    /// Marks the path as invalid.
    /// </summary>
    public void MarkInvalid()
    {
        // Set the border color to red
        PathTextBox.BorderBrush = Brushes.IndianRed;

        // Play the error sound
        SoundManager.PlayErrorSound();
    }

    /// <summary>
    /// Gets the path from the text box.
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Sets the path in the text box.
    /// </summary>
    /// <param name="path">The path to set.</param>
    public void SetPath(string path)
    {
        // Set the text box to the path
        PathTextBox.Text = path;
    }
}
