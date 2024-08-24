using System.Collections;
using System.Linq;
using Avalonia.Controls;

namespace Orthographic.Renderer.Controls;

public partial class RenderQueue : UserControl
{
    public Queue PendingQueue { get; } = new();
    public Queue CompletedQueue { get; } = new();
    public Queue FailedQueue { get; } = new();

    public RenderQueue()
    {
        InitializeComponent();
    }

    public void EnqueuePending(RenderQueueItem item)
    {
        PendingQueue.Enqueue(item);
    }

    public RenderQueueItem DequeuePending()
    {
        return (RenderQueueItem)PendingQueue.Dequeue()!;
    }

    public void EnqueueCompleted(RenderQueueItem item)
    {
        CompletedQueue.Enqueue(item);
    }

    public RenderQueueItem DequeueCompleted()
    {
        return (RenderQueueItem)CompletedQueue.Dequeue()!;
    }
    
    public void EnqueueFailed(RenderQueueItem item)
    {
        FailedQueue.Enqueue(item);
    }
    
    public RenderQueueItem DequeueFailed()
    {
        return (RenderQueueItem)FailedQueue.Dequeue()!;
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

    public IEnumerable GetItemsPending()
    {
        return PendingQueue.ToArray();
    }

    public IEnumerable GetItemsCompleted()
    {
        return CompletedQueue.ToArray();
    }
    
    public IEnumerable GetItemsFailed()
    {
        return FailedQueue.ToArray();
    }

    public IEnumerable GetItems()
    {
        // Combine the three queues into one
        return PendingQueue.ToArray().Concat(CompletedQueue.ToArray()).Concat(FailedQueue.ToArray());
    }

    public void ClearItems()
    {
        PendingQueue.Clear();
        CompletedQueue.Clear();
        FailedQueue.Clear();
        Items.Children.Clear();
        EmptyImage.IsVisible = true;
    }
}
