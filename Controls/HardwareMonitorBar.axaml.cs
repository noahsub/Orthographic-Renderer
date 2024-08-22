using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Threading;
using LibreHardwareMonitor.Hardware;
using Orthographic.Renderer.Managers;
using Hardware = Orthographic.Renderer.Entities.Hardware;

namespace Orthographic.Renderer.Controls;

public partial class HardwareMonitorBar : UserControl
{
    /// <summary>
    /// Interval for polling updates in milliseconds.
    /// </summary>
    private const int PollingInterval = 500;
    
    public HardwareMonitorBar()
    {
        InitializeComponent();
        
        // Wait for RenderHardwareCollected to be true asynchronously
        Task.Run(SetupMonitor);

        // Update hardware status asynchronously
        Task.Run(UpdateMonitor);
    }
    
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
    /// Continuously updates the hardware monitor and check for changes in window size, position, and scroll to ensure
    /// that such operations are not affected by the update of the hardware monitor.
    /// </summary>
    private async Task UpdateMonitor()
    {
        while (true)
        {
            await Task.Delay(PollingInterval);
            
            // Perform the update on a background thread
            var updatedValues = await Task.Run(() =>
            {
                return UpdateHardwareMonitorControl();
            });

            Dispatcher.UIThread.Post(() =>
            {
                for (int i = 0; i < HardwareGrid.Children.Count; i++)
                {
                    
                    ((HardwareStatus)HardwareGrid.Children[i]).ValueLabel.Content = updatedValues?[i];
                }
            });
        }
        // ReSharper disable once FunctionNeverReturns
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
            HardwareGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
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
            var hardwareStatusControl = CreateHardwareMonitorControl(HardwareManager.HardwareToMonitor[i]);
            Grid.SetColumn(hardwareStatusControl, i);
            HardwareGrid.Children.Add(hardwareStatusControl);
        }
    }
    
    /// <summary>
    /// Updates the hardware monitor controls with the latest hardware status values.
    /// </summary>
    private List<string>? UpdateHardwareMonitorControl()
    {
        List<string> newValues = new List<string>();
        
        if (HardwareManager.HardwareToMonitor == null)
        {
            return null;
        }

        if (HardwareGrid.Children.Count != HardwareManager.HardwareToMonitor.Count)
        {
            return null;
        }

        for (var i = 0; i < HardwareManager.HardwareToMonitor.Count; i++)
        {
            var hardware = HardwareManager.HardwareToMonitor[i];
            
            if (hardware.Path == null && hardware.Type == HardwareType.GpuNvidia)
            {
                continue;
            }

            HardwareManager.RefreshHardware(HardwareManager.Computer, hardware);

            var formattedValue = $"{hardware.Value:0.00}";
            newValues.Add(formattedValue);
            // ((HardwareControl)HardwareGrid.Children[i]).ValueLabel.Content = formattedValue;
        }

        return newValues;
    }
    
    /// <summary>
    /// Creates a hardware monitor control for a given hardware item.
    /// </summary>
    /// <param name="hardware">The hardware item to create a control for.</param>
    /// <returns>A control displaying the hardware status.</returns>
    private static Control CreateHardwareMonitorControl(Hardware hardware)
    {
        var hardwareMonitorControl = new HardwareStatus();
        hardwareMonitorControl.TypeLabel.Content = HardwareManager.FormatName(hardware);
        hardwareMonitorControl.ValueLabel.Content = 0.00;
        hardwareMonitorControl.UnitLabel.Content = hardware.Unit;
        return hardwareMonitorControl;
    }
}