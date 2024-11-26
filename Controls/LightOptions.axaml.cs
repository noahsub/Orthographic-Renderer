using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using AvaloniaColorPicker;
using Orthographic.Renderer.Managers;

namespace Orthographic.Renderer.Controls;

public partial class LightOptions : UserControl
{
    public LightOptions()
    {
        InitializeComponent();
        ComputeSliderBounds();
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
                DistanceValueSelector.SetSliderBounds(0, (Math.Floor(maxDimension) * 5) * DataManager.UnitScale, 0.2);
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
}