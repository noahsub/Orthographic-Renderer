////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// RenderPage.axaml.cs
// This file contains the logic for the RenderPage.
//
// Author(s): https://github.com/noahsub
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// IMPORTS
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Orthographic.Renderer.Constants;
using Orthographic.Renderer.Controls;
using Orthographic.Renderer.Managers;
using Orthographic.Renderer.Windows;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// NAMESPACE
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Orthographic.Renderer.Pages;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// RENDER PAGE CLASS
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
public partial class RenderPage : UserControl
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // GLOBALS
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Token to cancel renders.
    /// </summary>
    private CancellationTokenSource _cancelToken;

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // INITIALIZATION
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Initializes a new instance of the <see cref="RenderPage"/> class.
    /// </summary>
    public RenderPage()
    {
        InitializeComponent();

        // Set the layout of the ViewStackGrid.
        ViewStackGrid.SetLayout(5);

        // Populate the views.
        PopulateViews(RenderManager.RenderViews);

        // Set the file label to the name of the model file.
        FileLabel.Content = Path.GetFileName(DataManager.ModelPath);

        // Hide the cancel button.
        CancelButton.IsVisible = false;
        CancelButton.IsEnabled = false;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // SELECT AND SORT
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Selects the views based on the selection options.
    /// </summary>
    private void SelectViews()
    {
        // Select the views based on the selection options.
        foreach (var control in ViewStackGrid.GetItems())
        {
            var view = (ViewSelection)control;

            // Select all views.
            if (ViewSelectOptions.AllButton.IsChecked == true)
            {
                view.SetSelected(true);
            }
            // Deselect all views.
            else if (ViewSelectOptions.NoneButton.IsChecked == true)
            {
                view.SetSelected(false);
            }
            // Invert the selection of all views.
            else if (ViewSelectOptions.InvertButton.IsChecked == true)
            {
                view.SetSelected(!view.GetSelected());
            }
        }

        // Clear the selection options.
        ViewSelectOptions.ClearSelection();
    }

    /// <summary>
    /// Sorts the views based on the selected faces.
    /// </summary>
    public void SortViews()
    {
        var selectedViews = GetSelectedViews();
        ViewStackGrid.ClearItems();
        var selectedFaces = ViewSortOptions.GetSelectedFaces();
        var sortedViews = RenderManager.SortViews(selectedFaces);
        PopulateViews(sortedViews);
        foreach (var control in ViewStackGrid.GetItems())
        {
            var viewSelection = (ViewSelection)control;
            if (selectedViews.Contains(viewSelection.Key))
            {
                viewSelection.SetSelected(true);
            }
        }
    }

    /// <summary>
    /// Gets the selected views.
    /// </summary>
    /// <returns>A list of selected views.</returns>
    private List<string> GetSelectedViews()
    {
        List<string> selectedViews = [];

        foreach (var control in ViewStackGrid.GetItems())
        {
            var viewSelection = (ViewSelection)control;
            if (viewSelection.GetSelected())
            {
                selectedViews.Add(viewSelection.Key);
            }
        }

        return selectedViews;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // VIEWS
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Populates the views in the ViewStackGrid.
    /// </summary>
    /// <param name="views">The list of views to populate.</param>
    private void PopulateViews(List<string> views)
    {
        // foreach view in the list of views
        foreach (var view in views)
        {
            // create a new ViewSelection control
            var viewSelection = new ViewSelection();
            viewSelection.SetName(view);
            viewSelection.SetImage(view);

            // add the view to the ViewStackGrid
            ViewStackGrid.AddItem(viewSelection);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // LOCKING
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Locks the page controls during rendering.
    /// </summary>
    private void LockPage()
    {
        Settings.IsEnabled = false;
        ViewSelectOptions.IsEnabled = false;
        ViewSortOptions.IsEnabled = false;
        ViewStackGrid.IsEnabled = false;
        RenderButton.IsEnabled = false;
        CancelButton.IsVisible = true;
        CancelButton.IsEnabled = true;
    }

    /// <summary>
    /// Unlocks the page controls after rendering.
    /// </summary>
    private void UnlockPage()
    {
        Settings.IsEnabled = true;
        ViewSelectOptions.IsEnabled = true;
        ViewSortOptions.IsEnabled = true;
        ViewStackGrid.IsEnabled = true;
        RenderButton.IsEnabled = true;
        CancelButton.IsVisible = false;
        CancelButton.IsEnabled = false;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // RENDERING
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Renders the specified item.
    /// </summary>
    /// <param name="renderItem">The render item to process.</param>
    /// <param name="prefix">The prefix for the output files.</param>
    /// <param name="outputDir">The output directory.</param>
    /// <param name="distance">The distance for the render.</param>
    /// <param name="width">The width of the render.</param>
    /// <param name="height">The height of the render.</param>
    /// <param name="scale">The scale of the render.</param>
    /// <param name="save">Whether to save the render.</param>
    /// <param name="token">The cancellation token.</param>
    private async Task Render(
        RenderQueueItem renderItem,
        string prefix,
        string outputDir,
        float distance,
        int width,
        int height,
        int scale,
        bool save,
        CancellationToken token
    )
    {
        // Set the status of the render item to in progress.
        renderItem.SetStatus(RenderStatus.InProgress);

        // Get the paths for the model, blender, and script.
        var blenderPath = DataManager.BlenderPath;
        var modelPath = DataManager.ModelPath;
        var scriptPath = FileManager.GetAbsolutePath("Scripts/render.py");

        // Get the position of the view.
        var position = RenderManager.GetPosition(renderItem.Key, distance);

        // Set the arguments for the blender process.
        string blenderArguments;
        if (modelPath.EndsWith(".blend"))
        {
            blenderArguments = $"-b \"{modelPath}\" -P \"{scriptPath}\" -- ";
        }
        else
        {
            blenderArguments = $"-b -P \"{scriptPath}\" -- ";
        }

        // Set the arguments for the script.
        var arguments =
            blenderArguments
            + $"--model \"{modelPath}\" "
            + $"--name {prefix} "
            + $"--output_path \"{outputDir}\" "
            + $"--resolution {width} {height} "
            + $"--scale {scale} "
            + $"--distance {distance} "
            + $"--unit {DataManager.UnitScale} "
            + $"--save {save} "
            + $"--x {position.X} "
            + $"--y {position.Y} "
            + $"--z {position.Z} "
            + $"--rx {position.Rx} "
            + $"--ry {position.Ry} "
            + $"--rz {position.Rz}";

        // Run the blender process.
        bool success;
        try
        {
            success = await Task.Run(
                () =>
                {
                    token.ThrowIfCancellationRequested();
                    return ProcessManager.RunProcessCheck(blenderPath, arguments);
                },
                token
            );
        }
        catch (TaskCanceledException)
        {
            success = false;
        }

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

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // EVENTS
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Handles the tap event of the ViewSelect button.
    /// </summary>
    private void ViewSelect_OnTapped(object? sender, TappedEventArgs e)
    {
        SelectViews();
    }

    /// <summary>
    /// Handles the tap event of the ViewSort button.
    /// </summary>
    private void ViewSort_OnTapped(object? sender, TappedEventArgs e)
    {
        SortViews();
    }

    /// <summary>
    /// Handles the click event of the Render button.
    /// </summary>
    private async void RenderButton_OnClick(object? sender, RoutedEventArgs e)
    {
        // Create a new cancellation token.
        _cancelToken = new CancellationTokenSource();
        var token = _cancelToken.Token;

        // Clear the render items.
        RenderItems.ClearItems();

        // Verify the render settings.
        if (!Settings.VerifyRenderSettings())
        {
            SoundManager.PlaySound("Assets/Sounds/error.mp3");
            return;
        }

        // Collect the render settings.
        var mode = Settings.GetMode();
        var threads = Settings.GetThreads();
        var prefix = Settings.GetPrefix();
        var outputDir = Settings.GetOutputDir();
        var distance = Settings.GetDistance();
        var width = Settings.GetResolutionWidth();
        var height = Settings.GetResolutionHeight();
        var scale = Settings.GetScale();
        var sound = Settings.GetPlaySound();
        var save = Settings.GetSave();

        // Get the selected views.
        var selectedViews = GetSelectedViews();

        // Iterate through the selected views.
        foreach (var view in selectedViews)
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

        // Start the timer.
        var timeStarted = DateTime.Now;
        var timerRunning = true;
        Task.Run(async () =>
        {
            var stopwatch = Stopwatch.StartNew();
            // ReSharper disable once LoopVariableIsNeverChangedInsideLoop
            while (timerRunning)
            {
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    TimerLabel.Content = stopwatch.Elapsed.ToString(@"hh\:mm\:ss");
                });
                await Task.Delay(100);
            }
        });

        // Lock the page controls.
        LockPage();

        // Render the items based on the mode.
        switch (mode)
        {
            case "sequential":
                // Iterate through the pending queue one item at a time.
                while (RenderItems.PendingQueue.Count > 0)
                {
                    // Dequeue the next item.
                    var renderItem = RenderItems.DequeuePending();
                    // Render the item.
                    await Render(
                        renderItem,
                        prefix,
                        outputDir,
                        distance,
                        width,
                        height,
                        scale,
                        save,
                        token
                    );
                }
                break;
            case "parallel":
                // Use a semaphore to limit the number of concurrent threads.
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
                            await Render(
                                renderItem,
                                prefix,
                                outputDir,
                                distance,
                                width,
                                height,
                                scale,
                                save,
                                token
                            );
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
        if (sound)
        {
            SoundManager.PlaySound("Assets/Sounds/ping.mp3");
        }

        // Unlock the page controls.
        UnlockPage();
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

    /// <summary>
    /// Handles the click event of the Back button.
    /// </summary>
    private void BackButton_OnClick(object? sender, RoutedEventArgs e)
    {
        // Switch to the ModelPage
        var mainWindow = (MainWindow)this.VisualRoot!;
        NavigationManager.SwitchPage(mainWindow, new ModelPage());
    }

    /// <summary>
    /// Handles the click event of the Cancel button.
    /// </summary>
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

        // Unlock the page controls
        UnlockPage();
    }
}
