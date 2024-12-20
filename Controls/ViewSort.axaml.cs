﻿////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// ViewSort.axaml.cs
// This file contains the logic for the ViewSort control.
//
// Copyright (C) 2024 noahsub
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// IMPORTS
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// NAMESPACE
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Orthographic.Renderer.Controls;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// VIEW SORT OPTIONS CLASS
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

/// <summary>
/// Selection of faces to sort by.
/// </summary>
public partial class ViewSort : UserControl
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // INITIALIZATION
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Initializes a new instance of the <see cref="ViewSort"/> class.
    /// </summary>
    public ViewSort()
    {
        InitializeComponent();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // SELECTION
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Get the selected faces.
    /// </summary>
    /// <returns>A list of selected faces.</returns>
    public List<string> GetSelectedFaces()
    {
        // Create a list to store the selected faces
        var selectedFaces = new List<string>();

        // Iterate through each control in the stack panel
        foreach (var control in FacesStackPanel.Children)
        {
            // If the button is checked, add the face to the list
            var button = (ToggleButton)control;
            if (button.IsChecked == true)
            {
                selectedFaces.Add(button.Content?.ToString()?.ToLower() ?? string.Empty);
            }
        }

        // Return the list of selected faces
        return selectedFaces;
    }
}
