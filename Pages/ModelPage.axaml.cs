using System.Collections.Generic;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Newtonsoft.Json;
using Orthographic.Renderer.Managers;

namespace Orthographic.Renderer.Pages;

public partial class ModelPage : UserControl
{
    public ModelPage()
    {
        InitializeComponent();
        LoadRecentFiles();
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
            var modelPath = ModelPathTextBox.PathTextBox.Text;
            DataManager.ModelPath = modelPath;
            
            // Save the model path to the recent_models.json file
            var paths = FileManager.ReadJsonArray("Data/recent_models.json", "paths");
            
            if (!paths.Contains(modelPath))
            {
                paths.Insert(0, modelPath);
            }

            else
            {
                // bring the file to the front of the list
                paths.Remove(modelPath);
                paths.Insert(0, modelPath);
            }
            
            var json = new Dictionary<string, object>
            {
                { "paths", paths }
            };

            var jsonString = JsonConvert.SerializeObject(json, Formatting.Indented);
            File.WriteAllText("Data/recent_models.json", jsonString);
            
            pageContent.Content = new AdvancedRenderPage();
        }
    }

    private void LoadRecentFiles()
    {
        var paths = FileManager.ReadJsonArray("Data/recent_models.json", "paths");
        foreach (var path in paths)
        {
            RecentlyOpenedComboBox.Items.Add(path);
        }
    }

    private void RecentlyOpenedComboBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        var selectedText = RecentlyOpenedComboBox.SelectedItem.ToString();
        ModelPathTextBox.PathTextBox.Text = selectedText;
    }
}