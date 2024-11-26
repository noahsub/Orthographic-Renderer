using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using AvaloniaColorPicker;

namespace Orthographic.Renderer.Controls;

public partial class ColourSelector : UserControl
{
    public ColourSelector()
    {
        InitializeComponent();
        ColourLabel.Content = ColourPicker.Color.ToString();
    }

    private void LightColourPicker_OnPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == ColorButton.ColorProperty)
        {
            ColourLabel.Content = ColourPicker.Color.ToString().ToUpper();
            ColourRectangle.Fill = new SolidColorBrush(ColourPicker.Color);
        }
    }
}