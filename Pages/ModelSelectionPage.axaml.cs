using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using LibreHardwareMonitor.Hardware;
using Orthographic.Renderer.Controls;
using Orthographic.Renderer.Entities;
using Hardware = Orthographic.Renderer.Entities.Hardware;

namespace Orthographic.Renderer.Pages;

public class UpdateVisitor : IVisitor
{
    public void VisitComputer(IComputer computer)
    {
        computer.Traverse(this);
    }
    public void VisitHardware(IHardware hardware)
    {
        hardware.Update();
        foreach (IHardware subHardware in hardware.SubHardware) subHardware.Accept(this);
    }
    public void VisitSensor(ISensor sensor) { }
    public void VisitParameter(IParameter parameter) { }
}

public partial class ModelSelectionPage : UserControl
{
    private Computer Computer { get; set; }
    private List<Hardware> CpuStatus { get; set; }
    private List<Hardware> GpuStatus { get; set; }
    Hardware MemoryStatus { get; set; }

    public ModelSelectionPage()
    {
        InitializeComponent();
        
        SetupComputer();
        
        var hardware = CollectRenderHardware();

        for (int i = 0; i < hardware.Count; i++)
        {
            HardwareStatusGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            var hardwareStatusControl = CreateHardwareStatusControl(hardware[i]);
            Grid.SetColumn(hardwareStatusControl, i);
            HardwareStatusGrid.Children.Add(hardwareStatusControl);
        }
        
        // run the in ui thread every 2 seconds
        Task.Run(async () =>
        {
            while (true)
            {
                await Task.Delay(500);
                Dispatcher.UIThread.Post(() =>
                {
                    for (int i = 0; i < hardware.Count; i++)
                    {
                        RefreshHardware(Computer, hardware[i]);

                        var formattedValue = $"{hardware[i].Value:0.00}";
                        ((HardwareStatusControl)HardwareStatusGrid.Children[i]).ValueLabel.Content = formattedValue;
                    }
                });
            }
        });
    }

    private List<Hardware> CollectRenderHardware()
    {
        CpuStatus = MapHardware(Computer).FindAll(hardware => hardware.Type == HardwareType.Cpu && 
                                                              (hardware.SensorType == SensorType.Load) && 
                                                              hardware.SensorName == "CPU Total");
        
        var gpuTypes = new List<HardwareType> { HardwareType.GpuNvidia, HardwareType.GpuAmd, HardwareType.GpuIntel };
        GpuStatus = MapHardware(Computer).FindAll(hardware => gpuTypes.Contains(hardware.Type) &&
                                                              hardware.SensorType == SensorType.Load &&
                                                              hardware.SensorName == "GPU Core");
        
        var gpuTemps = MapHardware(Computer).FindAll(hardware => gpuTypes.Contains(hardware.Type) &&
                                                                 hardware.SensorType == SensorType.Temperature &&
                                                                 hardware.SensorName == "GPU Core");
        GpuStatus.AddRange(gpuTemps);
        
        // Add vram used to gpu status
        var vramUsed = MapHardware(Computer).FindAll(hardware => gpuTypes.Contains(hardware.Type) &&
                                                                 hardware.SensorType == SensorType.SmallData &&
                                                                 hardware.SensorName == "GPU Memory Used");
        GpuStatus.AddRange(vramUsed);
        
        
        
        MemoryStatus = MapHardware(Computer).Find(hardware => hardware.Type == HardwareType.Memory && 
                                                              hardware.SensorType == SensorType.Load && 
                                                              hardware.SensorName == "Memory");
        
        var allHardware = new List<Hardware>();
        allHardware.AddRange(CpuStatus);
        allHardware.AddRange(GpuStatus);
        allHardware.Add(MemoryStatus);
        
        return allHardware;
    }

    private static Control CreateHardwareStatusControl(Hardware hardware)
    {
        var hardwareStatusControl = new HardwareStatusControl();
        hardwareStatusControl.TypeLabel.Content = FormatName(hardware);
        hardwareStatusControl.ValueLabel.Content = 0.00;
        hardwareStatusControl.UnitLabel.Content = hardware.Unit;
        return hardwareStatusControl;
    }

