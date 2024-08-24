using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Orthographic.Renderer.Windows;

public partial class RenderComplete : Window
{
    public RenderComplete()
    {
        InitializeComponent();
    }

    public void SetValues(
        string startTime,
        string endTime,
        string totalTime,
        int completed,
        int failed
    )
    {
        StartTimeLabel.Content = $"Start Time: {startTime}";
        EndTimeLabel.Content = $"End Time: {endTime}";
        RenderTimeLabel.Content = $"Total Time: {totalTime}";
        RendersCompleted.Content = $"Completed: {completed}";
        RendersFailed.Content = $"Failed: {failed}";
    }

    private void CloseButton_OnClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}
