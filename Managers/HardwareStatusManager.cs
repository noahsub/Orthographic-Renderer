﻿using System.Collections.Generic;
using LibreHardwareMonitor.Hardware;
using Hardware = Orthographic.Renderer.Entities.Hardware;

namespace Orthographic.Renderer.Managers;

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

public class HardwareStatusManager
{
    public static Computer Computer { get; set; }
    public static bool HardwareMonitoringReady { get; set; }
    public static List<Hardware> RenderHardware { get; set; }
    public static bool RenderHardwareCollected { get; set; }
    
    public static void SetupComputer()
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
        HardwareMonitoringReady = true;
    }
    
    public static void CollectRenderHardware()
    {
        RenderHardware = new List<Hardware>();
        
        // Get all hardware
        var hardwareMap = MapHardware(Computer);
        
        // Cpu details to monitor
        var requiredCpuSensorsTypes = new List<SensorType> { SensorType.Load };
        var requiredCpuSensorNames = new List<string> { "CPU Total" };
        
        // Gpu details to monitor
        var gpuTypes = new List<HardwareType> { HardwareType.GpuNvidia, HardwareType.GpuAmd, HardwareType.GpuIntel };
        var requiredGpuSensorsTypes = new List<SensorType> { SensorType.Load, SensorType.Temperature, SensorType.SmallData };
        var requiredGpuSensorNames = new List<string> { "GPU Core", "GPU Memory Used" };
        
        // Memory details to monitor
        var requiredMemorySensorsTypes = new List<SensorType> { SensorType.Load };
        var requiredMemorySensorNames = new List<string> { "Memory" };
        
        foreach (var hardware in hardwareMap)
        {
            // Cpu filter 
            if (hardware.Type == HardwareType.Cpu && requiredCpuSensorsTypes.Contains(hardware.SensorType) && requiredCpuSensorNames.Contains(hardware.SensorName))
            {
                RenderHardware.Add(hardware);
            }
            
            // Gpu filter
            else if (gpuTypes.Contains(hardware.Type) && requiredGpuSensorsTypes.Contains(hardware.SensorType) && requiredGpuSensorNames.Contains(hardware.SensorName))
            {
                RenderHardware.Add(hardware);
            }
            
            // Memory filter
            else if (hardware.Type == HardwareType.Memory && requiredMemorySensorsTypes.Contains(hardware.SensorType) && requiredMemorySensorNames.Contains(hardware.SensorName))
            {
                RenderHardware.Add(hardware);
            }
        }
        
        RenderHardwareCollected = true;
    }
    
    private static string FormatName(Hardware hardware)
    {
        // Create a new name to display in the UI
        var newName = hardware.Name;
        
        // Remove certain words from the name to reduce length in the UI
        var redundantWords = new List<string> { "Generic ", "NVIDIA", "Intel", "AMD" };
        foreach (var word in redundantWords)
        {
            newName = newName.Replace(word, "");
        }
        
        // If name contains a single alphanumeric character (e.g. "A", "1") at the end, remove it
        if (char.IsLetterOrDigit(newName[^1]))
        {
            newName = newName.Substring(0, newName.Length - 1);
        }
        
        // Remove any leading or trailing whitespace
        newName = newName.Trim();
        
        // Add "VRAM" to the end of GPU memory sensor names
        var gpuTypes = new List<HardwareType> { HardwareType.GpuNvidia, HardwareType.GpuAmd, HardwareType.GpuIntel };
        if (gpuTypes.Contains(hardware.Type) && hardware.SensorType == SensorType.SmallData)
        {
            newName = $"{newName} VRAM";
        }

        return newName;
    }
    
    
    public static string? FindUnit(SensorType sensorType)
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
    
    public static List<Hardware> MapHardware(Computer computer)
    {
        var map = new List<Hardware>();
        
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
    
    public static void RefreshHardware(Computer computer, Hardware hardware)
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