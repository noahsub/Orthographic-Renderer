using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
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
        
        Dispatcher.UIThread.Post(() =>
        {
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
        });
        
        var dimensions = ModelManager.GetDimensions(DataManager.ModelPath);
        var maxDimension = new[] { dimensions.X, dimensions.Y, dimensions.Z }.Max() * DataManager.UnitScale;
        CameraDistance.SetValue(maxDimension * 2);
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
    
    private void AspectRatioToggleButton_OnClick(object? sender, RoutedEventArgs e)
    {
        // Uncheck all aspect ratio toggle buttons
        AspectRatio16X9ToggleButton.IsChecked = false;
        AspectRatio21X9ToggleButton.IsChecked = false;
        AspectRatio4X3ToggleButton.IsChecked = false;
        AspectRatio1X1ToggleButton.IsChecked = false;

        // Determine the clicked button and set the corresponding aspect ratio
        var clickedButton = (ToggleButton)sender!;
        clickedButton.IsChecked = true;

        var aspectRatios = clickedButton switch
        {
            _ when clickedButton == AspectRatio1X1ToggleButton => Resolution.AspectRatio1X1.ToArray(),
            _ when clickedButton == AspectRatio4X3ToggleButton => Resolution.AspectRatio4X3.ToArray(),
            _ when clickedButton == AspectRatio16X9ToggleButton => Resolution.AspectRatio16X9.ToArray(),
            _ => Resolution.AspectRatio21X9.ToArray()
        };

        for (var i = 0; i < 12; i++)
        {
            // Get button from ResolutionGrid starting from the 7th element
            var resolutionButton = (Button)ResolutionGrid.Children[i + 6];
            resolutionButton.Content = aspectRatios[i];
        }
    }

    private void OnePointLightingButton_OnClick(object? sender, RoutedEventArgs e)
    {
        SetupLighting(new List<(string orientation, int power)>
        {
            ("front", 1000)
        });
    }

    private void ThreePointLightingButton_OnClick(object? sender, RoutedEventArgs e)
    {
        SetupLighting(new List<(string orientation, int power)>
        {
            ("top-right-back", 200),
            ("top-back-left", 1000),
            ("top-left-front", 800)
        });
    }

    private void OverheadLightingButton_OnClick(object? sender, RoutedEventArgs e)
    {
        SetupLighting(new List<(string orientation, int power)>
        {
            ("top", 1000)
        });
    }

    private void SetupLighting(List<(string orientation, int power)> lights)
    {
        // Get the maximum dimension of the model
        var dimensions = ModelManager.GetDimensions(DataManager.ModelPath);
        var maxDimension = new[] { dimensions.X, dimensions.Y, dimensions.Z }.Max() * DataManager.UnitScale;

        // Clear the current lights
        LightOptionsStackPanel.Children.Clear();
        
        // Add the lights to the UI
        foreach (var (orientation, power) in lights)
        {
            var light = new LightOptions();
            light.SetOrientation(orientation);
            light.SetColour(Colors.White);
            light.SetPower(power);
            light.SetSize(3);
            light.SetDistance(maxDimension * 80);
            LightOptionsStackPanel.Children.Add(light);
        }
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

    private List<Light> GetLights()
    {
        var lights = new List<Light>();

        foreach (var lightOption in LightOptionsStackPanel.Children)
        {
            var lightOptions = (LightOptions)lightOption;
            var lightOrientation = lightOptions.LightOrientationSelector.CurrentOrientation.Name;
            var lightColour = lightOptions.LightColourSelector.GetHexColour()[..7];
            var lightPower = float.Parse(lightOptions.PowerValueSelector.ValueTextBox.Text ?? "0");
            var lightSize = float.Parse(lightOptions.SizeValueSelector.ValueTextBox.Text ?? "0");
            var lightDistance = float.Parse(lightOptions.DistanceValueSelector.ValueTextBox.Text ?? "0");
            var lightPosition = RenderManager.GetPosition(lightOrientation, lightDistance);
            var light = new Light(lightPosition, lightColour, lightPower, lightSize, lightDistance);
            lights.Add(light);
        }

        return lights;
    }
    
    private void SaveOptions()
    {
        DataManager.CameraDistance = float.Parse(CameraDistance.ValueTextBox.Text ?? "0");
        DataManager.Resolution = new Entities.Resolution(int.Parse(WidthTextBox.Text ?? "0") , int.Parse(HeightTextBox.Text ?? "0"));
        DataManager.Lights = GetLights();
    }

    private void PreviewButton_OnClick(object? sender, RoutedEventArgs e)
    {
        // Update the UI to indicate that rendering is in progress.
        Dispatcher.UIThread.Post(() =>
        {
            PreviewButton.Content = "Rendering";
            PreviewButton.IsEnabled = false;
            PreviewImage.IsVisible = false;
            LoadingImage.IsVisible = true;
        });

        // Create the render options
        var previewRenderOptions = new RenderOptions();
        
        // Store and retrieve the current options
        SaveOptions();
        var resolution = DataManager.Resolution;
        var cameraDistance = DataManager.CameraDistance;
        var lights = DataManager.Lights;

        // Setup the preview camera
        var cameraView = CameraOrientation.CurrentOrientation.Name; 
        var cameraPosition = RenderManager.GetPosition(cameraView, cameraDistance);
        var camera = new Camera(cameraDistance, cameraPosition);
        
        // Get the uuid for the render
        var uuid = Guid.NewGuid().ToString().Replace("-", "");
        
        // Get the temp directory
        var tempDirectory = Path.GetTempPath().Replace("\\", "/");
        
        // Set the render options
        previewRenderOptions.SetName(uuid);
        previewRenderOptions.SetModel(DataManager.ModelPath);
        previewRenderOptions.SetUnit(DataManager.UnitScale);
        previewRenderOptions.SetOutputDirectory(tempDirectory);
        previewRenderOptions.SetResolution(resolution);
        previewRenderOptions.SetCamera(camera);
        previewRenderOptions.SetSaveBlenderFile(true);
        previewRenderOptions.AddLights(lights);

        Task.Run(async () =>
        {
            // Render the preview image
            RenderManager.Render(previewRenderOptions);
            
            // Update the UI to display the preview image
            Dispatcher.UIThread.Post(() =>
            {
                PreviewImage.Source = new Bitmap(tempDirectory + uuid + ".png");
                PreviewImage.IsVisible = true;
                LoadingImage.IsVisible = false;
                PreviewButton.IsEnabled = true;
                PreviewButton.Content = "Preview";
            });
        });
    }
}