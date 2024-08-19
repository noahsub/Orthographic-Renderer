using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media;

namespace Orthographic.Renderer.Controls;

public partial class RenderSelectionControl : UserControl
{
    public bool IsSelected { get; set; }
    
    public RenderSelectionControl()
    {
        InitializeComponent();
    }

    private void Grid_OnTapped(object? sender, TappedEventArgs e)
    {
        if (IsSelected)
        {
            IsSelected = false;
            MainGrid.Background = new SolidColorBrush(Color.Parse("#1a1b1e"));
        }
        else
        {
            IsSelected = true;
            MainGrid.Background = new SolidColorBrush(Color.Parse("#1d965d"));
        }
    }
}