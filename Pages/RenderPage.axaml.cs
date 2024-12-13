////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// RenderPage.axaml.cs
// This file contains the logic for the RenderPage.
//
// Copyright (C) 2024 noahsub
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// IMPORTS
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
using Orthographic.Renderer.Constants;
using Orthographic.Renderer.Controls;
using Orthographic.Renderer.Entities;
using Orthographic.Renderer.Interfaces;
using Orthographic.Renderer.Managers;
using Orthographic.Renderer.Windows;
using RenderOptions = Orthographic.Renderer.Entities.RenderOptions;
using Resolution = Orthographic.Renderer.Entities.Resolution;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// NAMESPACE
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Orthographic.Renderer.Pages;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// RENDER PAGE CLASS
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

/// <summary>
/// The render page of the application.
/// </summary>
public partial class RenderPage : UserControl, IPage
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // GLOBALS
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// A token source for cancelling the render tasks.
    /// </summary>
    private CancellationTokenSource _cancelToken = new();

    /// <summary>
    /// Specifies whether the blender file should be saved.
    /// </summary>
    private bool SaveBlenderFile { get; set; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // INITIALIZATION
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Creates a new instance of the <see cref="RenderPage"/> class.
    /// </summary>
    public RenderPage()
    {
        Initialize();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // QUEUE
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Populates the render queue with the selected views.
    /// </summary>
    private void PopulateRenderQueue()
    {
        // Clear the render items.
        RenderItems.ClearItems();

        // Populate the render items.
        foreach (var view in DataManager.SelectedViews)
        {
            // Create a new render item.
            var renderItem = new RenderQueueItem();
            // Set the properties of the render item.
            renderItem.NameLabel.Content = ViewManager.GetFormattedViewName(view);
            renderItem.Key = view;

            // Enqueue the render item.
            RenderItems.EnqueuePending(renderItem);

            // Add the render item to the display.
            RenderItems.AddToDisplay(renderItem);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // EVENT HANDLERS
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Handles the selection of the rendering mode.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ToggleButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (sender is ToggleButton toggleButton)
        {
            if (toggleButton == SequentialToggleButton)
            {
                SequentialToggleButton.IsChecked = true;
                ParallelToggleButton.IsChecked = false;

                ThreadsNumericUpDown.Value = 1;
                ThreadsNumericUpDown.IsEnabled = false;
            }
            else if (toggleButton == ParallelToggleButton)
            {
                ParallelToggleButton.IsChecked = true;
                SequentialToggleButton.IsChecked = false;

                ThreadsNumericUpDown.IsEnabled = true;
            }
        }
    }

    /// <summary>
    /// Navigates back to the ViewsPage.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void BackButton_OnClick(object? sender, RoutedEventArgs e)
    {
        // Switch to the ModelPage
        var mainWindow = (MainWindow)this.VisualRoot!;
        NavigationManager.SwitchPage(mainWindow, "ViewsPage");
    }

    /// <summary>
    /// Cancels the render tasks.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CancelButton_OnClick(object? sender, RoutedEventArgs e)
    {
        // Cancel all running render tasks
        _cancelToken.Cancel();

        // Find all running processes and kill them
        var processes = Process.GetProcessesByName("blender");
        foreach (var process in processes)
        {
            process.Kill();
        }

        // Enable the render button and disable the cancel button
        RenderButton.IsEnabled = true;
        CancelButton.IsVisible = false;
        CancelButton.IsEnabled = false;
    }

    /// <summary>
    /// Renders the selected views sequentially or in parallel.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void RenderButton_OnClick(object? sender, RoutedEventArgs e)
    {
        // Populate the render queue.
        PopulateRenderQueue();

        // Check the output directory
        OutputDirectoryPathSelector.FixPath();
        if (!OutputDirectoryPathSelector.CheckPath())
        {
            return;
        }

        // Modify UI to indicate that rendering has started.
        RenderingStarted();

        // Create a new cancel token.
        _cancelToken = new CancellationTokenSource();
        var token = _cancelToken.Token;

        // Start the timer.
        var timeStarted = DateTime.Now;
        var timerRunning = true;
        _ = Task.Run(
            async () =>
            {
                var stopwatch = Stopwatch.StartNew();
                // ReSharper disable once LoopVariableIsNeverChangedInsideLoop
                while (timerRunning)
                {
                    await Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        TimerLabel.Content = stopwatch.Elapsed.ToString(@"hh\:mm\:ss");
                    });
                    await Task.Delay(100, token);
                }
            },
            token
        );

        // Get the mode of rendering.
        var mode = "sequential";
        if (ParallelToggleButton.IsChecked == true)
        {
            mode = "parallel";
        }

        await RenderAll(mode, token);

        // Stop the timer.
        var timeEnded = DateTime.Now;
        timerRunning = false;

        // Display the render statistics in a new window.
        DisplayRenderStats(timeStarted, timeEnded);

        RenderingFinished();
    }
    
    private void OpenOutputDirectoryButton_OnClick(object? sender, RoutedEventArgs e)
    {
        // Fix and check the output directory path.
        OutputDirectoryPathSelector.FixPath();
        if (!OutputDirectoryPathSelector.CheckPath())
        {
            return;
        }
        
        // Get the output directory path.
        var path = OutputDirectoryPathSelector.GetPath();
        
        // Open the output directory in the default file manager according to the operating system.
        if (OperatingSystem.IsWindows())
        {
            ProcessManager.RunProcess("explorer.exe", $"\"{path.Replace('/', '\\')}\"");
        }
        
        if (OperatingSystem.IsLinux())
        {
            ProcessManager.RunProcess("xdg-open", $"\"{path}\"");
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // RENDER STATISTICS
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Displays the render statistics.
    /// </summary>
    /// <param name="timeStarted">The time the render started.</param>
    /// <param name="timeEnded">The time the render ended.</param>
    private void DisplayRenderStats(DateTime timeStarted, DateTime timeEnded)
    {
        // Create a new RenderComplete window.
        var renderComplete = new RenderComplete();

        // Set the values of the RenderComplete window.
        renderComplete.SetValues(
            timeStarted.ToString(),
            timeEnded.ToString(),
            TimerLabel.Content.ToString(),
            RenderItems.CompletedQueue.Count,
            RenderItems.FailedQueue.Count
        );

        // Show the RenderComplete window.
        renderComplete.Show();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // RENDERING
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Renders the next item in the render queue.
    /// </summary>
    /// <param name="renderItem"></param>
    /// <param name="token"></param>
    private async Task RenderNext(RenderQueueItem renderItem, CancellationToken token)
    {
        var renderOptions = CreateRenderOptions(renderItem);

        if (SaveBlenderFile == true)
        {
            SaveBlenderFile = false;
        }

        // Set the status of the render item to in progress.
        renderItem.SetStatus(RenderStatus.InProgress);

        // Render the item and get the success status.
        var success = await RenderManager.Render(renderOptions, "normal", token);

        // Update the status of the render item.
        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            // If the render was successful, set the status to completed.
            if (success)
            {
                renderItem.SetStatus(RenderStatus.Completed);
                
                var outputDirectory = renderOptions.OutputDirectory;
                if (!outputDirectory.EndsWith("/"))
                {
                    outputDirectory += "/";
                }
                
                using (var stream = File.OpenRead($"{outputDirectory}{renderOptions.Name}.png"))
                {
                    var originalImage = new Bitmap(stream);
                    var originalResolution =
                        new Resolution(originalImage.PixelSize.Width, originalImage.PixelSize.Height);
                    var resizedResolution = ImageManager.ResizeResolution(originalResolution, 600);
                    var resizedImage = originalImage.CreateScaledBitmap(new PixelSize(resizedResolution.Width, resizedResolution.Height));
                    originalImage.Dispose();
                    RenderOutputStackPanel.Children.Insert(0, new Image { Source = resizedImage });
                }
                
                if (RenderOutputStackPanel.Children.Count > 0)
                {
                    RenderOutputPlaceholderImage.IsVisible = false;
                }
            }
            // If the render failed, set the status to failed.
            else
            {
                renderItem.SetStatus(RenderStatus.Failed);
            }

            // Push the render item to the bottom of the display.
            RenderItems.RemoveFromDisplay(renderItem);
            RenderItems.AddToDisplay(renderItem);
        });

        // Enqueue the render item based on the success or failure of the render.
        if (success)
        {
            RenderItems.EnqueueCompleted(renderItem);
        }
        else
        {
            RenderItems.EnqueueFailed(renderItem);
        }
    }

    /// <summary>
    /// Creates the render options based on the saved settings and the specified output directory.
    /// </summary>
    /// <param name="renderItem">The render item to create the render options for.</param>
    /// <returns>A new render options object.</returns>
    private RenderOptions CreateRenderOptions(RenderQueueItem renderItem)
    {
        // Get the render options.
        var renderOptions = new RenderOptions();
        var modelName = Path.GetFileNameWithoutExtension(DataManager.ModelPath);
        var uuid = Guid.NewGuid().ToString().Replace("-", "");
        renderOptions.SetName($"{modelName}-{renderItem.Key}-{uuid}");
        renderOptions.SetModel(DataManager.ModelPath);
        renderOptions.SetUnit(DataManager.UnitScale);
        renderOptions.SetOutputDirectory(OutputDirectoryPathSelector.PathTextBox.Text ?? string.Empty);
        renderOptions.SetResolution(DataManager.Resolution);

        var cameraDistance = DataManager.CameraDistance;
        var cameraPosition = SceneManager.GetPosition(renderItem.Key, cameraDistance);
        var camera = new Camera(cameraDistance, cameraPosition);
        renderOptions.SetCamera(camera);

        renderOptions.AddLights(DataManager.Lights);
        renderOptions.SetBackgroundColour(DataManager.BackgroundColour);
        renderOptions.SetSaveBlenderFile(SaveBlenderFile);
        return renderOptions;
    }

    /// <summary>
    /// Render all the items in the render queue based on the specified mode.
    /// </summary>
    /// <param name="mode">Either "sequential" or "parallel" rendering mode.</param>
    /// <param name="token">The cancellation token.</param>
    private async Task RenderAll(string mode, CancellationToken token)
    {
        // Render the items based on the mode.
        switch (mode)
        {
            // Render the items sequentially.
            case "sequential":
                while (RenderItems.PendingQueue.Count > 0)
                {
                    // Dequeue the next render item.
                    var renderItem = RenderItems.DequeuePending();
                    // Render the item.
                    await RenderNext(renderItem, token);
                }
                break;
            // Render the items in parallel.
            case "parallel":
                // Get the number of threads.
                var threads = (int)ThreadsNumericUpDown.Value!;
                // Create a semaphore to limit the number of threads.
                var semaphore = new SemaphoreSlim(threads);
                // Create a list of tasks to render the items.
                var tasks = RenderItems
                    .GetItemsPending()
                    .Cast<RenderQueueItem>()
                    .Select(async renderItem =>
                    {
                        // Wait for the semaphore to be released.
                        await semaphore.WaitAsync();
                        try
                        {
                            // Render the item.
                            await RenderNext(renderItem, token);
                        }
                        finally
                        {
                            // Release the semaphore.
                            semaphore.Release();
                        }
                    });

                // Wait for all tasks to complete.
                await Task.WhenAll(tasks);
                break;
        }
    }

    /// <summary>
    /// Updates the UI to indicate that rendering has finished.
    /// </summary>
    private void RenderingFinished()
    {
        // Play a sound if the setting is enabled.
        if (CompleteSoundCheckBox.IsChecked == true)
        {
            SoundManager.PlaySound("Assets/Sounds/ping.mp3");
        }

        // Enable the render button and disable the cancel button.
        CancelButton.IsVisible = false;
        CancelButton.IsEnabled = false;
        RenderButton.IsEnabled = true;
        BackButton.IsEnabled = true;
    }

    /// <summary>
    /// Updates the UI to indicate that rendering has started.
    /// </summary>
    private void RenderingStarted()
    {
        // Clear render output stack panel.
        RenderOutputStackPanel.Children.Clear();
        
        // Disable the render button and enable the cancel button.
        RenderButton.IsEnabled = false;
        CancelButton.IsEnabled = true;
        CancelButton.IsVisible = true;
        BackButton.IsEnabled = false;

        // Close all render complete windows.
        WindowManager.CloseAllRenderCompleteWindows();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // IPAGE INTERFACE IMPLEMENTATION
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    
    /// <summary>
    /// Initializes the RenderPage.
    /// </summary>
    public void Initialize()
    {
        InitializeComponent();

        // Set the numbers of threads to be between 1 and 100 and set the default value to 1.
        ThreadsNumericUpDown.Minimum = 1;
        ThreadsNumericUpDown.Maximum = 100;
        ThreadsNumericUpDown.Value = 1;
        ThreadsNumericUpDown.IsEnabled = false;

        // Set the default toggle button to sequential.
        SequentialToggleButton.IsChecked = true;
        ParallelToggleButton.IsChecked = false;

        // Ensure the cancel button is not visible.
        CancelButton.IsVisible = false;
        CancelButton.IsEnabled = false;
    }

    /// <summary>
    /// When the page is first loaded by the user.
    /// </summary>
    public void OnFirstLoad()
    {
        return;
    }

    /// <summary>
    /// When the page is navigated to.
    /// </summary>
    public void OnNavigatedTo()
    {
        // Set the file label to the name of the model file.
        FileLabel.Content = Path.GetFileName(DataManager.ModelPath);

        var userDirectory =  FileManager.GetDownloadsDirectoryPath();
        OutputDirectoryPathSelector.SetPath(userDirectory);

        SaveBlenderFile = true;

        // Populate the render queue.
        PopulateRenderQueue();
        
        // Clear the render output stack panel.
        RenderOutputStackPanel.Children.Clear();
        RenderOutputPlaceholderImage.IsVisible = true;
    }
}
