////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// RenderSettings.axaml.cs
// This file contains the logic for the RenderSettings control.
//
// Copyright (C) 2024 noahsub
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// IMPORTS
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Orthographic.Renderer.Constants;
using Orthographic.Renderer.Managers;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// NAMESPACE
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Orthographic.Renderer.Controls;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// RENDER SETTINGS CLASS
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

/// <summary>
/// Settings for rendering a scene.
/// </summary>
public partial class RenderSettings : UserControl
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // INITIALIZATION
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Initializes a new instance of the <see cref="RenderSettings"/> class.
    /// </summary>
    public RenderSettings()
    {
        InitializeComponent();

        // Set layout for preset resolutions
        PresetResolutions.SetLayout(5, 10, false);

        // Set bounds for numeric inputs
        ThreadsNumeric.Minimum = 1;
        ResolutionWidthNumeric.Minimum = 1;
        ResolutionHeightNumeric.Minimum = 1;
        ScaleNumeric.Minimum = 1;
        ScaleNumeric.Maximum = 100;

        // Set step values for numeric inputs
        DistanceNumeric.Increment = 0.5m;

        // Set default values
        RenderModeComboBox.SelectedIndex = RenderValues.DefaultMode;
        ThreadsNumeric.Value = RenderValues.DefaultThreads;
        PrefixTextBox.Text = RenderValues.DefaultPrefix;
        DistanceNumeric.Value = RenderValues.DefaultDistance;
        LightDistanceNumeric.Value = RenderValues.DefaultLightDistance;
        ResolutionWidthNumeric.Value = RenderValues.DefaultResolutionWidth;
        ResolutionHeightNumeric.Value = RenderValues.DefaultResolutionHeight;
        ScaleNumeric.Value = RenderValues.DefaultScale;
        PlaySoundCheckBox.IsChecked = true;

        // Preset resolutions
        PopulatePresetResolutions();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // VERIFICATION
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Verifies that all render settings are valid.
    /// </summary>
    /// <returns>True if all render settings are valid, false otherwise</returns>
    public bool VerifyRenderSettings()
    {
        // Flag to determine if all settings are valid
        var valid = true;

        // ensure that mode is not null and is in [Sequential, Parallel]
        if (RenderModeComboBox.SelectedIndex != 0 && RenderModeComboBox.SelectedIndex != 1)
        {
            valid = false;
        }

        // ensure that threads is not null and is between [0, 100] and rounded to the nearest integer
        if (ThreadsNumeric.Value is null or < 1 or > 100)
        {
            ThreadsNumeric.BorderBrush = Brushes.IndianRed;
            valid = false;
        }
        else
        {
            ThreadsNumeric.Value = (int)Math.Round(ThreadsNumeric.Value.Value);
            ThreadsNumeric.BorderBrush = Brushes.Transparent;
        }

        // ensure that the prefix is not null and is not empty
        if (string.IsNullOrEmpty(PrefixTextBox.Text))
        {
            PrefixTextBox.BorderBrush = Brushes.IndianRed;
            valid = false;
        }
        else
        {
            PrefixTextBox.BorderBrush = Brushes.Transparent;
        }

        // ensure that the directory path is not null and is a valid directory
        if (
            string.IsNullOrEmpty(OutputDirTextBox.PathTextBox.Text)
            || !FileManager.VerifyDirectoryPath(OutputDirTextBox.PathTextBox.Text)
        )
        {
            OutputDirTextBox.PathTextBox.BorderBrush = Brushes.IndianRed;
            valid = false;
        }
        else
        {
            OutputDirTextBox.PathTextBox.BorderBrush = Brushes.Transparent;
        }

        // ensure that the distance is not null
        if (DistanceNumeric.Value == null)
        {
            DistanceNumeric.BorderBrush = Brushes.IndianRed;
            valid = false;
        }
        else
        {
            DistanceNumeric.BorderBrush = Brushes.Transparent;
        }

        // ensure that the light distance is not null
        if (DistanceNumeric.Value == null)
        {
            LightDistanceNumeric.BorderBrush = Brushes.IndianRed;
            valid = false;
        }
        else
        {
            LightDistanceNumeric.BorderBrush = Brushes.Transparent;
        }

        // ensure that the resolution width and height are not null and are not less than 1, and are rounded to the nearest integer
        if (ResolutionWidthNumeric.Value is null or < 1)
        {
            ResolutionWidthNumeric.BorderBrush = Brushes.IndianRed;
            valid = false;
        }
        else
        {
            ResolutionWidthNumeric.Value = (int)Math.Round(ResolutionWidthNumeric.Value.Value);
            ResolutionWidthNumeric.BorderBrush = Brushes.Transparent;
        }

        // ensure that the resolution width and height are not null and are not less than 1, and are rounded to the nearest integer
        if (ResolutionHeightNumeric.Value is null or < 1)
        {
            ResolutionHeightNumeric.BorderBrush = Brushes.IndianRed;
            valid = false;
        }
        else
        {
            ResolutionHeightNumeric.Value = (int)Math.Round(ResolutionHeightNumeric.Value.Value);
            ResolutionHeightNumeric.BorderBrush = Brushes.Transparent;
        }

        // ensure that the scale is not null and is between [1, 100] and rounded to the nearest integer
        if (ScaleNumeric.Value is null or < 1 or > 100)
        {
            ScaleNumeric.BorderBrush = Brushes.IndianRed;
            valid = false;
        }
        else
        {
            ScaleNumeric.Value = (int)Math.Round(ScaleNumeric.Value.Value);
            ScaleNumeric.BorderBrush = Brushes.Transparent;
        }

        return valid;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // GETTERS
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Gets the render mode.
    /// </summary>
    /// <returns>The render mode.</returns>
    public string GetMode()
    {
        return RenderModeComboBox.SelectionBoxItem?.ToString()?.ToLower() ?? string.Empty;
    }

    /// <summary>
    /// Gets the number of threads to use for parallel rendering.
    /// </summary>
    /// <returns>The number of threads to use for parallel rendering.</returns>
    public int GetThreads()
    {
        if (ThreadsNumeric.Value == null)
        {
            return 0;
        }

        return (int)ThreadsNumeric.Value;
    }

    /// <summary>
    /// Gets the prefix to use for the output files.
    /// </summary>
    /// <returns>The prefix to use for the output files.</returns>
    public string GetPrefix()
    {
        return PrefixTextBox.Text ?? string.Empty;
    }

    /// <summary>
    ///  Gets the output directory.
    /// </summary>
    /// <returns>The output directory.</returns>
    public string GetOutputDir()
    {
        return OutputDirTextBox.PathTextBox.Text ?? string.Empty;
    }

    /// <summary>
    /// Get the distance between the camera and the scene origin.
    /// </summary>
    /// <returns>The distance between the camera and the scene origin.</returns>
    public float GetDistance()
    {
        if (DistanceNumeric.Value == null)
        {
            return 0;
        }

        return (float)DistanceNumeric.Value;
    }

    /// <summary>
    /// Get the distance between the lights and the scene origin.
    /// </summary>
    /// <returns>The distance between the camera and the scene origin.</returns>
    public float GetLightDistance()
    {
        if (LightDistanceNumeric.Value == null)
        {
            return 0;
        }

        return (float)LightDistanceNumeric.Value;
    }

    /// <summary>
    /// Get the resolution width.
    /// </summary>
    /// <returns>The resolution width.</returns>
    public int GetResolutionWidth()
    {
        if (ResolutionWidthNumeric.Value == null)
        {
            return 0;
        }

        return (int)ResolutionWidthNumeric.Value;
    }

    /// <summary>
    /// Get the resolution height.
    /// </summary>
    /// <returns>The resolution height.</returns>
    public int GetResolutionHeight()
    {
        if (ResolutionHeightNumeric.Value == null)
        {
            return 0;
        }

        return (int)ResolutionHeightNumeric.Value;
    }

    /// <summary>
    /// Get the scale of the rendered image.
    /// </summary>
    /// <returns>The scale of the rendered image</returns>
    public int GetScale()
    {
        if (ScaleNumeric.Value == null)
        {
            return 0;
        }

        return (int)ScaleNumeric.Value;
    }

    /// <summary>
    /// Get the value of the play sound checkbox.
    /// </summary>
    /// <returns>True if the play sound checkbox is checked, false otherwise.</returns>
    public bool GetPlaySound()
    {
        if (PlaySoundCheckBox.IsChecked == null)
        {
            return false;
        }

        return (bool)PlaySoundCheckBox.IsChecked;
    }

    /// <summary>
    /// Get the value of the save checkbox.
    /// </summary>
    /// <returns>True if the save checkbox is checked, false otherwise.</returns>
    public bool GetSave()
    {
        if (SaveCheckBox.IsChecked == null)
        {
            return false;
        }

        return (bool)SaveCheckBox.IsChecked;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // HELPER FUNCTIONS
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Populates the preset resolutions.
    /// </summary>
    private void PopulatePresetResolutions()
    {
        // Common resolutions and their dimensions
        var commonResolutions = new Dictionary<string, Tuple<int, int>>
        {
            ["480p"] = Resolution._480p,
            ["720p"] = Resolution._720p,
            ["1080p"] = Resolution._1080p,
            ["1440p"] = Resolution._1440p,
            ["4K"] = Resolution._4K,
            ["5K"] = Resolution._5K,
            ["8K"] = Resolution._8K,
            ["10K"] = Resolution._10K,
            ["12K"] = Resolution._12K,
            ["16K"] = Resolution._16K,
        };

        // Populate preset resolutions
        foreach (var resolution in commonResolutions)
        {
            // Create a button for each resolution
            var button = new Button
            {
                Content = resolution.Key,
                Margin = new Thickness(0, 0, 0, 0),
                Width = double.NaN,
                HorizontalAlignment = HorizontalAlignment.Stretch,
            };

            // If the button is clicked, set the resolution width and height to the corresponding values
            button.Click += (sender, e) =>
            {
                ResolutionWidthNumeric.Value = resolution.Value.Item1;
                ResolutionHeightNumeric.Value = resolution.Value.Item2;
            };

            // Add the button to the preset resolutions
            PresetResolutions.AddItem(button);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // EVENTS
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Handles the mode selection changed event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void RenderModeComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // If the mode is set to sequential, disable the threads numeric input
        if (RenderModeComboBox.SelectedIndex == 0)
        {
            ThreadsNumeric.Value = 1;
            ThreadsNumeric.IsEnabled = false;
        }
        // Otherwise, the mode is set to parallel and the threads numeric input is enabled
        else
        {
            ThreadsNumeric.IsEnabled = true;
        }
    }
}
