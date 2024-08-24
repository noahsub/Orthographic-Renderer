using System.Collections.Generic;
using System.IO;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Orthographic.Renderer.Managers;

namespace Orthographic.Renderer.Pages;

public partial class ModelPage : UserControl
{
    private static readonly List<string> ValidTypes = [".blend", ".obj", ".3mf", ".stl"];

    public ModelPage()
    {
        InitializeComponent();
        LoadRecentFiles();
    }

    private void NextButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var modelPath = ModelPathTextBox.PathTextBox.Text;

        if (modelPath == null)
        {
            ModelPathTextBox.PathTextBox.BorderBrush = new SolidColorBrush(Colors.Red);
            return;
        }

        if (!IsValidModelPath(modelPath))
        {
            ModelPathTextBox.PathTextBox.BorderBrush = new SolidColorBrush(Colors.Red);
            return;
        }

        ModelPathTextBox.PathTextBox.BorderBrush = new SolidColorBrush(Colors.MediumSpringGreen);
        modelPath = FileManager.ReformatPath(modelPath);
        DataManager.ModelPath = modelPath;
        UpdateRecentFiles(modelPath);

        var mainWindow = (MainWindow)this.VisualRoot!;
        var pageContent = mainWindow.FindControl<ContentControl>("PageContent");

        if (pageContent != null)
        {
            pageContent.Content = new RenderPage();
        }
    }

    private static bool IsValidModelPath(string modelPath)
    {
        if (string.IsNullOrEmpty(modelPath))
        {
            return false;
        }

        return File.Exists(modelPath) && ValidTypes.Contains(Path.GetExtension(modelPath));
    }

    private static void UpdateRecentFiles(string modelPath)
    {
        var paths = FileManager.ReadJsonArray("Data/recent_models.json", "paths");

        paths.Remove(modelPath);
        paths.Insert(0, modelPath);

        FileManager.WriteArrayToJsonFile("Data/recent_models.json", "paths", paths);
    }

    private void LoadRecentFiles()
    {
        var paths = FileManager.ReadJsonArray("Data/recent_models.json", "paths");
        foreach (var path in paths)
        {
            RecentlyOpenedComboBox.Items.Add(path);
        }
    }

    private void RecentlyOpenedComboBox_OnSelectionChanged(
        object? sender,
        SelectionChangedEventArgs e
    )
    {
        var selectedText = RecentlyOpenedComboBox.SelectedItem?.ToString();
        ModelPathTextBox.PathTextBox.Text = selectedText;
    }
}
