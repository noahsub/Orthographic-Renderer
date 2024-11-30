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
using Avalonia.Threading;
using Orthographic.Renderer.Constants;
using Orthographic.Renderer.Controls;
using Orthographic.Renderer.Entities;
using Orthographic.Renderer.Managers;
using Orthographic.Renderer.Windows;
using RenderOptions = Orthographic.Renderer.Entities.RenderOptions;

namespace Orthographic.Renderer.Pages;

public partial class RenderPage : UserControl
{
    private CancellationTokenSource _cancelToken = new();
    
    public RenderPage()
    {
        InitializeComponent();

        ThreadsNumericUpDown.Minimum = 0;
        ThreadsNumericUpDown.Maximum = 100;
        ThreadsNumericUpDown.Value = 1;
        ThreadsNumericUpDown.IsEnabled = false;
        
        SequentialToggleButton.IsChecked = true;
        ParallelToggleButton.IsChecked = false;
        
        CancelButton.IsVisible = false;
        CancelButton.IsEnabled = false;
        
        // Set the file label to the name of the model file.
        FileLabel.Content = Path.GetFileName(DataManager.ModelPath);

        PopulateRenderQueue();
    }

    private void PopulateRenderQueue()
    {
        RenderItems.ClearItems();

        // Populate the render items.
        foreach (var view in DataManager.SelectedViews)
        {
            // Create a new render item.
            var renderItem = new RenderQueueItem();
            // Set the properties of the render item.
            renderItem.Name.Content = RenderManager.GetFormattedViewName(view);
            renderItem.Key = view;
        
            // Enqueue the render item.
            RenderItems.EnqueuePending(renderItem);
        
            // Add the render item to the display.
            RenderItems.AddToDisplay(renderItem);
        }
    }

    private void BackButton_OnClick(object? sender, RoutedEventArgs e)
    {
        // Switch to the ModelPage
        var mainWindow = (MainWindow)this.VisualRoot!;
        NavigationManager.SwitchPage(mainWindow, "ViewsPage");
    }

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
        
        RenderButton.IsEnabled = true;
        CancelButton.IsVisible = false;
        CancelButton.IsEnabled = false;
    }

    private async void RenderButton_OnClick(object? sender, RoutedEventArgs e)
    {
        PopulateRenderQueue();
        
        var outputDirectory = OutputBrowsableDirectoryTextBox.PathTextBox.Text ?? string.Empty;
        if (string.IsNullOrWhiteSpace(outputDirectory))
        {
            OutputBrowsableDirectoryTextBox.PathTextBox.BorderBrush = Brushes.Red;
            return;
        }

        else
        {
            OutputBrowsableDirectoryTextBox.PathTextBox.BorderBrush = Brushes.Transparent;
        }
        
        RenderButton.IsEnabled = false;
        CancelButton.IsEnabled = true;
        CancelButton.IsVisible = true;
        
        WindowManager.CloseAllRenderCompleteWindows();
        
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
        
        var mode = "sequential";
        if (ParallelToggleButton.IsChecked == true)
        {
            mode = "parallel";
        }

        switch (mode)
        {
            case "sequential":
                while (RenderItems.PendingQueue.Count > 0)
                {
                    var renderItem = RenderItems.DequeuePending();
                    await RenderNext(renderItem, token);
                }
                break;
            case "parallel":
                var threads = (int)ThreadsNumericUpDown.Value!;
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
        
        // Stop the timer.
        var timeEnded = DateTime.Now;
        timerRunning = false;

        // Display the render statistics in a new window.
        DisplayRenderStats(timeStarted, timeEnded);

        // Play a sound if the setting is enabled.
        SoundManager.PlaySound("Assets/Sounds/ping.mp3");
        
        CancelButton.IsVisible = false;
        CancelButton.IsEnabled = false;
        
        RenderButton.IsEnabled = true;
    }
    
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

    private async Task RenderNext(RenderQueueItem renderItem, CancellationToken token)
    {
        RenderOptions renderOptions = new RenderOptions();
        renderOptions.SetName(Guid.NewGuid().ToString().Replace("-", ""));
        renderOptions.SetModel(DataManager.ModelPath);
        renderOptions.SetUnit(DataManager.UnitScale);
        renderOptions.SetOutputDirectory(OutputBrowsableDirectoryTextBox.PathTextBox.Text ?? string.Empty);
        renderOptions.SetResolution(DataManager.Resolution);
                    
        var cameraDistance = DataManager.CameraDistance;
        var cameraPosition = RenderManager.GetPosition(renderItem.Key, cameraDistance);
        var camera = new Camera(cameraDistance, cameraPosition);
        renderOptions.SetCamera(camera);
                    
        renderOptions.AddLights(DataManager.Lights);
        renderOptions.SetSaveBlenderFile(false);

        renderItem.SetStatus(RenderStatus.InProgress);
        var success = await RenderManager.Render(renderOptions, token);
                    
        // Update the status of the render item.
        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            // If the render was successful, set the status to completed.
            if (success)
            {
                renderItem.SetStatus(RenderStatus.Completed);
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
}