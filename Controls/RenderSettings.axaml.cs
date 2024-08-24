using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;

namespace Orthographic.Renderer.Controls;

public partial class RenderSettings : UserControl
{
    public RenderSettings()
    {
        InitializeComponent();
        ThreadsNumeric.Minimum = 1;
        ResolutionWidthNumeric.Minimum = 1;
        ResolutionHeightNumeric.Minimum = 1;
        ScaleNumeric.Minimum = 1;
        ScaleNumeric.Maximum = 100;
        PlaySoundCheckBox.IsChecked = true;

        RenderModeComboBox.SelectedIndex = 0;

        ThreadsNumeric.Value = 1;
        PrefixTextBox.Text = "Render";

        ResolutionWidthNumeric.Value = 1920;
        ResolutionHeightNumeric.Value = 1080;

        ScaleNumeric.Value = 100;
        DistanceNumeric.Value = 8;

        PresetResolutions.SetColumns(5, 10, false);

        Dictionary<string, Tuple<int, int>> CommonResolutions =
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

        foreach (var resolution in CommonResolutions)
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

    private void RenderModeComboBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
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
}
