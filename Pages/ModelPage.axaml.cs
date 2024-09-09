////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// ModelPage.axaml.cs
// This file contains the logic for the ModelPage.
//
// Copyright (C) 2024 noahsub
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// IMPORTS
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using Avalonia.Controls;
using Avalonia.Input;
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
        var paths = FileManager.ReadJsonArray(FileManager.GetRecentModelsFile(), "paths");

        // Remove the model path if it already exists
        paths.Remove(modelPath);
        // Add the model path to the front of the list
        paths.Insert(0, modelPath);

        // If the list of paths is greater than 10, remove the last path
        if (paths.Count > 10)
        {
            paths.RemoveAt(10);
        }

        // Write the updated paths to the file
        FileManager.WriteArrayToJsonFile(FileManager.GetRecentModelsFile(), "paths", paths);
    }

    /// <summary>
    /// Loads the recent files into the RecentlyOpenedComboBox.
    /// </summary>
    private void LoadRecentFiles()
    {
        // Read all paths
        var paths = FileManager.ReadJsonArray(FileManager.GetRecentModelsFile(), "paths");

        // Add each path to the RecentlyOpenedComboBox
        foreach (var path in paths.Where(path => IsValidModelPath(path)))
        {
            RecentlyOpenedComboBox.Items.Add(path);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // WARNING
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Display a warning message to the user.
    /// </summary>
    /// <param name="message"></param>
    private static void DisplayWarning(string message)
    {
        var warning = new Windows.Warning();
        warning.SetWarning(message);
        warning.Show();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // SETTERS
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public void SetDimensionsUnknown()
    {
        SizeLabel.Content = "unknown";
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

        // Check if the path requires elevated permissions
        if (FileManager.ElevatedPath(modelPath))
        {
            DisplayWarning(
                "The model file is located in a protected directory. Please run the application as an administrator or move the file to a different location."
            );
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
        NavigationManager.SwitchPage(mainWindow, "RenderPage");
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
        SizeLabel.Content = "unknown";
    }

    /// <summary>
    /// Handles the selection changed event of the UnitComboBox.
    /// </summary>
    private void UnitComboBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        // Set the unit scale based on the selected item in the UnitComboBox
        var comboBoxItem = (ComboBoxItem)UnitComboBox.SelectedItem!;
        var selectedText = comboBoxItem.Content?.ToString()?.ToLower();

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
        NavigationManager.SwitchPage(mainWindow, "RequirementsPage");
    }

    /// <summary>
    /// Measures the dimensions of the model.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MeasureButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var modelPath = ModelPathTextBox.PathTextBox.Text;
        
        // if the model path is null or empty
        if (string.IsNullOrEmpty(modelPath))
        {
            SizeLabel.Content = "unknown";
            return;
        }
        
        // Get the model path
        modelPath = FileManager.ReformatPath(modelPath);
        if (
            modelPath == null
            || !IsValidModelPath(modelPath)
            || FileManager.ElevatedPath(modelPath)
        )
        {
            SizeLabel.Content = "unknown";
            return;
        }

        ModelPathTextBox.PathTextBox.Text = modelPath;

        // Get the extension of the file
        var extension = Path.GetExtension(modelPath);

        // if the model is a valid type and not a .blend file
        if (!ValidTypes.Contains(extension))
        {
            SizeLabel.Content = "unknown";
            return;
        }

        // Get the dimensions of the model
        Vector3 dimensions;
        switch (extension)
        {
            case ".obj":
                dimensions = ModelManager.GetObjDimensions(modelPath);
                SizeLabel.Content =
                    $"Size X: {dimensions.X}, Size Y: {dimensions.Y}, Size Z: {dimensions.Z} (unit unknown)";
                break;
            case ".stl":
                dimensions = ModelManager.GetStlDimensions(modelPath);
                SizeLabel.Content =
                    $"Size X: {dimensions.X}, Size Y: {dimensions.Y}, Size Z: {dimensions.Z} (unit unknown)";
                break;
            default:
                SizeLabel.Content = "Cannot calculate dimensions for this file type.";
                break;
        }
    }

    /// <summary>
    /// Deselects the selected item in the RecentlyOpenedComboBox when tapped.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void RecentlyOpenedComboBox_OnTapped(object? sender, TappedEventArgs e)
    {
        var text = ModelPathTextBox.PathTextBox.Text;
        // Deselect any selected items
        RecentlyOpenedComboBox.SelectedIndex = -1;
        // Set the model path to the original text
        ModelPathTextBox.PathTextBox.Text = text;
    }
}
