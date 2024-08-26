////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// ModelPage.axaml.cs
// This file contains the logic for the ModelPage.
//
// Author(s): https://github.com/noahsub
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// IMPORTS
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System.Collections.Generic;
using System.IO;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Orthographic.Renderer.Constants;
using Orthographic.Renderer.Managers;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// NAMESPACE
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Orthographic.Renderer.Pages;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// MODEL PAGE CLASS
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

/// <summary>
/// The ModelPage user control. Allows the user to select a model file.
/// </summary>
public partial class ModelPage : UserControl
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // CONSTANTS
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// List of valid file types for models.
    /// </summary>
    private static readonly List<string> ValidTypes = [".blend", ".obj", ".stl"];

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // INITIALIZATION
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Initializes a new instance of the <see cref="ModelPage"/> class.
    /// </summary>
    public ModelPage()
    {
        InitializeComponent();

        // Load the recently opened files.
        LoadRecentFiles();
        // Set the default unit to millimeters.
        UnitComboBox.SelectedIndex = 0;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // MODEL
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Checks if the model path is valid.
    /// </summary>
    /// <param name="modelPath">The model path to validate.</param>
    /// <returns>True if the model path is valid, otherwise, false.</returns>
    private static bool IsValidModelPath(string modelPath)
    {
        // Check if the model path is null or empty
        if (string.IsNullOrEmpty(modelPath))
        {
            return false;
        }

        // Check if the model path exists and is a valid file type
        return File.Exists(modelPath) && ValidTypes.Contains(Path.GetExtension(modelPath));
    }

    /// <summary>
    /// Updates the list of recent files.
    /// </summary>
    /// <param name="modelPath">The model path to add to the recent files.</param>
    private static void UpdateRecentFiles(string modelPath)
    {
        // Read all paths
        var paths = FileManager.ReadJsonArray("Data/recent_models.json", "paths");

        // Remove the model path if it already exists
        paths.Remove(modelPath);
        // Add the model path to the front of the list
        paths.Insert(0, modelPath);

        // Write the updated paths to the file
        FileManager.WriteArrayToJsonFile("Data/recent_models.json", "paths", paths);
    }

    /// <summary>
    /// Loads the recent files into the RecentlyOpenedComboBox.
    /// </summary>
    private void LoadRecentFiles()
    {
        // Read all paths
        var paths = FileManager.ReadJsonArray("Data/recent_models.json", "paths");

        // Add each path to the RecentlyOpenedComboBox
        foreach (var path in paths)
        {
            RecentlyOpenedComboBox.Items.Add(path);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // EVENT HANDLERS
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Handles the click event of the Next button.
    /// </summary>
    private void NextButton_OnClick(object? sender, RoutedEventArgs e)
    {
        // Get the model path
        var modelPath = ModelPathTextBox.PathTextBox.Text;

        // If the model path is null, set the border color to red and play an error sound
        if (modelPath == null)
        {
            ModelPathTextBox.PathTextBox.BorderBrush = new SolidColorBrush(Colors.Red);
            SoundManager.PlaySound("Assets/Sounds/error.mp3");
            return;
        }

        // Reformat the model path
        modelPath = FileManager.ReformatPath(modelPath);

        // If the model path is not valid, set the border color to red and play an error sound
        if (!IsValidModelPath(modelPath))
        {
            ModelPathTextBox.PathTextBox.BorderBrush = new SolidColorBrush(Colors.Red);
            SoundManager.PlaySound("Assets/Sounds/error.mp3");
            return;
        }

        // Set the border color to green and set the model path if it is valid
        ModelPathTextBox.PathTextBox.BorderBrush = new SolidColorBrush(Colors.MediumSpringGreen);
        DataManager.ModelPath = modelPath;
        UpdateRecentFiles(modelPath);

        // Switch to the RenderPage
        var mainWindow = (Windows.MainWindow)this.VisualRoot!;
        NavigationManager.SwitchPage(mainWindow, new RenderPage());
    }

    /// <summary>
    /// Handles the selection changed event of the RecentlyOpenedComboBox.
    /// </summary>
    private void RecentlyOpenedComboBox_OnSelectionChanged(
        object? sender,
        SelectionChangedEventArgs e
    )
    {
        // Set the model path to the selected item in the RecentlyOpenedComboBox
        var selectedText = RecentlyOpenedComboBox.SelectedItem?.ToString();
        ModelPathTextBox.PathTextBox.Text = selectedText;
    }

    /// <summary>
    /// Handles the selection changed event of the UnitComboBox.
    /// </summary>
    private void UnitComboBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        // Set the unit scale based on the selected item in the UnitComboBox
        var comboBoxItem = (ComboBoxItem)UnitComboBox.SelectedItem;
        var selectedText = comboBoxItem.Content.ToString().ToLower();

        DataManager.UnitScale = selectedText switch
        {
            "millimeters" => ModelUnit.Millimeter,
            "centimeters" => ModelUnit.Centimeter,
            "meters" => ModelUnit.Meter,
            "inches" => ModelUnit.Inch,
            "feet" => ModelUnit.Foot,
            _ => DataManager.UnitScale,
        };
    }

    /// <summary>
    /// Handles the click event of the Back button.
    /// </summary>
    private void BackButton_OnClick(object? sender, RoutedEventArgs e)
    {
        // Switch to the RequirementsPage
        var mainWindow = (Windows.MainWindow)this.VisualRoot!;
        NavigationManager.SwitchPage(mainWindow, new RequirementsPage());
    }
}
