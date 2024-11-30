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

namespace Orthographic.Renderer.Controls;

public partial class LightOptions : UserControl
{
    public LightOptions()
    {
        InitializeComponent();
        ComputeSliderBounds();
    }
    
    public void SetOrientation(string orientation)
    {
        LightOrientationSelector.SetOrientation(orientation);
    }
    
    public void SetColour(Color colour)
    {
        LightColourSelector.ColourPicker.Color = colour;
    }
    
    public void SetPower(double power)
    {
        PowerValueSelector.SetValue(power);
    }
    
    public void SetSize(double size)
    {
        SizeValueSelector.SetValue(size);
    }
    
    public void SetDistance(double distance)
    {
        DistanceValueSelector.SetValue(distance);
    }

    private void ComputeSliderBounds()
    {
        PowerValueSelector.SetSliderBounds(0, 2000, 50);
        SizeValueSelector.SetSliderBounds(0, 100, 1);

        if (FileManager.VerifyModelPath(DataManager.ModelPath))
        {
            var dimensions = ModelManager.GetDimensions(DataManager.ModelPath);
            
            // Get the biggest dimension
            var maxDimension = new[] { dimensions.X, dimensions.Y, dimensions.Z }.Max();

            if (maxDimension == 0)
            {
                DistanceValueSelector.SetSliderBounds(0, 100, 0.2);
            }

            else
            {
                DistanceValueSelector.SetSliderBounds(0, (Math.Floor(maxDimension) * 200) * DataManager.UnitScale, 0.2);
            }
        }
    }

    private void RemoveButton_OnClick(object? sender, RoutedEventArgs e)
    {
        // Remove the entire control from the parent
        if (Parent is Panel parentPanel)
        {
            parentPanel.Children.Remove(this);
        }
    }

    public void VerifyOptions()
    {
        if (string.IsNullOrEmpty(PowerValueSelector.ValueTextBox.Text) || float.TryParse(PowerValueSelector.ValueTextBox.Text, out _) == false)
        {
            PowerValueSelector.SetValue(0);
        }
        
        if (string.IsNullOrEmpty(SizeValueSelector.ValueTextBox.Text) || float.TryParse(SizeValueSelector.ValueTextBox.Text, out _) == false)
        {
            SizeValueSelector.SetValue(0);
        }
        
        if (string.IsNullOrEmpty(DistanceValueSelector.ValueTextBox.Text) || float.TryParse(DistanceValueSelector.ValueTextBox.Text, out _) == false)
        {
            DistanceValueSelector.SetValue(0);
        }
    }
}