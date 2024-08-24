using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Orthographic.Renderer.Pages;

namespace Orthographic.Renderer.Windows;

public partial class MainWindow : Window
{
    private static Window Self { get; set; }
    public static bool IsPointerPressed { get; set; }

    public MainWindow()
    {
        InitializeComponent();
        Self = this;
        this.PointerPressed += OnPointerPressed;
        this.PointerReleased += OnPointerReleased;
        PageContent.Content = new RequirementsPage();
    }

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        IsPointerPressed = true;
    }

    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        IsPointerPressed = false;
    }

    public static Size? GetSize()
    {
        return Self.FrameSize;
    }

    public static PixelPoint GetPosition()
    {
        return Self.Position;
    }
}
