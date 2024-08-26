﻿////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// RenderQueueItem.axaml.cs
// This file contains the logic for the RenderQueueItem control.
//
// Author(s): https://github.com/noahsub
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// IMPORTS
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Media;
using Orthographic.Renderer.Constants;
using Orthographic.Renderer.Entities;
using Orthographic.Renderer.Managers;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// NAMESPACE
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Orthographic.Renderer.Controls;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// RENDER QUEUE ITEM CLASS
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

/// <summary>
/// An item in the render queue that displays the status of a render.
/// </summary>
public partial class RenderQueueItem : UserControl
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // GLOBALS
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// The key for the associated view, see <see cref="RenderManager.RenderViews"/> for assignable values.
    /// </summary>
    public string Key { get; set; }

    /// <summary>
    /// The status of the render.
    /// </summary>
    public RenderStatus Status { get; set; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // INITIALIZATION
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public RenderQueueItem()
    {
        InitializeComponent();
        // Set the status to pending by default
        SetStatus(RenderStatus.Pending);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // STATUS
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Set the status of the render.
    /// </summary>
    /// <param name="status"></param>
    public void SetStatus(RenderStatus status)
    {
        // Set the status label and background color based on the status
        switch (status)
        {
            case RenderStatus.Pending:
                StatusLabel.Content = "PENDING";
                ContentGrid.Background = Brushes.Goldenrod;
                Status = RenderStatus.Pending;
                break;
            case RenderStatus.InProgress:
                StatusLabel.Content = "IN PROGRESS";
                ContentGrid.Background = Brushes.MediumPurple;
                Status = RenderStatus.InProgress;
                break;
            case RenderStatus.Completed:
                StatusLabel.Content = "COMPLETED";
                ContentGrid.Background = Brushes.MediumSeaGreen;
                Status = RenderStatus.Completed;
                break;
            case RenderStatus.Failed:
                StatusLabel.Content = "FAILED";
                ContentGrid.Background = Brushes.IndianRed;
                Status = RenderStatus.Failed;
                break;
            default:
                // Throw an exception if the status is invalid
                throw new InvalidEnumArgumentException(
                    nameof(status),
                    (int)status,
                    typeof(RenderStatus)
                );
        }
    }
}
