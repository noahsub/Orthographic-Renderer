////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// RenderQueue.axaml.cs
// This file contains the logic for the RenderQueue control.
//
// Author(s): https://github.com/noahsub
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// IMPORTS
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System.Collections;
using System.Linq;
using Avalonia.Controls;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// NAMESPACE
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Orthographic.Renderer.Controls;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// RENDER QUEUE CLASS
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

/// <summary>
/// A queue of render items that displays the status of each render.
/// </summary>
public partial class RenderQueue : UserControl
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // GLOBALS
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// The queue of pending items.
    /// </summary>
    public Queue PendingQueue { get; } = new();

    /// <summary>
    /// The queue of completed items.
    /// </summary>
    public Queue CompletedQueue { get; } = new();

    /// <summary>
    /// The queue of failed items.
    /// </summary>
    public Queue FailedQueue { get; } = new();

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // INITIALIZATION
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public RenderQueue()
    {
        InitializeComponent();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // QUEUE OPERATIONS
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Enqueues an item into the pending queue.
    /// </summary>
    /// <param name="item">The item to enqueue.</param>
    public void EnqueuePending(RenderQueueItem item)
    {
        PendingQueue.Enqueue(item);
    }

    /// <summary>
    /// Dequeues an item from the pending queue.
    /// </summary>
    /// <returns>The dequeued item.</returns>
    public RenderQueueItem DequeuePending()
    {
        return (RenderQueueItem)PendingQueue.Dequeue()!;
    }

    /// <summary>
    /// Enqueues an item into the completed queue.
    /// </summary>
    /// <param name="item">The item to enqueue.</param>
    public void EnqueueCompleted(RenderQueueItem item)
    {
        CompletedQueue.Enqueue(item);
    }

    /// <summary>
    /// Dequeues an item from the completed queue.
    /// </summary>
    /// <returns>The dequeued item.</returns>
    public RenderQueueItem DequeueCompleted()
    {
        return (RenderQueueItem)CompletedQueue.Dequeue()!;
    }

    /// <summary>
    /// Enqueues an item into the failed queue.
    /// </summary>
    /// <param name="item">The item to enqueue.</param>
    public void EnqueueFailed(RenderQueueItem item)
    {
        FailedQueue.Enqueue(item);
    }

    /// <summary>
    /// Dequeues an item from the failed queue.
    /// </summary>
    /// <returns>The dequeued item.</returns>
    public RenderQueueItem DequeueFailed()
    {
        return (RenderQueueItem)FailedQueue.Dequeue()!;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // ARRAY OPERATIONS
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Get all items in the pending queue.
    /// </summary>
    /// <returns>An enumerable of items in the pending queue.</returns>
    public IEnumerable GetItemsPending()
    {
        return PendingQueue.ToArray();
    }

    /// <summary>
    /// Get all items in the completed queue as an array.
    /// </summary>
    /// <returns>An enumerable of items in the completed queue.</returns>
    public IEnumerable GetItemsCompleted()
    {
        return CompletedQueue.ToArray();
    }

    /// <summary>
    /// Get all items in the failed queue.
    /// </summary>
    /// <returns>An enumerable of items in the failed queue.</returns>
    public IEnumerable GetItemsFailed()
    {
        return FailedQueue.ToArray();
    }

    /// <summary>
    /// Get all items in the queues combined in the order pending, completed, failed.
    /// </summary>
    /// <returns>An enumerable of all items in the queues.</returns>
    public IEnumerable GetItems()
    {
        // Combine the three queues into one
        return PendingQueue
            .ToArray()
            .Concat(CompletedQueue.ToArray())
            .Concat(FailedQueue.ToArray());
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // DISPLAY
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Adds an item to the display.
    /// </summary>
    /// <param name="item">The item to add to the display.</param>
    public void AddToDisplay(RenderQueueItem item)
    {
        // If there are no items in the display, hide the empty state image
        if (Items.Children.Count == 0)
        {
            EmptyImage.IsVisible = false;
        }

        // Add the item to the Items stack panel
        Items.Children.Add(item);
    }

    /// <summary>
    /// Removes an item from the display.
    /// </summary>
    /// <param name="item">The item to remove from the display.</param>
    public void RemoveFromDisplay(RenderQueueItem item)
    {
        // If there are no items in the display, show the empty state image
        if (Items.Children.Count == 0)
        {
            EmptyImage.IsVisible = true;
        }

        // Remove the item from the Items stack panel
        Items.Children.Remove(item);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // CLEANUP
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Clears all items from the queues and the display.
    /// </summary>
    public void ClearItems()
    {
        // Clear the queues
        PendingQueue.Clear();
        CompletedQueue.Clear();
        FailedQueue.Clear();
        // Clear the display
        Items.Children.Clear();
        // Show the empty state image
        EmptyImage.IsVisible = true;
    }
}
