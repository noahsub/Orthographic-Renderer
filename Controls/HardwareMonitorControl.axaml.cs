using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Orthographic.Renderer.Controls;

public partial class HardwareMonitorControl : UserControl
{
    public static float Value = 0;
    
    public HardwareMonitorControl()
    {
        InitializeComponent();
        DataContext = this;
    }
}