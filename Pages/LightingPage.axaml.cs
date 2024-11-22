using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Orthographic.Renderer.Managers;
using Orthographic.Renderer.Windows;

namespace Orthographic.Renderer.Pages;

public partial class LightingPage : UserControl
{
    public LightingPage()
    {
        InitializeComponent();
        TopLightingGridSelector.SetTitle("Top");
        BottomLightingGridSelector.SetTitle("Bottom");
        FrontLightingGridSelector.SetTitle("Front");
        BackLightingGridSelector.SetTitle("Back");
        LeftLightingGridSelector.SetTitle("Left");
        RightLightingGridSelector.SetTitle("Right");
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
}