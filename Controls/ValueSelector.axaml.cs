using System;
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Orthographic.Renderer.Controls;

public partial class ValueSelector : UserControl
{
    public event EventHandler ValueChanged;

    public ValueSelector()
    {
        InitializeComponent();
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

    private void SliderPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        ValueChanged?.Invoke(this, e);
    }

    private void SliderPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        return;
    }

    public void SetUnit(string unit)
    {
        UnitLabel.Content = unit;
    }
    
    public float GetValue()
    {
        if (float.TryParse(ValueTextBox.Text, out var value))
        {
            return value;
        }

        return 0;
    }

    private void ValueSlider_OnValueChanged(object? sender, RangeBaseValueChangedEventArgs e)
    {
        ValueTextBox.Text = e.NewValue.ToString("F2");
    }

    private void ValueTextBox_OnTextChanging(object? sender, TextChangingEventArgs e)
    {
        if (double.TryParse(ValueTextBox.Text, out var value))
        {
            ValueSlider.Value = value;
        }
        else
        {
            ValueSlider.Value = 0;
            ValueTextBox.Text = "0";
        }
    }

    public void SetSliderBounds(double min, double max, double increment)
    {
        ValueSlider.Minimum = min;
        ValueSlider.Maximum = max;
        ValueSlider.SmallChange = increment;
    }

    public void SetValue(double value)
    {
        ValueSlider.Value = value;
        ValueTextBox.Text = value.ToString();
    }

    private void ValueSlider_OnPointerEntered(object? sender, PointerEventArgs e)
    {
        Debug.WriteLine("Pointer entered");
    }

    private void ValueTextBox_OnKeyUp(object? sender, KeyEventArgs e)
    {
        // check if value is a double
        if (double.TryParse(ValueTextBox.Text, out var value))
        {
            ValueChanged?.Invoke(this, e);
        }
    }
}
