﻿////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// LightSetupItem.axaml.cs
// This file contains the logic for the LightSetupItem control.
//
// Copyright (C) 2024 noahsub
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// IMPORTS
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using AvaloniaColorPicker;
using Orthographic.Renderer.Managers;
using ColorPicker = Avalonia.Controls.ColorPicker;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// NAMESPACE
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Orthographic.Renderer.Controls;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// LIGHT OPTIONS CLASS
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


/// <summary>
/// A control that allows for the configuration of a blender area light.
/// </summary>
public partial class LightSetupItem : UserControl
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // INITIALIZATION
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Initializes a new instance of the <see cref="LightSetupItem"/> class.
    /// </summary>
    public LightSetupItem()
    {
        InitializeComponent();

        // Set the units of the value selectors
        PowerValueSelector.SetUnit("W");
        SizeValueSelector.SetUnit("m");
        DistanceValueSelector.SetUnit("m");

        // Set the slider bounds of the value selectors
        PowerValueSelector.SetSliderBounds(0, 2000, 50);
        SizeValueSelector.SetSliderBounds(0, 10, 0.2);
        DistanceValueSelector.SetSliderBounds(0, 20, 1);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // SETTERS
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Sets the orientation of the light.
    /// </summary>
    /// <param name="orientation"></param>
    public void SetOrientation(string orientation)
    {
        LightOrientationSelector.SetOrientation(orientation);
    }

    /// <summary>
    /// Set the colour of the light.
    /// </summary>
    /// <param name="colour"></param>
    public void SetColour(Color colour)
    {
        LightColourSelector.ColourPicker.Color = colour;
    }

    /// <summary>
    /// Set the power of the light.
    /// </summary>
    /// <param name="power"></param>
    public void SetPower(double power)
    {
        PowerValueSelector.SetValue(power);
    }

    /// <summary>
    /// Set the size of the light.
    /// </summary>
    /// <param name="size"></param>
    public void SetSize(double size)
    {
        SizeValueSelector.SetValue(size);
    }

    /// <summary>
    /// Set the distance of the light.
    /// </summary>
    /// <param name="distance"></param>
    public void SetDistance(double distance)
    {
        DistanceValueSelector.SetValue(distance);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // EVENTS
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// When the remove button is clicked, remove the control from the parent.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void RemoveButton_OnClick(object? sender, RoutedEventArgs e)
    {
        // Remove the entire control from the parent
        if (Parent is Panel parentPanel)
        {
            parentPanel.Children.Remove(this);
        }
    }
}
