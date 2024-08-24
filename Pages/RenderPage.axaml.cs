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
using Orthographic.Renderer.Controls;
using Orthographic.Renderer.Entities;
using Orthographic.Renderer.Managers;
using Orthographic.Renderer.Windows;

namespace Orthographic.Renderer.Pages;

public partial class RenderPage : UserControl
{
    public RenderPage()
    {
        InitializeComponent();
        ViewStackGrid.SetColumns(5);
        PopulateViews(RenderManager.RenderViews);
        FileLabel.Content = Path.GetFileName(DataManager.ModelPath);
    }

    private void PopulateViews(List<string> views)
    {
        foreach (var view in views)
        {
            var viewSelection = new ViewSelection();
            viewSelection.SetName(view);
            viewSelection.SetImage(view);
            ViewStackGrid.AddItem(viewSelection);
        }
    }

    private void SelectViews()
    {
        foreach (var control in ViewStackGrid.GetItems())
        {
            var view = (ViewSelection)control;

            if (ViewSelectOptions.AllButton.IsChecked == true)
            {
                view.SetSelected(true);
            }
            
            else if (ViewSelectOptions.NoneButton.IsChecked == true)
            {
                view.SetSelected(false);
            }

            else if (ViewSelectOptions.InvertButton.IsChecked == true)
            {
                view.SetSelected(!view.GetSelected());
            }
        }
        
        ViewSelectOptions.ClearSelection();
    }

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

    private void ViewSelect_OnTapped(object? sender, TappedEventArgs e)
    {
        SelectViews();
    }

    private void ViewSort_OnTapped(object? sender, TappedEventArgs e)
    {
        SortViews();
    }

    private async void RenderButton_OnClick(object? sender, RoutedEventArgs e)
    {
        RenderItems.ClearItems();
        
        if (!Settings.VerifyRenderSettings())
        {
            return;
        }

        var mode = Settings.GetMode();
        var threads = Settings.GetThreads();
        var prefix = Settings.GetPrefix();
        var outputDir = Settings.GetOutputDir();
        var distance = Settings.GetDistance();
        var width = Settings.GetResolutionWidth();
        var height = Settings.GetResolutionHeight();
        var scale = Settings.GetScale();
        var sound = Settings.GetPlaySound();
        
        var selectedViews = GetSelectedViews();
        foreach (var view in selectedViews)
        {
            var renderItem = new RenderQueueItem();
            renderItem.Name.Content = RenderManager.GetFormattedViewName(view);
            renderItem.Key = view;
            RenderItems.EnqueuePending(renderItem);
            RenderItems.AddToDisplay(renderItem);
        }
        
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
        
        LockPage();

        switch (mode)
        {
            case "sequential":
                while (RenderItems.PendingQueue.Count > 0)
                {
                    var renderItem = RenderItems.DequeuePending();
                    await Render(renderItem, prefix, outputDir, distance, width, height, scale);
                }
                break;
            case "parallel":
                var semaphore = new SemaphoreSlim(threads);

                var tasks = RenderItems
                    .GetItemsPending()
                    .Cast<RenderQueueItem>()
                    .Select(async renderItem =>
                    {
                        await semaphore.WaitAsync();
                        try
                        {
                            await Render(renderItem, prefix, outputDir, distance, width, height, scale);
                        }
                        finally
                        {
                            semaphore.Release();
                        }
                    });

                await Task.WhenAll(tasks);
                break;
        }
        
        var timeEnded = DateTime.Now;
        timerRunning = false;
        
        DisplayRenderStats(timeStarted, timeEnded);

        if (sound)
        {
            PlayCompleteSound();
        }
        
        UnlockPage();
    }

    private static void PlayCompleteSound()
    {
        Task.Run(() =>
        {
            SoundManager.PlaySound("Assets/Sounds/ping.mp3");
        });
    }

    private void DisplayRenderStats(DateTime timeStarted, DateTime timeEnded)
    {
        var renderComplete = new RenderComplete();
        renderComplete.SetValues(
            timeStarted.ToString(),
            timeEnded.ToString(),
            TimerLabel.Content.ToString(),
            RenderItems.CompletedQueue.Count,
            0
        );
        renderComplete.Show();
    }

    private void LockPage()
    {
        Settings.IsEnabled = false;
        ViewSelectOptions.IsEnabled = false;
        ViewSortOptions.IsEnabled = false;
        ViewStackGrid.IsEnabled = false;
        RenderButton.IsEnabled = false;
    }

    private void UnlockPage()
    {
        Settings.IsEnabled = true;
        ViewSelectOptions.IsEnabled = true;
        ViewSortOptions.IsEnabled = true;
        ViewStackGrid.IsEnabled = true;
        RenderButton.IsEnabled = true;
    }
    
    private Task StartTimer(ref bool timerRunning)
    {
        var x = timerRunning;
        return Task.Run(async () =>
        {
            var stopwatch = Stopwatch.StartNew();
            // ReSharper disable once LoopVariableIsNeverChangedInsideLoop
            while (x)
            {
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    TimerLabel.Content = stopwatch.Elapsed.ToString(@"hh\:mm\:ss");
                });
                await Task.Delay(100);
            }
        });
    }

    private async Task Render(RenderQueueItem renderItem, string prefix, string outputDir, float distance, int width, int height, int scale)
    {
        renderItem.SetStatus(RenderStatus.InProgress);

        var blenderPath = DataManager.BlenderPath;
        var modelPath = DataManager.ModelPath;
        var scriptPath = FileManager.GetAbsolutePath("Scripts/render.py");

        var position = RenderManager.GetPosition(renderItem.Key, distance);

        string blenderArguments;
        if (modelPath.EndsWith(".blend"))
        {
            blenderArguments = $"-b \"{modelPath}\" -P \"{scriptPath}\" -- ";
        }

        else
        {
            blenderArguments = $"-b -P \"{scriptPath}\" -- ";
        }

        var arguments = blenderArguments +
                        $"--model \"{modelPath}\" " +
                        $"--name {prefix} " +
                        $"--output_path \"{outputDir}\" " +
                        $"--resolution {width} {height} " +
                        $"--scale {scale} " +
                        $"--distance {distance} " +
                        $"--x {position.X} " +
                        $"--y {position.Y} " +
                        $"--z {position.Z} " +
                        $"--rx {position.Rx} " +
                        $"--ry {position.Ry} " +
                        $"--rz {position.Rz}";

        bool success = await Task.Run(() =>
        {
            return ProcessManager.RunProcessCheck(blenderPath, arguments);
        });

        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            if (success)
            {
                renderItem.SetStatus(RenderStatus.Completed);
            }
            else
            {
                renderItem.SetStatus(RenderStatus.Failed);
            }
            RenderItems.RemoveFromDisplay(renderItem);
            RenderItems.AddToDisplay(renderItem);
        });

        if (success)
        {
            RenderItems.EnqueueCompleted(renderItem);
        }
        else
        {
            RenderItems.EnqueueFailed(renderItem);
        }
    }
}
