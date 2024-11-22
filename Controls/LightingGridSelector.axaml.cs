using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Orthographic.Renderer.Controls;

public partial class LightingGridSelector : UserControl
{
    public LightingGridSelector()
    {
        InitializeComponent();
    }
    
    public void SetTitle(string title)
    {
        // Set the title of the grid
        // This is a placeholder method
        TitleLabel.Content = title;
    }
}