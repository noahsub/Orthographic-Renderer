using System.Collections;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Orthographic.Renderer.Controls;

public partial class RenderQueue : UserControl
{
    public Queue ProgressQueue { get; } = new();
    public Queue CompletedQueue { get; } = new();

    public RenderQueue()
    {
        InitializeComponent();
    }

    public void EnqueueProgress(RenderQueueItem item)
    {
        ProgressQueue.Enqueue(item);
    }

    public RenderQueueItem DequeueProgress()
    {
        return (RenderQueueItem)ProgressQueue.Dequeue();
    }

    public void EnqueueCompleted(RenderQueueItem item)
    {
        CompletedQueue.Enqueue(item);
    }

    public RenderQueueItem DequeueCompleted()
    {
        return (RenderQueueItem)CompletedQueue.Dequeue();
    }

    public void AddToDisplay(RenderQueueItem item)
    {
        if (Items.Children.Count == 0)
        {
            EmptyImage.IsVisible = false;
        }

        Items.Children.Add(item);
    }

    public void RemoveFromDisplay(RenderQueueItem item)
    {
        if (Items.Children.Count == 0)
        {
            EmptyImage.IsVisible = true;
        }

        Items.Children.Remove(item);
    }

    public IEnumerable GetItemsInProgress()
    {
        return ProgressQueue.ToArray();
    }

    public IEnumerable GetItemsCompleted()
    {
        return CompletedQueue.ToArray();
    }

    public IEnumerable GetItems()
    {
        // Combine the two queues into a single enumerable
        return ProgressQueue.ToArray().Concat(CompletedQueue.ToArray());
    }

    public void ClearItems()
    {
        ProgressQueue.Clear();
        CompletedQueue.Clear();
        Items.Children.Clear();
        EmptyImage.IsVisible = true;
    }
}
