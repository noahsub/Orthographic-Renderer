﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using AvaloniaColorPicker;
using Orthographic.Renderer.Controls;
using Orthographic.Renderer.Entities;
using Orthographic.Renderer.Managers;
using Orthographic.Renderer.Windows;
using RenderOptions = Orthographic.Renderer.Entities.RenderOptions;
using Resolution = Orthographic.Renderer.Constants.Resolution;

namespace Orthographic.Renderer.Pages;

public partial class LightingPage : UserControl
{
    public LightingPage()
    {
        InitializeComponent();
        
        FileLabel.Content = Path.GetFileName(DataManager.ModelPath);

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

    private void OnePointLightingButton_OnClick(object? sender, RoutedEventArgs e)
    {
        LightOptionsStackPanel.Children.Clear();
        
        // Verify that the StackPanel is empty
        Debug.Assert(LightOptionsStackPanel.Children.Count == 0, "StackPanel is not empty after clearing.");
        
        var dimensions = ModelManager.GetDimensions(DataManager.ModelPath);
        var maxDimension = new[] { dimensions.X, dimensions.Y, dimensions.Z }.Max() * DataManager.UnitScale;
        
        var light = new LightOptions();
        light.SetOrientation("front");
        light.SetColour(Colors.White);
        light.SetPower(1000);
        light.SetSize(3);
        light.SetDistance(maxDimension * 5);
        
        LightOptionsStackPanel.Children.Add(light);
    }

    private void ThreePointLightingButton_OnClick(object? sender, RoutedEventArgs e)
    {
        LightOptionsStackPanel.Children.Clear();
        
        // Verify that the StackPanel is empty
        Debug.Assert(LightOptionsStackPanel.Children.Count == 0, "StackPanel is not empty after clearing.");
        
        var dimensions = ModelManager.GetDimensions(DataManager.ModelPath);
        var maxDimension = new[] { dimensions.X, dimensions.Y, dimensions.Z }.Max() * DataManager.UnitScale;

        LightOptions light1 = new LightOptions();
        light1.SetOrientation("top-right-back");
        light1.SetColour(Colors.White);
        light1.SetPower(200);
        light1.SetSize(3);
        light1.SetDistance(maxDimension * 5);
        
        LightOptions light2 = new LightOptions();
        light2.SetOrientation("top-back-left");
        light2.SetColour(Colors.White);
        light2.SetPower(1000);
        light2.SetSize(3);
        light2.SetDistance(maxDimension * 5);
        
        LightOptions light3 = new LightOptions();
        light3.SetOrientation("top-left-front");
        light3.SetColour(Colors.White);
        light3.SetPower(800);
        light3.SetSize(3);
        light3.SetDistance(maxDimension * 5);
        
        LightOptionsStackPanel.Children.Add(light1);
        LightOptionsStackPanel.Children.Add(light2);
        LightOptionsStackPanel.Children.Add(light3);
    }

    private void OverheadLightingButton_OnClick(object? sender, RoutedEventArgs e)
    {
        LightOptionsStackPanel.Children.Clear();
        
        // Verify that the StackPanel is empty
        Debug.Assert(LightOptionsStackPanel.Children.Count == 0, "StackPanel is not empty after clearing.");
        
        var dimensions = ModelManager.GetDimensions(DataManager.ModelPath);
        var maxDimension = new[] { dimensions.X, dimensions.Y, dimensions.Z }.Max() * DataManager.UnitScale;

        LightOptions light = new LightOptions();
        light.SetOrientation("top");
        light.SetColour(Colors.White);
        light.SetPower(1000);
        light.SetSize(3);
        light.SetDistance(maxDimension * 5);
        
        LightOptionsStackPanel.Children.Add(light);
    }

    private void AddLightButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var light = new LightOptions();
        LightOptionsStackPanel.Children.Add(light);
    }

    private void ClearButton_OnClick(object? sender, RoutedEventArgs e)
    {
        LightOptionsStackPanel.Children.Clear();
        
        // Verify that the StackPanel is empty
        Debug.Assert(LightOptionsStackPanel.Children.Count == 0, "StackPanel is not empty after clearing.");
    }

    private void PreviewButton_OnClick(object? sender, RoutedEventArgs e)
    {
        DataManager.PreviewRenderOptions = new RenderOptions();
        
        var resolution = new Entities.Resolution(Int32.Parse(WidthTextBox.Text ?? "0") , Int32.Parse(HeightTextBox.Text ?? "0"), 75);
        var cameraView = CameraOrientation.CurrentOrientation.Name; 
        var cameraDistance = float.Parse(CameraDistance.ValueTextBox.Text ?? "0");
        
        Debug.WriteLine("Camera Distance: " + cameraDistance);
        
        var cameraPosition = RenderManager.GetPosition(cameraView, cameraDistance);
        
        Debug.WriteLine($"Camera Position: X: {cameraPosition.X} Y: {cameraPosition.Y} Z: {cameraPosition.Z} RX: {cameraPosition.Rx} RY: {cameraPosition.Ry} RZ: {cameraPosition.Rz}");
        
        var camera = new Camera(cameraDistance, cameraPosition);
        
        var uuid = Guid.NewGuid().ToString().Replace("-", "");
        var tempDirectory = Path.GetTempPath().Replace("\\", "/");
        Debug.WriteLine(tempDirectory);
        
        DataManager.PreviewRenderOptions.SetName(uuid);
        DataManager.PreviewRenderOptions.SetModel(DataManager.ModelPath);
        DataManager.PreviewRenderOptions.SetUnit(DataManager.UnitScale);
        DataManager.PreviewRenderOptions.SetOutputDirectory(tempDirectory);
        DataManager.PreviewRenderOptions.SetResolution(resolution);
        DataManager.PreviewRenderOptions.SetCamera(camera);
        DataManager.PreviewRenderOptions.SetSaveBlenderFile(true);
        
        for (int i = 0; i < LightOptionsStackPanel.Children.Count; i++)
        {
            var lightOptions = (LightOptions)LightOptionsStackPanel.Children[i];
            var lightOrientation = lightOptions.LightOrientationSelector.CurrentOrientation.Name;
            var lightColour = lightOptions.LightColourSelector.ColourPicker.Color.ToString();
            var lightPower = float.Parse(lightOptions.PowerValueSelector.ValueTextBox.Text ?? "0" ?? "0");
            var lightSize = float.Parse(lightOptions.SizeValueSelector.ValueTextBox.Text ?? "0" ?? "0");
            var lightDistance = float.Parse(lightOptions.DistanceValueSelector.ValueTextBox.Text ?? "0" ?? "0");
            var lightPosition = RenderManager.GetPosition(lightOrientation, lightDistance);
            var light = new Light(lightPosition, lightColour, lightPower, lightSize, lightDistance);
            DataManager.PreviewRenderOptions.AddLight(light);
        }

        Task.Run(async () =>
        {
            RenderManager.Render(DataManager.PreviewRenderOptions);
            
            Dispatcher.UIThread.Post(() =>
            {
                PreviewImage.Source = new Bitmap(tempDirectory + uuid + ".png");
            });
        });
    }
}