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
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Orthographic.Renderer.Managers;
using Orthographic.Renderer.Pages;

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
public partial class FilePathSelector : UserControl
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
        var path = file[0].Path.AbsolutePath.Replace("%20", " ");
        PathTextBox.Text = path;

        // If the current page is the model page
        if (NavigationManager.GetCurrentPage() is ModelPage modelPage)
        {
            // Set the dimensions of the model to unknown
            modelPage.SetDimensionsUnknown();
        }
    }
}
