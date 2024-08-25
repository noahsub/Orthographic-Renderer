using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Orthographic.Renderer.Entities;
using Orthographic.Renderer.Managers;

namespace Orthographic.Renderer.Pages;

public partial class ModelPage : UserControl
{
    private static readonly List<string> ValidTypes = [".blend", ".obj", ".stl"];

    public ModelPage()
    {
        InitializeComponent();
        LoadRecentFiles();
        UnitComboBox.SelectedIndex = 0;
    }

    private void NextButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var modelPath = ModelPathTextBox.PathTextBox.Text;

        if (modelPath == null)
        {
            ModelPathTextBox.PathTextBox.BorderBrush = new SolidColorBrush(Colors.Red);
            SoundManager.PlaySound("Assets/Sounds/error.mp3");
            return;
        }
        
        modelPath = FileManager.ReformatPath(modelPath);

        if (!IsValidModelPath(modelPath))
        {
            ModelPathTextBox.PathTextBox.BorderBrush = new SolidColorBrush(Colors.Red);
            SoundManager.PlaySound("Assets/Sounds/error.mp3");
            return;
        }

        ModelPathTextBox.PathTextBox.BorderBrush = new SolidColorBrush(Colors.MediumSpringGreen);
        DataManager.ModelPath = modelPath;
        UpdateRecentFiles(modelPath);

        var mainWindow = (Windows.MainWindow)this.VisualRoot!;
        NavigationManager.SwitchPage(mainWindow, new RenderPage());
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

    private void RecentlyOpenedComboBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        var selectedText = RecentlyOpenedComboBox.SelectedItem?.ToString();
        ModelPathTextBox.PathTextBox.Text = selectedText;
    }

    private void UnitComboBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        var comboBoxItem = (ComboBoxItem)UnitComboBox.SelectedItem;
        var selectedText = comboBoxItem.Content.ToString().ToLower();
        
        Debug.WriteLine($"SELECTED TEXT: {selectedText}");

        switch (selectedText)
        {
            case "millimeters":
                DataManager.UnitScale = ModelUnit.Millimeter;
                break;
            case "centimeters":
                DataManager.UnitScale = ModelUnit.Centimeter;
                break;
            case "meters":
                DataManager.UnitScale = ModelUnit.Meter;
                break;
            case "inches":
                DataManager.UnitScale = ModelUnit.Inch;
                break;
            case "feet":
                DataManager.UnitScale = ModelUnit.Foot;
                break;
        }
    }

    private void BackButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var mainWindow = (Windows.MainWindow)this.VisualRoot!;
        NavigationManager.SwitchPage(mainWindow, new RequirementsPage());
    }
}
