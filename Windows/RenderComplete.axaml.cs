using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Orthographic.Renderer.Windows;

public partial class RenderComplete : Window
{
    public RenderComplete()
    {
        InitializeComponent();
    }
    
    public void SetRenderTime(string time)
    {
        TimeLabel.Content = time;
    }

    private void CloseButton_OnClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}