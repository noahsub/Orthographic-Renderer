using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using AvaloniaColorPicker;
using Orthographic.Renderer.Constants;
using Orthographic.Renderer.Controls;
using Orthographic.Renderer.Managers;
using Orthographic.Renderer.Windows;

namespace Orthographic.Renderer.Pages;

public partial class LightingPage : UserControl
{
    public LightingPage()
    {
        InitializeComponent();

        BackgroundColourSelector.ColourPicker.Color = Colors.Black;
        BackgroundColourSelector.ColourChanged += BackgroundColourChanged_Event;
        
        AspectRatio16X9ToggleButton.IsChecked = true;

        for (int i = 0; i < 12; i++)
        {
            Button resolutionButton = new Button();
            resolutionButton.Content = Resolution.AspectRatio16X9[i];
            resolutionButton.Click += ResolutionButton_OnClick;
            resolutionButton.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
            resolutionButton.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch;
            resolutionButton.HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Center;
            resolutionButton.VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center;
            resolutionButton.Margin = new Thickness(5);
            Grid.SetRow(resolutionButton, (i / 6) + 1);
            Grid.SetColumn(resolutionButton, i % 6);
            ResolutionGrid.Children.Add(resolutionButton);
        }
    }

    private void ResolutionButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var button = (Button)sender!;

        if (button.Content is null)
        {
            return;
        }

        WidthTextBox.Text = Resolution.ResolutionDictionary[button.Content.ToString() ?? string.Empty].Item1.ToString();
        HeightTextBox.Text = Resolution.ResolutionDictionary[button.Content.ToString() ?? string.Empty].Item2.ToString();
    }

    private void BackButton_OnClick(object? sender, RoutedEventArgs e)
    {
        // Switch to the ModelPage
        var mainWindow = (MainWindow)this.VisualRoot!;
        NavigationManager.SwitchPage(mainWindow, "ModelPage");
    }

    private void ViewsButton_OnClick(object? sender, RoutedEventArgs e)
    {
        // Switch to the RenderPage
        var mainWindow = (MainWindow)this.VisualRoot!;
        NavigationManager.SwitchPage(mainWindow, "RenderPage");
    }

    private void BackgroundColourChanged_Event(object? sender, EventArgs e)
    {
        // get the colour from the colour picker
        var colour = BackgroundColourSelector.ColourPicker.Color;
        
        // if the colour is transparent we set the background to black
        if (colour == Colors.Transparent)
        {
            BackgroundRectangle.Fill = new SolidColorBrush(Colors.Black);
            return;
        }
        
        // otherwise we set the background to the selected colour
        BackgroundRectangle.Fill = new SolidColorBrush(colour);
    }

    private void AspectRatio1X1ToggleButton_OnClick(object? sender, RoutedEventArgs e)
    {
        AspectRatio16X9ToggleButton.IsChecked = false;
        AspectRatio21X9ToggleButton.IsChecked = false;
        AspectRatio4X3ToggleButton.IsChecked = false;

        for (int i = 0; i < 12; i++)
        {
            // get button from ResolutionGrid starting from the 7th element
            Button resolutionButton = (Button)ResolutionGrid.Children[i + 6];
            resolutionButton.Content = Resolution.AspectRatio1X1[i];
        }
    }

    private void AspectRatio4X3ToggleButton_OnClick(object? sender, RoutedEventArgs e)
    {
        AspectRatio16X9ToggleButton.IsChecked = false;
        AspectRatio21X9ToggleButton.IsChecked = false;
        AspectRatio1X1ToggleButton.IsChecked = false;

        for (int i = 0; i < 12; i++)
        {
            // get button from ResolutionGrid starting from the 7th element
            Button resolutionButton = (Button)ResolutionGrid.Children[i + 6];
            resolutionButton.Content = Resolution.AspectRatio4X3[i];
        }
    }

    private void AspectRatio16X9ToggleButton_OnClick(object? sender, RoutedEventArgs e)
    {
        AspectRatio4X3ToggleButton.IsChecked = false;
        AspectRatio21X9ToggleButton.IsChecked = false;
        AspectRatio1X1ToggleButton.IsChecked = false;

        for (int i = 0; i < 12; i++)
        {
            // get button from ResolutionGrid starting from the 7th element
            Button resolutionButton = (Button)ResolutionGrid.Children[i + 6];
            resolutionButton.Content = Resolution.AspectRatio16X9[i];
        }
    }

    private void AspectRatio21X9ToggleButton_OnClick(object? sender, RoutedEventArgs e)
    {
        AspectRatio4X3ToggleButton.IsChecked = false;
        AspectRatio16X9ToggleButton.IsChecked = false;
        AspectRatio1X1ToggleButton.IsChecked = false;

        for (int i = 0; i < 12; i++)
        {
            // get button from ResolutionGrid starting from the 7th element
            Button resolutionButton = (Button)ResolutionGrid.Children[i + 6];
            resolutionButton.Content = Resolution.AspectRatio21X9[i];
        }
    }
}