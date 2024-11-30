using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using AvaloniaColorPicker;
using ColorPicker = Avalonia.Controls.ColorPicker;

namespace Orthographic.Renderer.Controls;

public partial class ColourSelector : UserControl
{
    public event EventHandler ColourChanged;

    public ColourSelector()
    {
        InitializeComponent();
        ColourLabel.Content = ColourPicker.Color.ToString();
    }

    private void LightColourPicker_OnPropertyChanged(
        object? sender,
        AvaloniaPropertyChangedEventArgs e
    )
    {
        if (e.Property == ColorButton.ColorProperty)
        {
            ColourChanged?.Invoke(this, e);
            ColourLabel.Content = ColourPicker.Color.ToString().ToUpper();
            ColourRectangle.Fill = new SolidColorBrush(ColourPicker.Color);
        }
    }

    public string GetHexColour()
    {
        int r = ColourPicker.Color.R;
        int g = ColourPicker.Color.G;
        int b = ColourPicker.Color.B;
        int a = ColourPicker.Color.A;
        return $"#{r:X2}{g:X2}{b:X2}{a:X2}";
    }
}
