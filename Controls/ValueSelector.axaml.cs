////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// ViewSelector.xaml.cs
// This file contains the logic for the ValueSelector control.
//
// Copyright (C) 2024 noahsub
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// IMPORTS
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Diagnostics;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// NAMESPACE
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Orthographic.Renderer.Controls;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// VALUE SELECTOR CLASS
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

/// <summary>
/// Selects a value using a slider and text box.
/// </summary>
public partial class ValueSelector : UserControl
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // GLOBALS
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Event handler for when the value is changed.
    /// </summary>
    public event EventHandler ValueChanged = delegate { };

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // INITIALIZATION
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Creates a new instance of the <see cref="ValueSelector"/> class.
    /// </summary>
    public ValueSelector()
    {
        // Create the component
        InitializeComponent();

        // Add event handlers
        ValueSlider.AddHandler(
            InputElement.PointerPressedEvent,
            SliderPointerPressed,
            RoutingStrategies.Tunnel
        );
        ValueSlider.AddHandler(
            InputElement.PointerReleasedEvent,
            SliderPointerReleased,
            RoutingStrategies.Tunnel
        );
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // EVENT HANDLERS
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// When the slider is released
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void SliderPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        ValueChanged(this, e);
    }

    /// <summary>
    /// When the slider is pressed
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void SliderPointerPressed(object? sender, PointerPressedEventArgs e)
    {
    }

    /// <summary>
    /// When the slider value is changed
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ValueSlider_OnValueChanged(object? sender, RangeBaseValueChangedEventArgs e)
    {
        ValueTextBox.Text = e.NewValue.ToString("F2");
    }

    /// <summary>
    /// When the text box value is changing
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ValueTextBox_OnTextChanging(object? sender, TextChangingEventArgs e)
    {
        if (double.TryParse(ValueTextBox.Text, out var value))
        {
            ValueSlider.Value = value;
        }
    }

    /// <summary>
    /// When a key is pressed in the value text box
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ValueTextBox_OnKeyUp(object? sender, KeyEventArgs e)
    {
        // check if value is a double
        if (double.TryParse(ValueTextBox.Text, out _))
        {
            ValueChanged(this, e);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // SETTERS
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Set the label of the value selector.
    /// </summary>
    /// <param name="unit"></param>
    public void SetUnit(string unit)
    {
        UnitLabel.Content = unit;
    }

    /// <summary>
    /// Set the bounds of the value slider.
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <param name="increment"></param>
    public void SetSliderBounds(double min, double max, double increment)
    {
        ValueSlider.Minimum = min;
        ValueSlider.Maximum = max;
        ValueSlider.SmallChange = increment;
    }

    /// <summary>
    /// Set the value of the value selector.
    /// </summary>
    /// <param name="value"></param>
    public void SetValue(double value)
    {
        ValueSlider.Value = value;
        ValueTextBox.Text = value.ToString(CultureInfo.InvariantCulture);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // GETTERS
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Get the value of the value selector.
    /// </summary>
    /// <returns></returns>
    public float GetValue()
    {
        // attempt to parse the value as a float and return it
        if (float.TryParse(ValueTextBox.Text, out var value))
        {
            return value;
        }

        // set value to 0
        SetValue(0);
        return 0;
    }
}
