////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// FilePathSelector.axaml.cs
// This file contains the logic for the FilePathSelector control.
//
// Copyright (C) 2024 noahsub
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// IMPORTS
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using Mono.Unix;
using Orthographic.Renderer.Constants;
using Orthographic.Renderer.Interfaces;
using Orthographic.Renderer.Managers;
using Orthographic.Renderer.Pages;
using Path = System.IO.Path;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// NAMESPACE
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Orthographic.Renderer.Controls;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// BROWSABLE FILE TEXT BOX CLASS
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

/// <summary>
/// A text box that allows the user to browse for a file.
/// </summary>
public partial class FilePathSelector : UserControl, IPathSelector
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // INITIALIZATION
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Initializes a new instance of the <see cref="FilePathSelector"/> class.
    /// </summary>
    public FilePathSelector()
    {
        InitializeComponent();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // EVENTS
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Allows the user to browse for a file when the browse button is clicked.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void BrowseButton_OnClick(object? sender, RoutedEventArgs e)
    {
        // Get the top level window
        var topLevel = TopLevel.GetTopLevel(this);

        // Open the file picker
        var file = await topLevel.StorageProvider.OpenFilePickerAsync(
            new FilePickerOpenOptions
            {
                Title = "Select a file",
                AllowMultiple = false,
                // Filter the file types
                FileTypeFilter = new List<FilePickerFileType>
                {
                    new("All Files") { Patterns = new[] { "*" } },
                    new("Model Files")
                    {
                        Patterns = new[]
                        {
                            "*.blend",
                            "*.obj",
                            "*.stl",
                            "*.BLEND",
                            "*.OBJ",
                            "*.STL",
                        },
                    },
                    new("Blender Files") { Patterns = new[] { "*.blend", "*.BLEND" } },
                    new("OBJ Files") { Patterns = new[] { "*.obj", "*.OBJ" } },
                    new("STL Files") { Patterns = new[] { "*.stl", "*.STL" } },
                },
            }
        );

        // If no file was selected, return
        if (file.Count < 1)
        {
            return;
        }

        // Set the path text box to the selected file
        var path = FileManager.ReformatPath(file[0].Path.AbsolutePath);
        SetPath(path);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // IPATHSELECTOR IMPLEMENTATION
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Fix the path in the text box.
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
    /// Check if the path in the text box is valid.
    /// </summary>
    /// <returns></returns>
    public bool CheckPath()
    {
        // Get the path from the text box
        var path = GetPath();
        
        // Check if the path is valid
        var isValid = FileManager.VerifyFilePath(path);

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

    /// <summary>
    /// Check if the path in the text box is valid.
    /// </summary>
    /// <param name="fileType">The type of file to check.</param>
    /// <returns></returns>
    public bool CheckPath(FileType fileType)
    {
        // Get the path
        var path = GetPath();
        
        // Verify the file path
        if (!FileManager.VerifyFilePath(path))
        {
            MarkInvalid();
            return false;
        }

        // Check if the path requires elevated permissions
        if (FileManager.ElevatedPath(path))
        {
            MarkInvalid();
            DialogManager.ShowElevatedPermissionsWarningDialog();
            return false;
        }

        // If the file type is executable, check if the path is valid
        if (fileType == FileType.Executable)
        {
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(path).ToLower();
            if (fileNameWithoutExtension == "blender")
            {
                if (!FileManager.VerifyBlenderPath(path))
                {
                    MarkInvalid();
                    return false;
                }
            }
            else if (!FileManager.VerifyProgramPath(fileNameWithoutExtension, path))
            {
                MarkInvalid();
                return false;
            }
        }
        
        // If the file type is model, check if the path is valid
        else if (fileType == FileType.Model && !FileManager.VerifyModelPath(path))
        {
            MarkInvalid();
            return false;
        }

        // Mark the path as valid and return true
        MarkValid();
        return true;
    }

    /// <summary>
    /// Mark the path as valid.
    /// </summary>
    public void MarkValid()
    {
        // Set the border color to green
        PathTextBox.BorderBrush = Brushes.MediumSpringGreen;
    }

    /// <summary>
    /// Mark the path as invalid.
    /// </summary>
    public void MarkInvalid()
    {
        // Set the border color to red
        PathTextBox.BorderBrush = Brushes.IndianRed;
        
        // Play the error sound
        SoundManager.PlayErrorSound();
    }

    /// <summary>
    /// Get the path from the text box.
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
    /// Set the path in the text box.
    /// </summary>
    /// <param name="path">The path to set.</param>q
    public void SetPath(string path)
    {
        // Set the text box to the path
        PathTextBox.Text = path;
    }
}
