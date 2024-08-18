using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using LibreHardwareMonitor.Hardware;
using Orthographic.Renderer.Controls;
using Orthographic.Renderer.Managers;
using Hardware = Orthographic.Renderer.Entities.Hardware;

namespace Orthographic.Renderer.Pages;



public partial class ModelSelectionPage : UserControl
{
    public ModelSelectionPage()
    {
        InitializeComponent();
        
        // wait for RenderHardwareCollected to be true asynchronously

        Task.Run(() =>
        {
            while (!HardwareStatusManager.RenderHardwareCollected)
            {
                Thread.Sleep(250);
            }
        })
            .ContinueWith(task =>
            {
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    SetupHardwareGrid();
                    PopulateHardwareGrid();
                });
            });

        Task.Run(async () =>
        {
            var currentSize = MainWindow.GetSize();
            var currentPosition = MainWindow.GetPosition();
            
            while (true)
            {
                await Task.Delay(500);
                
                var newSize = MainWindow.GetSize();
                var newPosition = MainWindow.GetPosition();
                
                // If the window is currently being moved or resized, skip the update
                if (currentSize != newSize || currentPosition != newPosition)
                {
                    currentSize = newSize;
                    currentPosition = newPosition;
                    continue;
                }
                
                Dispatcher.UIThread.Post(() =>
                {
                    UpdateHardwareStatus();
                });
            }
        });
    }

    private void SetupHardwareGrid()
    {
        for (int i = 0; i < HardwareStatusManager.RenderHardware.Count; i++)
        {
            HardwareStatusGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        }
    }

    private void PopulateHardwareGrid()
    {
        for (int i = 0; i < HardwareStatusManager.RenderHardware.Count; i++)
        {
            var hardwareStatusControl = CreateHardwareStatusControl(HardwareStatusManager.RenderHardware[i]);
            Grid.SetColumn(hardwareStatusControl, i);
            HardwareStatusGrid.Children.Add(hardwareStatusControl);
        }
    }

    private void UpdateHardwareStatus()
    {
        if (HardwareStatusGrid.Children.Count != HardwareStatusManager.RenderHardware.Count)
        {
            return;
        }
        
        for (int i = 0; i < HardwareStatusManager.RenderHardware.Count; i++)
        {
            HardwareStatusManager.RefreshHardware(HardwareStatusManager.Computer,
                HardwareStatusManager.RenderHardware[i]);
            var formattedValue = $"{HardwareStatusManager.RenderHardware[i].Value:0.00}";
            ((HardwareStatusControl)HardwareStatusGrid.Children[i]).ValueLabel.Content = formattedValue;
        }
    }
    

    private static Control CreateHardwareStatusControl(Hardware hardware)
    {
        var hardwareStatusControl = new HardwareStatusControl();
        hardwareStatusControl.TypeLabel.Content = HardwareStatusManager.FormatName(hardware);
        hardwareStatusControl.ValueLabel.Content = 0.00;
        hardwareStatusControl.UnitLabel.Content = hardware.Unit;
        return hardwareStatusControl;
    }
}