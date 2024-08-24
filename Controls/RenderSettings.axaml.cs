using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Orthographic.Renderer.Managers;

namespace Orthographic.Renderer.Controls;

public partial class RenderSettings : UserControl
{
    private const int DefaultMode = 0;
    private const int DefaultThreads = 1;
    private const string DefaultPrefix = "Render";
    private const decimal DefaultDistance = 8.0m;
    private const int DefaultResolutionWidth = 1920;
    private const int DefaultResolutionHeight = 1080;
    private const int DefaultScale = 100;

    public RenderSettings()
    {
        InitializeComponent();
        PresetResolutions.SetColumns(5, 10, false);
        
        // Set bounds for numeric inputs
        ThreadsNumeric.Minimum = 1;
        ResolutionWidthNumeric.Minimum = 1;
        ResolutionHeightNumeric.Minimum = 1;
        ScaleNumeric.Minimum = 1;
        ScaleNumeric.Maximum = 100;
        
        // Set step values for numeric inputs
        DistanceNumeric.Increment = 0.5m;
        
        // Set default values
        RenderModeComboBox.SelectedIndex = DefaultMode;
        ThreadsNumeric.Value = DefaultThreads;
        PrefixTextBox.Text = DefaultPrefix;
        DistanceNumeric.Value = DefaultDistance;
        ResolutionWidthNumeric.Value = DefaultResolutionWidth;
        ResolutionHeightNumeric.Value = DefaultResolutionHeight;
        ScaleNumeric.Value = DefaultScale;
        PlaySoundCheckBox.IsChecked = true;

        Dictionary<string, Tuple<int, int>> commonResolutions =
            new()
            {
                { "480p", new Tuple<int, int>(854, 480) },
                { "720p", new Tuple<int, int>(1280, 720) },
                { "1080p", new Tuple<int, int>(1920, 1080) },
                { "1440p", new Tuple<int, int>(2560, 1440) },
                { "4K", new Tuple<int, int>(3840, 2160) },
                { "5K", new Tuple<int, int>(5120, 2880) },
                { "8K", new Tuple<int, int>(7680, 4320) },
                { "10K", new Tuple<int, int>(10240, 5760) },
                { "12K", new Tuple<int, int>(12288, 6480) },
                { "16K", new Tuple<int, int>(15360, 8640) },
            };

        foreach (var resolution in commonResolutions)
        {
            Button button = new Button
            {
                Content = resolution.Key,
                Margin = new Thickness(0, 0, 0, 0),
                Width = double.NaN,
                HorizontalAlignment = HorizontalAlignment.Stretch,
            };

            button.Click += (sender, e) =>
            {
                ResolutionWidthNumeric.Value = resolution.Value.Item1;
                ResolutionHeightNumeric.Value = resolution.Value.Item2;
            };

            PresetResolutions.AddItem(button);
        }
    }

    private void RenderModeComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (RenderModeComboBox.SelectedIndex == 0)
        {
            ThreadsNumeric.Value = 1;
            ThreadsNumeric.IsEnabled = false;
        }
        else
        {
            ThreadsNumeric.IsEnabled = true;
        }
    }

    public bool VerifyRenderSettings()
    {
        var valid = true;
        
        // ensure that mode is not null and is in [Sequential, Parallel]
        if (RenderModeComboBox.SelectedIndex != 0 && RenderModeComboBox.SelectedIndex != 1)
        {
            valid = false;
        }
        
        // ensure that threads is not null and is between [0, 100] and rounded to the nearest integer
        if (ThreadsNumeric.Value == null || ThreadsNumeric.Value < 1 || ThreadsNumeric.Value > 100)
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
        if (string.IsNullOrEmpty(OutputDirTextBox.PathTextBox.Text) || !FileManager.VerifyDirectoryPath(OutputDirTextBox.PathTextBox.Text))
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
        
        // ensure that the resolution width and height are not null and are not less than 1, and are rounded to the nearest integer
        if (ResolutionWidthNumeric.Value == null || ResolutionWidthNumeric.Value < 1)
        {
            ResolutionWidthNumeric.BorderBrush = Brushes.IndianRed;
            valid = false;
        }

        else
        {
            ResolutionWidthNumeric.Value = (int)Math.Round(ResolutionWidthNumeric.Value.Value);
            ResolutionWidthNumeric.BorderBrush = Brushes.Transparent;
        }
        
        if (ResolutionHeightNumeric.Value == null || ResolutionHeightNumeric.Value < 1)
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
        if (ScaleNumeric.Value == null || ScaleNumeric.Value < 1 || ScaleNumeric.Value > 100)
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
    
    public string GetMode()
    {
        return RenderModeComboBox.SelectionBoxItem?.ToString()?.ToLower() ?? String.Empty;
    }
    
    public int GetThreads()
    {
        if (ThreadsNumeric.Value == null)
        {
            return 0;
        }
        
        return (int)ThreadsNumeric.Value;
    }
    
    public string GetPrefix()
    {
        return PrefixTextBox.Text ?? string.Empty;
    }

    public string GetOutputDir()
    {
        return OutputDirTextBox.PathTextBox.Text ?? string.Empty;
    }
    
    public float GetDistance()
    {
        if (DistanceNumeric.Value == null)
        {
            return 0;
        }
        
        return (float)DistanceNumeric.Value;
    }
    
    public int GetResolutionWidth()
    {
        if (ResolutionWidthNumeric.Value == null)
        {
            return 0;
        }
        
        return (int)ResolutionWidthNumeric.Value;
    }
    
    public int GetResolutionHeight()
    {
        if (ResolutionHeightNumeric.Value == null)
        {
            return 0;
        }
        
        return (int)ResolutionHeightNumeric.Value;
    }
    
    public int GetScale()
    {
        if (ScaleNumeric.Value == null)
        {
            return 0;
        }
        
        return (int)ScaleNumeric.Value;
    }
    
    public bool GetPlaySound()
    {
        if (PlaySoundCheckBox.IsChecked == null)
        {
            return false;
        }
        
        return (bool)PlaySoundCheckBox.IsChecked;
    }
}
