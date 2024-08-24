using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
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
    
    /// <summary>
    /// Populates the view selection stack panels with render view controls for different angles from left to right, top
    /// to bottom.
    /// </summary>
    /*
    ┌───┐ ┌───┐ ┌───┐ ┌───┐
    │ A │ │ B │ │ C │ │ D │
    │ E │ │ F │ │ G │ │ H │
    │ I │ │ J │ │ K │ │ . │
    └───┘ └───┘ └───┘ └───┘
     */
    private void PopulateViews(List<string> views)
    {
        foreach (var view in views)
        {
            // Create a new RenderViewControl
            var viewSelection = new ViewSelection();

            // Set the image source
            var image = new Bitmap($"Assets/Images/RenderAngles/{view}.png");
            viewSelection.Image.Source = image;

            var formattedName =
                // remove hyphens from angle name
                string.Join(
                    " ",
                    view.Replace("-", " ")
                        // capitalize the first letter of each word
                        .Split(' ')
                        .Select(word => char.ToUpper(word[0]) + word[1..].ToLower())
                );
            // set the name label
            viewSelection.Name.Content = formattedName;

            // add the control to the stack panel in the correct column
            ViewStackGrid.AddItem(viewSelection);
        }
    }

    private void ViewSelect_OnTapped(object? sender, TappedEventArgs e)
    {
        if (ViewSelect.AllButton.IsChecked == true)
        {
            foreach (ViewSelection view in ViewStackGrid.GetItems())
            {
                view.CheckBox.IsChecked = true;
            }

            ViewSelect.AllButton.IsChecked = false;
        }
        
        else if (ViewSelect.NoneButton.IsChecked == true)
        {
            foreach (ViewSelection view in ViewStackGrid.GetItems())
            {
                view.CheckBox.IsChecked = false;
            }
            
            ViewSelect.NoneButton.IsChecked = false;
        }
        
        else if (ViewSelect.InvertButton.IsChecked == true)
        {
            foreach (ViewSelection view in ViewStackGrid.GetItems())
            {
                view.CheckBox.IsChecked = !view.CheckBox.IsChecked;
            }
            
            ViewSelect.InvertButton.IsChecked = false;
        }
    }

    private void ViewSort_OnTapped(object? sender, TappedEventArgs e)
    {
        var selectedViews = GetSelectedViews();

        // Clear all views from the stack grid
        ViewStackGrid.ClearItems();

        var facesToSortBy = new List<string>();
        foreach (ToggleButton face in ViewSort.Faces.Children)
        {
            if (face.IsChecked == true)
            {
                facesToSortBy.Add(face.Content.ToString().ToLower());
            }
        }

        var matchingViews = new List<string>();
        var nonMatchingViews = new List<string>();
        foreach (var view in RenderManager.RenderViews)
        {
            if (facesToSortBy.All(view.Contains))
            {
                matchingViews.Add(view);
            }

            else
            {
                nonMatchingViews.Add(view);
            }
        }

        var sortedViews = new List<string>();
        sortedViews.AddRange(matchingViews);
        sortedViews.AddRange(nonMatchingViews);
        
        PopulateViews(sortedViews);

        foreach (ViewSelection view in ViewStackGrid.GetItems())
        {
            if (selectedViews.Contains(view.Name.Content.ToString()))
            {
                view.CheckBox.IsChecked = true;
            }
        }
    }

    private List<string> GetSelectedViews()
    {
        // Get the selected views
        var selectedViews = new List<string>();
        foreach (ViewSelection view in ViewStackGrid.GetItems())
        {
            if (view.CheckBox.IsChecked == true)
            {
                selectedViews.Add(view.Name.Content.ToString());
            }
        }

        return selectedViews;
    }

    private async void RenderButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var timeStarted = DateTime.Now;
        
        LockPage();
        
        // Start a timer thread to update the timer label every second until this method completes
        var timerRunning = true;
        var timerTask = Task.Run(async () =>
        {
            var stopwatch = Stopwatch.StartNew();
            while (timerRunning)
            {
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    TimerLabel.Content = stopwatch.Elapsed.ToString(@"hh\:mm\:ss");
                });
                await Task.Delay(100);
            }
        });

        RenderItems.ClearItems();

        var prefix = Settings.PrefixTextBox.Text;
        var outputDir = Settings.OutputDirTextBox.PathTextBox.Text;
        var distance = (float)Settings.DistanceNumeric.Value;
        var width = (int)Settings.ResolutionWidthNumeric.Value;
        var height = (int)Settings.ResolutionHeightNumeric.Value;
        var scale = Settings.ScaleNumeric.Value;
        var sound = Settings.PlaySoundCheckBox.IsChecked;
        var mode = Settings.RenderModeComboBox.SelectionBoxItem.ToString().ToLower();
        var threads = (int)Settings.ThreadsNumeric.Value;

        foreach (var view in GetSelectedViews())
        {
            var viewKey = view.ToLower().Replace(" ", "-");
            var renderItem = new RenderQueueItem();
            renderItem.Name.Content = view;
            RenderItems.EnqueueProgress(renderItem);
            RenderItems.AddToDisplay(renderItem);
        }

        var blenderPath = DataManager.BlenderPath;
        var pythonPath = DataManager.PythonPath;
        var modelPath = DataManager.ModelPath;
        var scriptPath = FileManager.GetAbsolutePath("Scripts/render.py");

        switch (mode)
        {
            case "sequential":
                while (RenderItems.ProgressQueue.Count > 0)
                {
                    var renderItem = RenderItems.DequeueProgress();
                    await Render(renderItem, distance, modelPath, scriptPath, prefix, outputDir, width, height, scale, blenderPath);
                }
                break;

            case "parallel":
                var semaphore = new SemaphoreSlim(threads);

                var tasks = RenderItems.GetItemsInProgress().Cast<RenderQueueItem>().Select(async renderItem =>
                {
                    await semaphore.WaitAsync();
                    try
                    {
                        await Render(renderItem, distance, modelPath, scriptPath, prefix, outputDir, width, height, scale, blenderPath);
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                });

                await Task.WhenAll(tasks);
                break;
        }

        // Cancel the timer task
        timerRunning = false;
        
        var timeEnded = DateTime.Now;
        
        UnlockPage();
        
        var renderComplete = new RenderComplete();
        renderComplete.SetValues(timeStarted.ToString(), timeEnded.ToString(), TimerLabel.Content.ToString(), RenderItems.CompletedQueue.Count, 0);
        renderComplete.Show();

        if (sound == true)
        {
            Task.Run(() =>
            {
                SoundManager.PlaySound("Assets/Sounds/ping.mp3");
            });
        }
    }

    private async Task Render(RenderQueueItem renderItem, float distance, string modelPath, string scriptPath,
        string? prefix, string? outputDir, decimal? width, decimal? height, decimal? scale, string? blenderPath)
    {
        var position = RenderManager.GetPosition(renderItem.Name.Content.ToString().ToLower().Replace(" ", "-"), distance);
        renderItem.SetStatus(RenderStatus.InProgress);

        var arguments =
            $"-b \"{modelPath}\" -P \"{scriptPath}\" -- --name {prefix} --output_path \"{outputDir}\" --resolution {width} {height} --scale {scale} --distance {distance} --x {position.X} --y {position.Y} --z {position.Z} --rx {position.Rx} --ry {position.Ry} --rz {position.Rz}";

        await Task.Run(() => { ProcessManager.RunProcess(blenderPath, arguments); });

        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            renderItem.SetStatus(RenderStatus.Completed);
            RenderItems.RemoveFromDisplay(renderItem);
            RenderItems.AddToDisplay(renderItem);
        });

        RenderItems.EnqueueCompleted(renderItem);
    }

    private void LockPage()
    {
        Settings.IsEnabled = false;
        ViewSelect.IsEnabled = false;
        ViewSort.IsEnabled = false;
        ViewStackGrid.IsEnabled = false;
        RenderButton.IsEnabled = false;
    }
    
    private void UnlockPage()
    {
        Settings.IsEnabled = true;
        ViewSelect.IsEnabled = true;
        ViewSort.IsEnabled = true;
        ViewStackGrid.IsEnabled = true;
        RenderButton.IsEnabled = true;
    }
}