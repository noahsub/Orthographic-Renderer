////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// LightOptions.axaml.cs
// This file contains the logic for the LightOptions control.
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
public partial class LightOptions : UserControl
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // INITIALIZATION
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Initializes a new instance of the <see cref="LightOptions"/> class.
    /// </summary>
    public LightOptions()
    {
        InitializeComponent();

        // Compute the slider bounds based on the size of the model.
        ComputeSliderBounds();
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
    // SLIDER BOUNDS
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Compute the bounds of the sliders based on the size of the model.
    /// </summary>
    private void ComputeSliderBounds()
    {
        // Set the bounds of the power and size sliders, which are model independent.
        PowerValueSelector.SetSliderBounds(0, 2000, 50);
        SizeValueSelector.SetSliderBounds(0, 100, 1);

        // If the model path is invalid, return
        if (!FileManager.VerifyModelPath(DataManager.ModelPath))
            return;

        // Get the dimensions of the model
        var dimensions = ModelManager.GetDimensions(DataManager.ModelPath);
        // Get the biggest dimension
        var maxDimension = new[] { dimensions.X, dimensions.Y, dimensions.Z }.Max();

        // If the model has no dimensions, set the distance slider bounds to [0, 100] with a step of 0.2
        if (maxDimension == 0)
        {
            DistanceValueSelector.SetSliderBounds(0, 100, 0.2);
        }
        // otherwise, set the distance slider bounds to [0, maxDimension * 200] with a step of 0.2
        else
        {
            DistanceValueSelector.SetSliderBounds(
                0,
                (Math.Floor(maxDimension) * 200) * DataManager.UnitScale,
                0.2
            );
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // VERIFICATION
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Verifies that the options are valid.
    /// </summary>
    public void VerifyOptions()
    {
        // If the power value is invalid, set it to 0
        if (
            string.IsNullOrEmpty(PowerValueSelector.ValueTextBox.Text)
            || float.TryParse(PowerValueSelector.ValueTextBox.Text, out _) == false
        )
        {
            PowerValueSelector.SetValue(0);
        }

        // If the size value is invalid, set it to 0
        if (
            string.IsNullOrEmpty(SizeValueSelector.ValueTextBox.Text)
            || float.TryParse(SizeValueSelector.ValueTextBox.Text, out _) == false
        )
        {
            SizeValueSelector.SetValue(0);
        }

        // If the distance value is invalid, set it to 0
        if (
            string.IsNullOrEmpty(DistanceValueSelector.ValueTextBox.Text)
            || float.TryParse(DistanceValueSelector.ValueTextBox.Text, out _) == false
        )
        {
            DistanceValueSelector.SetValue(0);
        }
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
