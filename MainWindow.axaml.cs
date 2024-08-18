using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Orthographic.Renderer.Pages;

namespace Orthographic.Renderer;

public partial class MainWindow : Window
{
    private static Window Self { get; set; }

    public MainWindow()
    {
        InitializeComponent();
        Self = this;
        PageContent.Content = new RequirementsPage();
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