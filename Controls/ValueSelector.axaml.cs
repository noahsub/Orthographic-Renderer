using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml;

namespace Orthographic.Renderer.Controls;

public partial class ValueSelector : UserControl
{
    public ValueSelector()
    {
        InitializeComponent();
    }

    public void SetUnit(string unit)
    {
        UnitLabel.Content = unit;
    }

    private void ValueSlider_OnValueChanged(object? sender, RangeBaseValueChangedEventArgs e)
    {
        ValueTextBox.Text = e.NewValue.ToString();
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
}
