using Avalonia.Controls;
using Avalonia.Media;
using Orthographic.Renderer.Entities;

namespace Orthographic.Renderer.Controls;

public partial class RenderQueueItem : UserControl
{
    public string Key { get; set; }
    public RenderStatus Status { get; set; }

    public RenderQueueItem()
    {
        InitializeComponent();
        SetStatus(RenderStatus.Pending);
    }

    public void SetStatus(RenderStatus status)
    {
        switch (status)
        {
            case RenderStatus.Pending:
                StatusLabel.Content = "PENDING";
                ContentGrid.Background = Brushes.Goldenrod;
                break;
            case RenderStatus.InProgress:
                StatusLabel.Content = "IN PROGRESS";
                ContentGrid.Background = Brushes.MediumPurple;
                break;
            case RenderStatus.Completed:
                StatusLabel.Content = "COMPLETED";
                ContentGrid.Background = Brushes.MediumSeaGreen;
                break;
            case RenderStatus.Failed:
                StatusLabel.Content = "FAILED";
                ContentGrid.Background = Brushes.IndianRed;
                break;
        }
    }
}
