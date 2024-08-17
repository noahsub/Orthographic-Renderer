using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Threading;
using LibreHardwareMonitor.Hardware;
using Orthographic.Renderer.Managers;

namespace Orthographic.Renderer.Pages;



public partial class ModelSelectionPage : UserControl
{
    public ModelSelectionPage()
    {
        InitializeComponent();
        
        // wait for RenderHardwareCollected to be true asynchronously

        Task.Run(() =>
        {
            int i = 0;
            while (!HardwareStatusManager.RenderHardwareCollected)
            {
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    TestLabel.Content = $"RUNNING {i} TIMES";
                });
                Thread.Sleep(250);
                i++;
            }

            return i;
        })
            .ContinueWith(task =>
            {
                var result = task.Result;
                
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    TestLabel.Content = $"RAN {result} TIMES";
                });
            });
    }
    

    // private static Control CreateHardwareStatusControl(Hardware hardware)
    // {
    //     var hardwareStatusControl = new HardwareStatusControl();
    //     hardwareStatusControl.TypeLabel.Content = FormatName(hardware);
    //     hardwareStatusControl.ValueLabel.Content = 0.00;
    //     hardwareStatusControl.UnitLabel.Content = hardware.Unit;
    //     return hardwareStatusControl;
    // }
}