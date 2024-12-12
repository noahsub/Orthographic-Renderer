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
using Orthographic.Renderer.Interfaces;
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
public partial class ModelPage : UserControl, IPage
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // INITIALIZATION
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Initializes a new instance of the <see cref="ModelPage"/> class.
    /// </summary>
    public ModelPage()
    {
        Initialize();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // EVENT HANDLERS
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Handles the click event of the Next button.
    /// </summary>
    private void NextButton_OnClick(object? sender, RoutedEventArgs e)
    {
        // Fix the model path
        ModelFilePathSelector.FixPath();
        
        var path = ModelFilePathSelector.GetPath();

        if (!ModelFilePathSelector.CheckPath(FileType.Model))
        {
            return;
        }

        if (!string.Equals(DataManager.ModelPath, path))
        {
            DataManager.ModelPath = path;
        }
        
        var mainWindow = (Windows.MainWindow)this.VisualRoot!;
        NavigationManager.SwitchPage(mainWindow, "LightingPage");
    }

    /// <summary>
    /// Handles the selection changed event of the RecentlyOpenedComboBox.
    /// </summary>
    private void RecentlyOpenedComboBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        var selectedItem = RecentlyOpenedComboBox.SelectedItem?.ToString() ?? string.Empty;
        ModelFilePathSelector.SetPath(selectedItem);
        SizeLabel.Content = "UNKNOWN";
    }

    /// <summary>
    /// Handles the selection changed event of the UnitComboBox.
    /// </summary>
    private void UnitComboBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        // Set the unit scale based on the selected item in the UnitComboBox
        var comboBoxItem = (ComboBoxItem)UnitComboBox.SelectedItem!;
        var selectedText = comboBoxItem.Content?.ToString()?.ToLower() ?? string.Empty;
        ModelManager.SetModelUnit(selectedText);
    }

    /// <summary>
    /// Handles the click event of the Back button.
    /// </summary>
    private void BackButton_OnClick(object? sender, RoutedEventArgs e)
    {
        // Switch to the RequirementsPage
        var mainWindow = (Windows.MainWindow)this.VisualRoot!;
        NavigationManager.SwitchPage(mainWindow, "HardwarePage");
    }

    /// <summary>
    /// Measures the dimensions of the model.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MeasureButton_OnClick(object? sender, RoutedEventArgs e)
    {
        // Fix the model path
        ModelFilePathSelector.FixPath();
        // Check the model path
        if (!ModelFilePathSelector.CheckPath(FileType.Model))
        {
            return;
        }
        
        // Get the model path
        var path = ModelFilePathSelector.GetPath();

        // Get and set the dimensions of the model
        var dimensions = ModelManager.GetDimensions(path);
        SizeLabel.Content = $"Size X: {dimensions.X}, Size Y: {dimensions.Y}, Size Z: {dimensions.Z} (unit unknown)";
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // IPAGE INTERFACE IMPLEMENTATION
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Initializes the ModelPage.
    /// </summary>
    public void Initialize()
    {
        InitializeComponent();
        
        // Load the recent models
        var recentModels = FileManager.GetRecentModels();
        foreach (var model in recentModels)
        {
            RecentlyOpenedComboBox.Items.Add(model);
        }
        
        // Set the default unit
        UnitComboBox.SelectedIndex = 0;
    }
 
    /// <summary>
    /// When the page is first loaded by the user.
    /// </summary>
    public void OnFirstLoad()
    {
        return;
    }

    /// <summary>
    ///  When the page is navigated to.
    /// </summary>
    public void OnNavigatedTo()
    {
        return;
    }
}
