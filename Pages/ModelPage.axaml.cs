using System.Collections.Generic;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Orthographic.Renderer.Managers;

namespace Orthographic.Renderer.Pages;

public partial class ModelPage : UserControl
{
    public ModelPage()
    {
        InitializeComponent();
    }

    private void NextButton_OnClick(object? sender, RoutedEventArgs e)
    {
        ModelPathTextBox.PathTextBox.Text = FileManager.ReformatPath(ModelPathTextBox.PathTextBox.Text);
        var validTypes = new List<string> { ".blend", ".obj", ".3mf", ".stl"};
        
        if (!File.Exists(ModelPathTextBox.PathTextBox.Text))
        {
            ModelPathTextBox.PathTextBox.BorderBrush = new SolidColorBrush(Colors.Red);
        }
        
        else if (!validTypes.Contains(Path.GetExtension(ModelPathTextBox.PathTextBox.Text)))
        {
            ModelPathTextBox.PathTextBox.BorderBrush = new SolidColorBrush(Colors.Red);
        }
        
        else
        {
            ModelPathTextBox.PathTextBox.BorderBrush = new SolidColorBrush(Colors.MediumSpringGreen);
            // Get the ContentControl called "PageContent" from the MainWindow
            var mainWindow = (MainWindow) this.VisualRoot;
            var pageContent = mainWindow.FindControl<ContentControl>("PageContent");
            pageContent.Content = new RenderPage();
        }
    }
}