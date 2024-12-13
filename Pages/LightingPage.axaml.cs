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
using Orthographic.Renderer.Interfaces;
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
public partial class LightingPage : UserControl, IPage
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // GLOBALS
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    private RenderOptions _previewRenderOptions = new();

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
        Initialize();
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

        var resolution = ImageManager.ConvertResolutionNameToResolution(
            button.Content.ToString() ?? string.Empty
        );
        WidthTextBox.Text = resolution.Width.ToString();
        HeightTextBox.Text = resolution.Height.ToString();

        // Change the colour of the text boxes to indicate that the resolution has changed
        var originalColour = WidthTextBox.Foreground;

        Task.Run(async () =>
        {
            // Get the primary accent colour from the resource dictionary
            var primaryColourHex =
                Application.Current?.Resources["PrimaryAccent"]?.ToString()
                ?? Colors.SeaGreen.ToString();
            var highlightColour = Color.Parse(primaryColourHex);

            // Change the colour of the text boxes to the highlight colour
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                WidthTextBox.Foreground = new SolidColorBrush(highlightColour);
                HeightTextBox.Foreground = new SolidColorBrush(highlightColour);
            });

            // Wait for 1 second
            await Task.Delay(1000);

            // Change the colour of the text boxes back to the original colour
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                WidthTextBox.Foreground = originalColour;
                HeightTextBox.Foreground = originalColour;
            });
        });
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
        // Save the current options
        RenderManager.SaveRenderOptions(_previewRenderOptions);

        // Switch to the RenderPage
        var mainWindow = (MainWindow)this.VisualRoot!;
        NavigationManager.SwitchPage(mainWindow, "ViewsPage");
    }

    /// <summary>
    /// Detects if the background color has changed and updates the background accordingly.
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
        LightSetupItemsStackPanel.Children.Clear();
        var lights = SceneManager.SetupOnePointLighting();
        foreach (var light in lights)
        {
            LightSetupItemsStackPanel.Children.Add(CreateLight(light));
        }
    }

    /// <summary>
    /// Handles the event when the three point lighting button is clicked.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ThreePointLightingButton_OnClick(object? sender, RoutedEventArgs e)
    {
        LightSetupItemsStackPanel.Children.Clear();
        var lights = SceneManager.SetupThreePointLighting();
        foreach (var light in lights)
        {
            LightSetupItemsStackPanel.Children.Add(CreateLight(light));
        }
    }

    /// <summary>
    /// Handles the event when the overhead lighting button is clicked.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OverheadLightingButton_OnClick(object? sender, RoutedEventArgs e)
    {
        LightSetupItemsStackPanel.Children.Clear();
        var lights = SceneManager.SetupOverheadLighting();
        foreach (var light in lights)
        {
            LightSetupItemsStackPanel.Children.Add(CreateLight(light));
        }
    }

    /// <summary>
    /// Adds a light to the light stack panel.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void AddLightButton_OnClick(object? sender, RoutedEventArgs e)
    {
        LightSetupItem lightSetupItem = new();
        BindLightOptionChanged(lightSetupItem);
        LightSetupItemsStackPanel.Children.Add(lightSetupItem);
    }

    /// <summary>
    /// Clears the light stack panel.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ClearButton_OnClick(object? sender, RoutedEventArgs e)
    {
        LightSetupItemsStackPanel.Children.Clear();
    }

    /// <summary>
    /// When preview options are changed, render the preview.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Option_Changed(object? sender, EventArgs e)
    {
        if (LightSetupItemsStackPanel.Children.Count == 0)
        {
            NoLightsImage.IsVisible = true;
        }
        else
        {
            NoLightsImage.IsVisible = false;
        }

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
    // PREVIEW
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Renders the preview image of the model.
    /// </summary>
    private void RenderPreview()
    {
        // Cancel all running render tasks
        _cancelToken.Cancel();
        _cancelToken = new CancellationTokenSource();
        var token = _cancelToken.Token;

        // Update the UI to indicate that rendering is in progress
        RenderStarted();

        // Generate a unique identifier for the preview
        var uuid = Guid.NewGuid().ToString().Replace("-", "");
        // Set the preview render options
        SetPreviewRenderOptions(uuid);

        // Create a copy of the preview render options
        var previewRenderOptionsCopy = _previewRenderOptions.Copy();
        // Lower the resolution for the preview
        previewRenderOptionsCopy.Resolution = ImageManager.ResizeResolution(
            previewRenderOptionsCopy.Resolution,
            600
        );

        // Render the preview image
        Task.Run(async () =>
        {
            // Render the preview image
            var success = await RenderManager.Render(previewRenderOptionsCopy, "preview", token);

            if (!success)
            {
                return;
            }

            RenderFinished(uuid);
        });
    }

    /// <summary>
    /// Set the preview render options according to the current UI settings.
    /// </summary>
    /// <param name="name">The name of the image to render.</param>
    private void SetPreviewRenderOptions(string name)
    {
        // Name
        _previewRenderOptions.SetName(name);

        // Model
        _previewRenderOptions.SetModel(DataManager.ModelPath);

        // Unit
        _previewRenderOptions.SetUnit(DataManager.UnitScale);

        // Output directory
        _previewRenderOptions.SetOutputDirectory(FileManager.GetTempDirectoryPath());

        // Resolution
        var resolution = GetResolution();
        _previewRenderOptions.SetResolution(resolution);

        // Camera
        _previewRenderOptions.SetCamera(GetCamera());

        // Lights
        var lightSetupItems = LightSetupItemsStackPanel.Children.OfType<LightSetupItem>().ToList();
        var lights = SceneManager.GetLights(lightSetupItems);
        _previewRenderOptions.Lights.Clear();
        _previewRenderOptions.AddLights(lights);

        // Background color
        _previewRenderOptions.SetBackgroundColour(BackgroundColourSelector.ColourPicker.Color);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // HELPER FUNCTIONS
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Get the resolution from the width and height text boxes.
    /// If the values are invalid, the default resolution is returned.
    /// </summary>
    /// <returns>A resolution with the width and height from the text boxes.</returns>
    private Entities.Resolution GetResolution()
    {
        // Get and validate the width
        int width;
        bool widthValid = int.TryParse(WidthTextBox.Text, out width);

        // Get and validate the height
        int height;
        bool heightValid = int.TryParse(HeightTextBox.Text, out height);

        // If the values are invalid, set the resolution to the default resolution
        if (!widthValid || width <= 0)
        {
            width = Resolution.DefaultWidth;
            WidthTextBox.Text = Resolution.DefaultWidth.ToString();
        }

        if (!heightValid || height <= 0)
        {
            height = Resolution.DefaultHeight;
            HeightTextBox.Text = Resolution.DefaultHeight.ToString();
        }

        // Return the resolution
        return new Entities.Resolution(width, height);
    }

    /// <summary>
    /// Get the camera from the camera distance and orientation selectors.
    /// </summary>
    /// <returns></returns>
    private Camera GetCamera()
    {
        // Get the camera distance
        var cameraDistance = CameraDistanceValueSelector.GetValue();
        // Get the camera view
        var cameraView = CameraOrientationSelector.CurrentOrientation.Name;
        // Get the camera position
        var cameraPosition = SceneManager.GetPosition(cameraView, cameraDistance);
        // Return the camera
        return new Camera(cameraDistance, cameraPosition);
    }

    /// <summary>
    /// Create a LightSetupItem control from a Light object.
    /// </summary>
    /// <param name="light"></param>
    /// <returns></returns>
    private LightSetupItem CreateLight(Light light)
    {
        var lightSetupItem = new LightSetupItem();
        lightSetupItem.LightOrientationSelector.SetOrientation(light.View);

        var colourComponents = light.Colour.Split(',');
        var red = byte.Parse(colourComponents[0]);
        var green = byte.Parse(colourComponents[1]);
        var blue = byte.Parse(colourComponents[2]);
        var alpha = byte.Parse(colourComponents[3]);
        var colour = Avalonia.Media.Color.FromArgb(alpha, red, green, blue);
        lightSetupItem.SetColour(colour);

        lightSetupItem.SetPower(light.Power);
        lightSetupItem.SetSize(light.Size);
        lightSetupItem.SetDistance(light.Distance);

        BindLightOptionChanged(lightSetupItem);

        return lightSetupItem;
    }

    /// <summary>
    /// Sets the UI to indicate that rendering has started.
    /// </summary>
    private void RenderStarted()
    {
        // Update the UI to indicate that rendering is in progress.
        Dispatcher.UIThread.Post(() =>
        {
            PreviewButton.Content = "Rendering";
            PreviewButton.IsEnabled = false;
            PreviewImage.IsVisible = false;
            LoadingImage.IsVisible = true;
        });
    }

    /// <summary>
    /// Sets the UI to indicate that rendering has finished.
    /// </summary>
    /// <param name="uuid"></param>
    private void RenderFinished(string uuid)
    {
        var tempDirectory = FileManager.GetTempDirectoryPath();

        // Update the UI to indicate that rendering is complete.
        Dispatcher.UIThread.Post(() =>
        {
            // Display the image and load it into memory
            PreviewImage.Source = new Bitmap(tempDirectory + uuid + ".png");
            // Delete the temp image and blender file
            File.Delete(tempDirectory + uuid + ".png");
            File.Delete(tempDirectory + uuid + ".blend");
            PreviewImage.IsVisible = true;
            LoadingImage.IsVisible = false;
            PreviewButton.IsEnabled = true;
            PreviewButton.Content = "Preview";
        });
    }

    /// <summary>
    /// Creates the resolution buttons for the user to select.
    /// </summary>
    private void CreateResolutionButtons()
    {
        // Create the aspect ratio buttons
        for (var i = 0; i < 12; i++)
        {
            // Create the button
            var resolutionButton = new Button();
            // Set the button content
            resolutionButton.Content = Resolution.AspectRatio16X9[i];
            // Set the button click event
            resolutionButton.Click += ResolutionButton_OnClick;
            // Set the button properties
            resolutionButton.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
            resolutionButton.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch;
            resolutionButton.HorizontalContentAlignment = Avalonia
                .Layout
                .HorizontalAlignment
                .Center;
            resolutionButton.VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center;
            resolutionButton.Margin = new Thickness(5);
            // Add the button to the grid
            Grid.SetRow(resolutionButton, (i / 6) + 1);
            Grid.SetColumn(resolutionButton, i % 6);
            ResolutionGrid.Children.Add(resolutionButton);
        }
    }

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
    /// Bind the Option_Changed event handler to UI components.
    /// </summary>
    private void BindOptionChanged()
    {
        WidthTextBox.TextChanged += Option_Changed;
        HeightTextBox.TextChanged += Option_Changed;
        CameraOrientationSelector.OrientationChanged += Option_Changed;
        CameraDistanceValueSelector.ValueChanged += Option_Changed;
        BackgroundColourSelector.ColourChanged += Option_Changed;

        OnePointLightingButton.Click += Option_Changed;
        ThreePointLightingButton.Click += Option_Changed;
        OverheadLightingButton.Click += Option_Changed;

        AddLightButton.Click += Option_Changed;
        ClearButton.Click += Option_Changed;
    }

    /// <summary>
    /// Bind the Option_Changed event handler to a LightSetupItem control.
    /// </summary>
    private void BindLightOptionChanged(LightSetupItem lightSetupItem)
    {
        lightSetupItem.LightOrientationSelector.OrientationChanged += Option_Changed;
        lightSetupItem.LightColourSelector.ColourChanged += Option_Changed;
        lightSetupItem.PowerValueSelector.ValueChanged += Option_Changed;
        lightSetupItem.SizeValueSelector.ValueChanged += Option_Changed;
        lightSetupItem.DistanceValueSelector.ValueChanged += Option_Changed;
        lightSetupItem.RemoveButton.Click += Option_Changed;
    }

    /// <summary>
    /// Sets the optimal camera distance for the model.
    /// </summary>
    public void SetOptimalCameraDistance()
    {
        // Set the optimal camera distance
        var optimalCameraDistance = SceneManager.ComputeOptimalCameraDistance(
            DataManager.ModelMaxDimension * DataManager.UnitScale
        );
        CameraDistanceValueSelector.SetSliderBounds(0, optimalCameraDistance * 5, 0.2);
        CameraDistanceValueSelector.SetValue(optimalCameraDistance);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // IPAGE IMPLEMENTATION
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Initializes the LightingPage.
    /// </summary>
    public void Initialize()
    {
        // Initialize the page
        InitializeComponent();

        // Add color changed event for the background color selector
        BackgroundColourSelector.ColourChanged += BackgroundColourChanged_Event;

        // Create the resolution buttons
        CreateResolutionButtons();
    }

    /// <summary>
    /// When the page is first loaded by the user.
    /// </summary>
    public void OnFirstLoad()
    {
        // Bind the option changed event handler to the UI components
        BindOptionChanged();

        // Set up three point lighting by default
        LightSetupItemsStackPanel.Children.Clear();
        var lights = SceneManager.SetupThreePointLighting();
        foreach (var light in lights)
        {
            LightSetupItemsStackPanel.Children.Add(CreateLight(light));
        }

        BackgroundColourSelector.ColourPicker.Color = Color.Parse("#0B0B0C");
    }

    /// <summary>
    /// When the page is navigated to.
    /// </summary>
    public void OnNavigatedTo()
    {
        // Set the file name
        FileLabel.Content = Path.GetFileName(DataManager.ModelPath);

        // Render Preview
        RenderPreview();
    }
}
