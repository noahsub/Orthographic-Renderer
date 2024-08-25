////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// HardwareMonitorBar.axaml.cs
// This file contains the logic for the HardwareMonitorBar control.
//
// Author(s): https://github.com/noahsub
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// IMPORTS
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Threading;
using Orthographic.Renderer.Managers;
using Hardware = Orthographic.Renderer.Entities.Hardware;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// NAMESPACE
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Orthographic.Renderer.Controls;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// HARDWARE MONITOR BAR CLASS
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
public partial class HardwareMonitorBar : UserControl
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // GLOBALS
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Interval for polling updates in milliseconds.
    /// </summary>
    private const int PollingInterval = 500;

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // INITIALIZATION
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public HardwareMonitorBar()
    {
        InitializeComponent();

        // Wait for RenderHardwareCollected to be true asynchronously
        Task.Run(SetupMonitor);

        // Update hardware status asynchronously
        Task.Run(UpdateMonitor);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // SETUP
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Sets up the hardware monitor by waiting for hardware data to be collected and then initializing the UI.
    /// </summary>
    private async Task SetupMonitor()
    {
        // Wait for hardware data to be collected
        while (!HardwareManager.RenderHardwareCollected)
        {
            await Task.Delay(500);
        }

        // Initialize the UI
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
        // Add a column to the grid for each hardware item.
        for (var i = 0; i < HardwareManager.HardwareToMonitor.Count; i++)
        {
            HardwareGrid.ColumnDefinitions.Add(
                new ColumnDefinition
                {
                    // Set the width of the column to occupy the available space.
                    Width = new GridLength(1, GridUnitType.Star),
                }
            );
        }
    }

    /// <summary>
    /// Populates the hardware status grid with controls for each hardware item.
    /// </summary>
    private void PopulateMonitorGrid()
    {
        // Add a hardware monitor control for each hardware item.
        for (var i = 0; i < HardwareManager.HardwareToMonitor.Count; i++)
        {
            var hardwareStatusControl = CreateHardwareMonitorControl(
                HardwareManager.HardwareToMonitor[i]
            );
            Grid.SetColumn(hardwareStatusControl, i);
            HardwareGrid.Children.Add(hardwareStatusControl);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // MONITORING
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Continuously updates the hardware monitor and check for changes in window size, position, and scroll to ensure
    /// that such operations are not affected by the update of the hardware monitor.
    /// </summary>
    private async Task UpdateMonitor()
    {
        // Loop indefinitely
        while (true)
        {
            // Wait x=polling interval milliseconds
            await Task.Delay(PollingInterval);

            // Perform the update on a background thread
            var updatedValues = await Task.Run(() =>
            {
                return UpdateHardwareMonitorControl();
            });

            // Update the UI on the main thread
            Dispatcher.UIThread.Post(() =>
            {
                for (var i = 0; i < HardwareGrid.Children.Count; i++)
                {
                    ((HardwareStatus)HardwareGrid.Children[i]).ValueLabel.Content = updatedValues?[
                        i
                    ];
                }
            });
        }
        // ReSharper disable once FunctionNeverReturns
    }

    /// <summary>
    /// Updates the hardware monitor controls with the latest hardware status values.
    /// </summary>
    private List<string>? UpdateHardwareMonitorControl()
    {
        // Create a list to store the new values
        var newValues = new List<string>();

        // If the number of hardware items to monitor does not match the number of controls, return null
        if (HardwareGrid.Children.Count != HardwareManager.HardwareToMonitor.Count)
        {
            return null;
        }

        // Update the hardware status for each hardware item
        foreach (var hardware in HardwareManager.HardwareToMonitor)
        {
            // Refresh the hardware status
            HardwareManager.RefreshHardware(HardwareManager.Computer, hardware);
            // Format the value to two decimal places
            var formattedValue = $"{hardware.Value:0.00}";
            // Add the formatted value to the list
            newValues.Add(formattedValue);
        }

        // Return the new values.
        return newValues;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // HELPER FUNCTIONS
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Creates a hardware monitor control for a given hardware item.
    /// </summary>
    /// <param name="hardware">The hardware item to create a control for.</param>
    /// <returns>A control displaying the hardware status.</returns>
    private static HardwareStatus CreateHardwareMonitorControl(Hardware hardware)
    {
        // Create a new hardware monitor control
        var hardwareMonitorControl = new HardwareStatus();
        // Set the hardware monitor control properties based on the hardware item
        hardwareMonitorControl.TypeLabel.Content = HardwareManager.FormatName(hardware);
        hardwareMonitorControl.ValueLabel.Content = 0.00;
        hardwareMonitorControl.UnitLabel.Content = hardware.Unit;
        // Return the hardware monitor control
        return hardwareMonitorControl;
    }
}
