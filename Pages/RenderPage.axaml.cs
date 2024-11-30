using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Orthographic.Renderer.Controls;
using Orthographic.Renderer.Managers;

namespace Orthographic.Renderer.Pages;

public partial class RenderPage : UserControl
{
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
        throw new System.NotImplementedException();
    }

    private void CancelButton_OnClick(object? sender, RoutedEventArgs e)
    {
        throw new System.NotImplementedException();
    }

    private void RenderButton_OnClick(object? sender, RoutedEventArgs e)
    {
        throw new System.NotImplementedException();
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