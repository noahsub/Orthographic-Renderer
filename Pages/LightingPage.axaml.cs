using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using AvaloniaColorPicker;
using Orthographic.Renderer.Controls;
using Orthographic.Renderer.Managers;
using Orthographic.Renderer.Windows;

namespace Orthographic.Renderer.Pages;

public partial class LightingPage : UserControl
{
    public LightingPage()
    {
        InitializeComponent();
    }

    private void BackButton_OnClick(object? sender, RoutedEventArgs e)
    {
        // Switch to the ModelPage
        var mainWindow = (MainWindow)this.VisualRoot!;
        NavigationManager.SwitchPage(mainWindow, "ModelPage");
    }

    private void ViewsButton_OnClick(object? sender, RoutedEventArgs e)
    {
        // Switch to the RenderPage
        var mainWindow = (MainWindow)this.VisualRoot!;
        NavigationManager.SwitchPage(mainWindow, "RenderPage");
    }

    private void BackgroundColourSelector_OnPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        // if (e.Property == ColourSelector.)
        // {
        //     BackgroundRectangle.Fill = new SolidColorBrush(BackgroundColourSelector.ColourPicker.Color);
        // }
    }
}