    private static string FormatName(Hardware hardware)
    {
        var newName = hardware.Name;
        
        var redundantStrings = new List<string> { "NVIDIA", "GeForce", "AMD", "Intel", "Generic" };
        foreach (var redundantString in redundantStrings)
        {
            newName = newName.Replace(redundantString, "");
        }
        
        if (hardware.Type == HardwareType.Cpu)
        {
            // If single number at end of string, remove it
            if (char.IsDigit(newName[^1]))
            {
                newName = newName.Substring(0, newName.Length - 1);
            }
        }
        
        // remove whitespace at start and end
        newName = newName.Trim();
        
        var gpuTypes = new List<HardwareType> { HardwareType.GpuNvidia, HardwareType.GpuAmd, HardwareType.GpuIntel };
        if (gpuTypes.Contains(hardware.Type) && hardware.SensorName.Contains("Memory"))
        {
            newName = newName + " VRAM";
        }

        return newName;
    }

    private void SetupComputer()
    {
        Computer = new Computer
        {
            IsCpuEnabled = true,
            IsGpuEnabled = true,
            IsMemoryEnabled = true,
            IsMotherboardEnabled = true,
            IsControllerEnabled = true,
            IsNetworkEnabled = true,
            IsStorageEnabled = true
        };

        Computer.Open();
        Computer.Accept(new UpdateVisitor());
    }
    
    private string? FindUnit(SensorType sensorType)
    {
        switch (sensorType)
        {
            case SensorType.Load:
                return "%";
            case SensorType.Power:
                return "W";
            case SensorType.Clock:
                return "MHz";
            case SensorType.Factor:
                return "?";
            case SensorType.Voltage:
                return "V";
            case SensorType.Temperature:
                return "°C";
            case SensorType.Data:
                return "GB";
            case SensorType.Fan:
                return "RPM";
            case SensorType.Control:
                return "?";
            case SensorType.SmallData:
                return "MB";
            case SensorType.Throughput:
                return "KB/s";
            default:
                return null;
        }
    }
    
    private List<Hardware> MapHardware(Computer computer)
    {
        List<Hardware> map = new List<Hardware>();
        
        for (var i = 0; i < computer.Hardware.Count; i++)
        {
            for (var j = 0; j < computer.Hardware[i].SubHardware.Length; j++)
            {
                for (var k = 0; k < computer.Hardware[i].SubHardware[j].Sensors.Length; k++)
                {
                    var hardware = new Hardware(
                        computer.Hardware[i].SubHardware[j].Name,
                        computer.Hardware[i].SubHardware[j].HardwareType,
                        computer.Hardware[i].SubHardware[j].Sensors[k].Name,
                        computer.Hardware[i].SubHardware[j].Sensors[k].SensorType, 
                        computer.Hardware[i].SubHardware[j].Sensors[k].Value, 
                        FindUnit(computer.Hardware[i].SubHardware[j].Sensors[k].SensorType), 
                        new List<int> { i, j, k });
                    
                    map.Add(hardware);
                }
            }

            for (int j = 0; j < computer.Hardware[i].Sensors.Length; j++)
            {
                var hardware = new Hardware(
                    computer.Hardware[i].Name,
                    computer.Hardware[i].HardwareType,
                    computer.Hardware[i].Sensors[j].Name,
                    computer.Hardware[i].Sensors[j].SensorType, 
                    computer.Hardware[i].Sensors[j].Value, 
                    FindUnit(computer.Hardware[i].Sensors[j].SensorType), 
                    new List<int> { i, j });
                
                map.Add(hardware);
            }
        }

        return map;
    }
    
    private void RefreshHardware(Computer computer, Hardware hardware)
    {
        var path = hardware.Path;
        computer.Hardware[path[0]].Update();
        
        switch (path.Count)
        {
            case 2:
            {
                hardware.UpdateValue(computer.Hardware[path[0]].Sensors[path[1]].Value);
                break;
            }
            case 3:
            {
                hardware.UpdateValue(computer.Hardware[path[0]].SubHardware[path[1]].Sensors[path[2]].Value);
                break;
            }
        }
    }
}