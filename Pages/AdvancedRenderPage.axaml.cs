using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace Orthographic.Renderer.Pages;

public partial class AdvancedRenderPage : UserControl
{
    public AdvancedRenderPage()
    {
        InitializeComponent();
        ViewStackGrid.SetColumns(5);
        PopulateViews(RenderManager.RenderViews);
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
        RenderItems.ClearItems();

        var prefix = Settings.PrefixTextBox.Text;
        var outputDir = Settings.OutputDirTextBox.PathTextBox.Text;
        var distance = (float)Settings.DistanceNumeric.Value;
        var width = Settings.ResolutionWidthNumeric.Value;
        var height = Settings.ResolutionHeightNumeric.Value;
        var scale = Settings.ScaleNumeric.Value;
        var sound = Settings.PlaySoundCheckBox.IsChecked;
        var mode = Settings.RenderModeComboBox.SelectionBoxItem.ToString().ToLower();
        var threads = (int)Settings.ThreadsNumeric.Value;

        var views = new List<Tuple<string, RenderQueueItem>>();
        foreach (var view in GetSelectedViews())
        {
            var viewKey = view.ToLower().Replace(" ", "-");
            var renderItem = new RenderQueueItem();
            renderItem.Name.Content = view;
            RenderItems.EnqueueProgress(renderItem);
            RenderItems.AddToDisplay(renderItem);
            views.Add(new Tuple<string, RenderQueueItem>(viewKey, renderItem));
        }

        var blenderPath = DataManager.BlenderPath;
        var pythonPath = DataManager.PythonPath;
        var modelPath = DataManager.ModelPath;
        var scriptPath = FileManager.GetAbsolutePath("Scripts/render.py");

        switch (mode)
        {
            case "sequential":
                foreach (var view in views)
                {
                    var position = RenderManager.GetPosition(view.Item1, distance);
                    view.Item2.SetStatus(RenderStatus.InProgress);

                    var arguments =
                        $"-b \"{modelPath}\" -P \"{scriptPath}\" -- --name {prefix} --output_path \"{outputDir}\" --resolution {width} {height} --scale {scale} --distance {distance} --x {position.X} --y {position.Y} --z {position.Z} --rx {position.Rx} --ry {position.Ry} --rz {position.Rz}";

                    await Task.Run(() => { ProcessManager.RunProcess(blenderPath, arguments); });

                    await Dispatcher.UIThread.InvokeAsync(() => { view.Item2.SetStatus(RenderStatus.Completed); });

                    RenderItems.DequeueProgress();
                }

                break;
            case "parallel":
                var semaphore = new SemaphoreSlim(threads);

                var tasks = views.Select(async view =>
                {
                    await semaphore.WaitAsync();
                    try
                    {
                        var position = RenderManager.GetPosition(view.Item1, distance);
                        view.Item2.SetStatus(RenderStatus.InProgress);

                        var arguments =
                            $"-b \"{modelPath}\" -P \"{scriptPath}\" -- --name {prefix} --output_path \"{outputDir}\" --resolution {width} {height} --scale {scale} --distance {distance} --x {position.X} --y {position.Y} --z {position.Z} --rx {position.Rx} --ry {position.Ry} --rz {position.Rz}";

                        await Task.Run(() => { ProcessManager.RunProcess(blenderPath, arguments); });

                        await Dispatcher.UIThread.InvokeAsync(() => { view.Item2.SetStatus(RenderStatus.Completed); });

                        RenderItems.DequeueProgress();
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                });

                await Task.WhenAll(tasks);
                break;
        }
    }
}