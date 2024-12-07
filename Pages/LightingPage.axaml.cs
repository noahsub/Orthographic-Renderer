////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// LightingPage.axaml.cs
// This file contains the logic for the LightingPage.
//
// Copyright (C) 2024 noahsub
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// IMPORTS
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading;
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

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// NAMESPACE
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Orthographic.Renderer.Pages;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// LIGHTING PAGE CLASS
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
public partial class LightingPage : UserControl
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // GLOBALS
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// The list of light options for one point lighting.
    /// </summary>
    private List<LightOptions> _onePointLighting;

    /// <summary>
    /// The list of light options for three point lighting.
    /// </summary>
    private List<LightOptions> _threePointLighting;

    /// <summary>
    /// The list of light options for overhead lighting.
    /// </summary>
    private List<LightOptions> _overheadLighting;

    /// <summary>
    /// The maximum dimension of the model.
    /// </summary>
    private float _maxDimension;

    /// <summary>
    /// A token source for cancelling the render tasks.
    /// </summary>
    private CancellationTokenSource _cancelToken = new();

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // INITIALIZATION
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Creates a new instance of the <see cref="LightingPage"/> class.
    /// </summary>
    public LightingPage()
    {
        InitializeComponent();

        // Initialize the light options
        _onePointLighting = [new LightOptions()];

        _threePointLighting = [new LightOptions(), new LightOptions(), new LightOptions()];

        _overheadLighting = [new LightOptions()];

        // Initialize the UI
        Dispatcher.UIThread.Post(() =>
        {
            // Set the colour picker to black
            BackgroundColourSelector.ColourPicker.Color = Colors.Black;
            BackgroundColourSelector.ColourRectangle.Fill = new SolidColorBrush(Colors.Black);
            // Bind an event to the colour picker
            BackgroundColourSelector.ColourChanged += BackgroundColourChanged_Event;

            // Set the aspect ratio 16:9 as the default
            AspectRatio16X9ToggleButton.IsChecked = true;

            // Add resolution buttons to the grid
            for (var i = 0; i < 12; i++)
            {
                var resolutionButton = new Button();
                resolutionButton.Content = Resolution.AspectRatio16X9[i];
                resolutionButton.Click += ResolutionButton_OnClick;
                resolutionButton.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
                resolutionButton.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch;
                resolutionButton.HorizontalContentAlignment = Avalonia
                    .Layout
                    .HorizontalAlignment
                    .Center;
                resolutionButton.VerticalContentAlignment = Avalonia
                    .Layout
                    .VerticalAlignment
                    .Center;
                resolutionButton.Margin = new Thickness(5);
                Grid.SetRow(resolutionButton, (i / 6) + 1);
                Grid.SetColumn(resolutionButton, i % 6);
                ResolutionGrid.Children.Add(resolutionButton);
            }

            WidthTextBox.TextChanged += Option_Changed;
            HeightTextBox.TextChanged += Option_Changed;
            CameraOrientation.OrientationChanged += Option_Changed;
            CameraDistance.ValueChanged += Option_Changed;
            BackgroundColourSelector.ColourChanged += Option_Changed;

            OnePointLightingButton.Click += Option_Changed;
            ThreePointLightingButton.Click += Option_Changed;
            OverheadLightingButton.Click += Option_Changed;
            
            AddLightButton.Click += Option_Changed;
            ClearButton.Click += Option_Changed;
        });
    }

    /// <summary>
    /// Method that is called when the page is navigated to.
    /// </summary>
    public void Load()
    {
        // Set the file label to the name of the model file
        FileLabel.Content = Path.GetFileName(DataManager.ModelPath);
        
        // Calculate the maximum dimension of the model
        var dimensions = ModelManager.GetDimensions(DataManager.ModelPath);
        _maxDimension =
            new[] { dimensions.X, dimensions.Y, dimensions.Z }.Max() * DataManager.UnitScale;
        // Set the camera distance to the maximum dimension multiplied by 2
        CameraDistance.SetValue(_maxDimension * 2);
        CameraDistance.SetSliderBounds(0, _maxDimension * 10, 0.2);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // EVENT HANDLERS
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Sets the resolution according to the resolution button clicked.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ResolutionButton_OnClick(object? sender, RoutedEventArgs e)
    {
        // Get the button that was clicked
        var button = (Button)sender!;

        if (button.Content is null)
        {
            return;
        }

        // Set the resolution text boxes to the resolution of the button
        WidthTextBox.Text = Resolution
            .ResolutionDictionary[button.Content.ToString() ?? string.Empty]
            .Item1.ToString();
        HeightTextBox.Text = Resolution
            .ResolutionDictionary[button.Content.ToString() ?? string.Empty]
            .Item2.ToString();
    }

    /// <summary>
    /// Takes the user back to the ModelPage.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void BackButton_OnClick(object? sender, RoutedEventArgs e)
    {
        // Switch to the ModelPage
        var mainWindow = (MainWindow)this.VisualRoot!;
        NavigationManager.SwitchPage(mainWindow, "ModelPage");
    }

    /// <summary>
    /// Takes the user to the ViewsPage.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ViewsButton_OnClick(object? sender, RoutedEventArgs e)
    {
        // Save the current render options
        SaveOptions();

        // Switch to the RenderPage
        var mainWindow = (MainWindow)this.VisualRoot!;
        NavigationManager.SwitchPage(mainWindow, "ViewsPage");
    }

    /// <summary>
    /// Detects if the background colour has changed and updates the background accordingly.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void BackgroundColourChanged_Event(object? sender, EventArgs e)
    {
        BackgroundRectangle.Fill = new SolidColorBrush(BackgroundColourSelector.ColourPicker.Color);
    }

    /// <summary>
    /// Handles the event when the aspect ratio toggle buttons are clicked.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void AspectRatioToggleButton_OnClick(object? sender, RoutedEventArgs e)
    {
        UncheckAllAspectRatioButtons();

        // Determine the clicked button and set the corresponding aspect ratio
        var clickedButton = (ToggleButton)sender!;
        clickedButton.IsChecked = true;

        // Set the aspect ratios according to the clicked button
        var aspectRatios = GetAspectRatios(clickedButton);

        // Set the content of the resolution buttons to the aspect ratios
        SetResolutionButtonsContent(aspectRatios);
    }

    /// <summary>
    /// Handles the event when the one point lighting button is clicked.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnePointLightingButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var settings = new List<(string orientation, int power)> { ("front", 1000) };
        SetupLighting(_onePointLighting, settings);
    }

    /// <summary>
    /// Handles the event when the three point lighting button is clicked.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ThreePointLightingButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var settings = new List<(string orientation, int power)>
        {
            ("top-right-back", 200),
            ("top-back-left", 1000),
            ("top-left-front", 800),
        };
        SetupLighting(_threePointLighting, settings);
    }

    /// <summary>
    /// Handles the event when the overhead lighting button is clicked.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OverheadLightingButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var settings = new List<(string orientation, int power)> { ("top", 1000) };
        SetupLighting(_overheadLighting, settings);
    }

    /// <summary>
    /// Adds a light to the light stack panel.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void AddLightButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var light = new LightOptions();
        AddLightEventHandlers(light);
        LightOptionsStackPanel.Children.Add(light);
    }

    /// <summary>
    /// Clears the light stack panel.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ClearButton_OnClick(object? sender, RoutedEventArgs e)
    {
        LightOptionsStackPanel.Children.Clear();
    }

    /// <summary>
    /// When preview options are changed, render the preview.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Option_Changed(object? sender, EventArgs e)
    {
        Debug.WriteLine($"Triggered by {sender} with name {((Control)sender!).Name}");
        RenderPreview();
    }

    /// <summary>
    /// Renders a preview image of the model according to the current options.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void PreviewButton_OnClick(object? sender, RoutedEventArgs e)
    {
        RenderPreview();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // LIGHTING METHODS
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Collects the lights in the light stack panel.
    /// </summary>
    /// <returns></returns>
    private List<Light> GetLights()
    {
        // Create a list of lights
        var lights = new List<Light>();

        // Iterate through the light options and add them to the list
        foreach (var lightOption in LightOptionsStackPanel.Children)
        {
            var lightOptions = (LightOptions)lightOption;
            var lightOrientation = lightOptions.LightOrientationSelector.CurrentOrientation.Name;
            // We only want the first 7 characters of the hex colour, meaning the alpha channel is ignored
            var lightColour = lightOptions.LightColourSelector.GetHexColour()[..7];
            var lightPower = float.Parse(lightOptions.PowerValueSelector.ValueTextBox.Text ?? "0");
            var lightSize = float.Parse(lightOptions.SizeValueSelector.ValueTextBox.Text ?? "0");
            var lightDistance = float.Parse(
                lightOptions.DistanceValueSelector.ValueTextBox.Text ?? "0"
            );
            var lightPosition = RenderManager.GetPosition(lightOrientation, lightDistance);
            var light = new Light(lightPosition, lightColour, lightPower, lightSize, lightDistance);
            lights.Add(light);
        }

        // Return the list of lights
        return lights;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // VERIFICATION
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Checks if the options are valid and sets them to default if they are not.
    /// </summary>
    private void VerifyOptions()
    {
        // Verify the width text box and set it to 1920 if it is invalid
        if (string.IsNullOrEmpty(WidthTextBox.Text) || !int.TryParse(WidthTextBox.Text, out _))
        {
            WidthTextBox.Text = "1920";
        }

        // Verify the height text box and set it to 1080 if it is invalid
        if (string.IsNullOrEmpty(HeightTextBox.Text) || !int.TryParse(HeightTextBox.Text, out _))
        {
            HeightTextBox.Text = "1080";
        }

        // Verify the width text's value and set it to 1920 if it is invalid
        if (int.Parse(WidthTextBox.Text) <= 0)
        {
            WidthTextBox.Text = "1920";
        }

        // Verify the height text's value and set it to 1080 if it is invalid
        if (int.Parse(HeightTextBox.Text) <= 0)
        {
            HeightTextBox.Text = "1080";
        }

        // Verify the camera distance text box and set it to 0 if it is invalid
        if (
            string.IsNullOrEmpty(CameraDistance.ValueTextBox.Text)
            || !float.TryParse(CameraDistance.ValueTextBox.Text, out _)
        )
        {
            CameraDistance.SetValue(0);
        }

        // Verify the light options in the light stack panel
        foreach (var lightOptions in LightOptionsStackPanel.Children.Cast<LightOptions?>())
        {
            lightOptions?.VerifyOptions();
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // SAVING
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Stores the current options in the data manager.
    /// </summary>
    private void SaveOptions()
    {
        // Verify the options
        VerifyOptions();

        // Store the camera distance
        DataManager.CameraDistance = float.Parse(CameraDistance.ValueTextBox.Text ?? "0");

        // Store the resolution
        DataManager.Resolution = new Entities.Resolution(
            int.Parse(WidthTextBox.Text ?? "1920"),
            int.Parse(HeightTextBox.Text ?? "1080")
        );

        // Store the lights
        DataManager.Lights = GetLights();

        // Store the background colour
        DataManager.BackgroundColour = BackgroundColourSelector.ColourPicker.Color;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // HELPER FUNCTIONS
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Unchecks all aspect ratio toggle buttons.
    /// </summary>
    private void UncheckAllAspectRatioButtons()
    {
        AspectRatio16X9ToggleButton.IsChecked = false;
        AspectRatio21X9ToggleButton.IsChecked = false;
        AspectRatio4X3ToggleButton.IsChecked = false;
        AspectRatio1X1ToggleButton.IsChecked = false;
    }

    /// <summary>
    /// Gets the aspect ratios according to the clicked button.
    /// </summary>
    /// <param name="clickedButton">The aspect ratio button that was clicked.</param>
    /// <returns>A string array of aspect ratios.</returns>
    private string[] GetAspectRatios(ToggleButton clickedButton)
    {
        return clickedButton switch
        {
            _ when clickedButton == AspectRatio1X1ToggleButton =>
                Resolution.AspectRatio1X1.ToArray(),
            _ when clickedButton == AspectRatio4X3ToggleButton =>
                Resolution.AspectRatio4X3.ToArray(),
            _ when clickedButton == AspectRatio16X9ToggleButton =>
                Resolution.AspectRatio16X9.ToArray(),
            _ => Resolution.AspectRatio21X9.ToArray(),
        };
    }

    /// <summary>
    /// Sets the content of the resolution buttons according to the aspect ratios.
    /// </summary>
    /// <param name="aspectRatios"></param>
    private void SetResolutionButtonsContent(string[] aspectRatios)
    {
        for (var i = 0; i < 12; i++)
        {
            // Get button from ResolutionGrid starting from the 7th element
            var resolutionButton = (Button)ResolutionGrid.Children[i + 6];
            resolutionButton.Content = aspectRatios[i];
        }
    }

    /// <summary>
    /// Sets up the lighting options according to the settings provided.
    /// </summary>
    /// <param name="lightingOptions">The list of light options to set up.</param>
    /// <param name="settings">The settings for the light options.</param>
    private void SetupLighting(
        List<LightOptions> lightingOptions,
        List<(string orientation, int power)> settings
    )
    {
        // Clear the light stack panel
        LightOptionsStackPanel.Children.Clear();

        // Set the light options using the provided settings
        for (var i = 0; i < lightingOptions.Count; i++)
        {
            var light = lightingOptions[i];
            light.SetOrientation(settings[i].orientation);
            light.SetColour(Colors.White);
            light.SetPower(settings[i].power);
            light.SetSize(3);
            light.SetDistance(_maxDimension * 80);
            AddLightEventHandlers(light);
            LightOptionsStackPanel.Children.Add(light);
        }
    }

    /// <summary>
    /// Adds event handlers to the light options.
    /// </summary>
    /// <param name="light"></param>
    private void AddLightEventHandlers(LightOptions light)
    {
        light.LightOrientationSelector.OrientationChanged += Option_Changed;
        light.LightColourSelector.ColourChanged += Option_Changed;
        light.PowerValueSelector.ValueChanged += Option_Changed;
        light.SizeValueSelector.ValueChanged += Option_Changed;
        light.DistanceValueSelector.ValueChanged += Option_Changed;
        light.RemoveButton.Click += Option_Changed;
    }

    /// <summary>
    /// Renders the preview image of the model.
    /// </summary>
    private void RenderPreview()
    {
        // Cancel all running render tasks
        _cancelToken.Cancel();
        _cancelToken = new CancellationTokenSource();
        var token = _cancelToken.Token;

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

        // Set the base resolution
        var resolution = DataManager.Resolution;
        resolution = RenderManager.CalculatePreviewResolution(resolution);

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
        previewRenderOptions.SetBackgroundColour(DataManager.BackgroundColour);

        Task.Run(async () =>
        {
            // Render the preview image
            var success = await RenderManager.Render(previewRenderOptions, "preview", token);

            if (!success)
            {
                return;
            }

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
