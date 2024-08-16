using Avalonia.Controls;
using Orthographic.Renderer.Pages;

namespace Orthographic.Renderer;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        PageContent.Content = new RequirementsPage();
    }
}