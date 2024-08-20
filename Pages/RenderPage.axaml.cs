////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// RenderPage.axaml.cs
// This file contains the logic for the RenderPage.axaml file. This file is responsible for displaying hardware
// monitoring, selecting render views, and starting blender render jobs.
//
// Author(s): https://github.com/noahsub
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// IMPORTS
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using Orthographic.Renderer.Controls;
using Orthographic.Renderer.Managers;
using Hardware = Orthographic.Renderer.Entities.Hardware;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// RENDER PAGE CLASS
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Orthographic.Renderer.Pages;

/// <summary>
/// Handles displaying hardware monitoring, selecting render views, and starting blender render jobs.
/// </summary>
public partial class RenderPage : UserControl
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // GLOBAL VARIABLES
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Interval for polling updates in milliseconds.
    /// </summary>
    private const int PollingInterval = 1500;

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // INITIALIZATION
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Initializes the RenderPage.
    /// </summary>
    public RenderPage()
    {
        InitializeComponent();

        // Wait for RenderHardwareCollected to be true asynchronously
        Task.Run(SetupMonitor);

        // Update hardware status asynchronously
        Task.Run(UpdateMonitor);

        // Populate the angles
        PopulateViews();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // MONITOR
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Sets up the hardware monitor by waiting for hardware data to be collected and then initializing the UI.
    /// </summary>
    private async Task SetupMonitor()
    {
        while (!HardwareManager.RenderHardwareCollected)
        {
            await Task.Delay(500);
        }

        Dispatcher.UIThread.InvokeAsync(() =>
        {
            SetupMonitorColumns();
            PopulateMonitorGrid();
        });
    }

    /// <summary>
    /// Sets up the columns in the hardware status grid based on the number of hardware items to monitor.
    /// </summary>
    private void SetupMonitorColumns()
    {
        // Ensure the hardware to monitor has been collected
        if (HardwareManager.HardwareToMonitor == null)
        {
            return;
        }

        for (var i = 0; i < HardwareManager.HardwareToMonitor.Count; i++)
        {
            HardwareStatusGrid.ColumnDefinitions.Add(
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
            );
        }
    }

    /// <summary>
    /// Populates the hardware status grid with controls for each hardware item.
    /// </summary>
    private void PopulateMonitorGrid()
    {
        if (HardwareManager.HardwareToMonitor == null)
        {
            return;
        }

        for (var i = 0; i < HardwareManager.HardwareToMonitor.Count; i++)
        {
            var hardwareStatusControl = CreateHardwareMonitorControl(
                HardwareManager.HardwareToMonitor[i]
            );
            Grid.SetColumn(hardwareStatusControl, i);
            HardwareStatusGrid.Children.Add(hardwareStatusControl);
        }
    }

    /// <summary>
    /// Continuously updates the hardware monitor and check for changes in window size, position, and scroll to ensure
    /// that such operations are not affected by the update of the hardware monitor.
    /// </summary>
    private async Task UpdateMonitor()
    {
        var currentSize = MainWindow.GetSize();
        var currentPosition = MainWindow.GetPosition();
        var currentScrollPosition = 0.0;
        var currentMousePressed = MainWindow.IsPointerPressed;

        while (true)
        {
            await Task.Delay(PollingInterval);

            var newSize = MainWindow.GetSize();
            var newPosition = MainWindow.GetPosition();
            var newMousePressed = MainWindow.IsPointerPressed;

            // If the window is currently being moved or resized, skip the update
            if (
                currentSize != newSize
                || currentPosition != newPosition
                || newMousePressed != currentMousePressed
            )
            {
                currentSize = newSize;
                currentPosition = newPosition;
                continue;
            }

            Dispatcher.UIThread.Post(() =>
            {
                // If the scroll position has changed, skip the update
                // Must be done in the UI thread
                var newScrollPosition = RenderSelectionScrollViewer.Offset.Y;

                if (Math.Abs(currentScrollPosition - newScrollPosition) > 0.5f)
                {
                    currentScrollPosition = newScrollPosition;
                    return;
                }

                UpdateHardwareMonitorControl();
            });
        }
        // ReSharper disable once FunctionNeverReturns
    }

    /// <summary>
    /// Updates the hardware monitor controls with the latest hardware status values.
    /// </summary>
    private void UpdateHardwareMonitorControl()
    {
        if (HardwareManager.HardwareToMonitor == null)
        {
            return;
        }

        if (HardwareStatusGrid.Children.Count != HardwareManager.HardwareToMonitor.Count)
        {
            return;
        }

        for (var i = 0; i < HardwareManager.HardwareToMonitor.Count; i++)
        {
            var hardware = HardwareManager.HardwareToMonitor[i];
            HardwareManager.RefreshHardware(HardwareManager.Computer, hardware);
            var formattedValue = $"{hardware.Value:0.00}";
            ((HardwareMonitorControl)HardwareStatusGrid.Children[i]).ValueLabel.Content =
                formattedValue;
        }
    }

    /// <summary>
    /// Creates a hardware monitor control for a given hardware item.
    /// </summary>
    /// <param name="hardware">The hardware item to create a control for.</param>
    /// <returns>A control displaying the hardware status.</returns>
    private static Control CreateHardwareMonitorControl(Hardware hardware)
    {
        var hardwareMonitorControl = new HardwareMonitorControl();
        hardwareMonitorControl.TypeLabel.Content = HardwareManager.FormatName(hardware);
        hardwareMonitorControl.ValueLabel.Content = 0.00;
        hardwareMonitorControl.UnitLabel.Content = hardware.Unit;
        return hardwareMonitorControl;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // VIEWS
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Determines the appropriate stack panel for a given index.
    /// </summary>
    /// <param name="index">The index to determine the stack panel for.</param>
    /// <returns>The stack panel corresponding to the index.</returns>
    /*
       ┌───┐ ┌───┐ ┌───┐ ┌───┐
       │   │ │   │ │   │ │   │
    ┌──► 0 ┼─► 1 ┼─► 2 ┼─► 3 ┼──┐
    │  │   │ │   │ │   │ │   │  │
    │  └───┘ └───┘ └───┘ └───┘  │
    │                           │
    └───────────────────────────┘
     */
    private StackPanel DetermineViewSelectionStackPanel(int index)
    {
        return index switch
        {
            0 => ViewSelectionStackPanel0,
            1 => ViewSelectionStackPanel1,
            2 => ViewSelectionStackPanel2,
            3 => ViewSelectionStackPanel3,
            _ => ViewSelectionStackPanel0,
        };
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
    private void PopulateViews()
    {
        var views = RenderManager.RenderViews;

        var index = 0;
        foreach (var view in views)
        {
            // Create a new RenderViewControl
            var renderSelectionControl = new RenderViewControl();

            // Set the image source
            var image = new Bitmap($"Assets/Images/RenderAngles/{view}.png");
            renderSelectionControl.Image.Source = image;

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
            renderSelectionControl.Name.Content = formattedName;

            // add the control to the stack panel in the correct column
            var viewSelectionStackPanel = DetermineViewSelectionStackPanel(index % 4);
            viewSelectionStackPanel.Children.Add(renderSelectionControl);
            index++;
        }
    }
}
