using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media;

namespace Orthographic.Renderer.Controls;

public partial class ViewSelection : UserControl
{
    public bool IsSelected { get; set; }

    public ViewSelection()
    {
        InitializeComponent();
    }
}
