﻿////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// ViewSelectOptions.axaml.cs
// This file contains the logic for the ViewSelectOptions control.
//
// Author(s): https://github.com/noahsub
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// IMPORTS
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using Avalonia.Controls;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// NAMESPACE
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Orthographic.Renderer.Controls;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// VIEW SELECT OPTIONS CLASS
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

/// <summary>
/// A control that allows the user to select all, none, or invert the selection of a list of items.
/// </summary>
public partial class ViewSelectOptions : UserControl
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // INITIALIZATION
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Initializes a new instance of the <see cref="ViewSelectOptions"/> class.
    /// </summary>
    public ViewSelectOptions()
    {
        InitializeComponent();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // CLEANUP
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Clears the selection of the buttons.
    /// </summary>
    public void ClearSelection()
    {
        AllButton.IsChecked = false;
        NoneButton.IsChecked = false;
        InvertButton.IsChecked = false;
    }
}
