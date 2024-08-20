using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media;

namespace Orthographic.Renderer.Controls;

public partial class RenderViewControl : UserControl
{
    public bool IsSelected { get; set; }
    
    public RenderViewControl()
    {
        InitializeComponent();
    }
}