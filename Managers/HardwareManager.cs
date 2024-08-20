////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// HardwareManager.cs
// This file manages hardware monitoring of rendering related components.
//
// Author(s): https://github.com/noahsub
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// NAMESPACE
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Orthographic.Renderer.Managers;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// IMPORTS
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System.Collections.Generic;
using LibreHardwareMonitor.Hardware;
using Hardware = Entities.Hardware;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// UPDATE VISITOR CLASS
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

/// <summary>
/// Visitor class to update hardware components.
/// </summary>
public class UpdateVisitor : IVisitor
{
    /// <summary>
    /// Traverses the hardware components of this computer.
    /// </summary>
    /// <param name="computer">This computer.</param>
    public void VisitComputer(IComputer computer)
    {
        computer.Traverse(this);
    }

    /// <summary>
    /// Visits the hardware and updates it.
    /// </summary>
    /// <param name="hardware">The hardware to visit.</param>
    public void VisitHardware(IHardware hardware)
    {
        hardware.Update();
        foreach (var subHardware in hardware.SubHardware)
        {
            subHardware.Accept(this);
        }
    }

    /// <summary>
    /// Visits the sensor.
    /// </summary>
    /// <param name="sensor">The sensor to visit.</param>
    public void VisitSensor(ISensor sensor) { }

    /// <summary>
    /// Visits the parameter.
    /// </summary>
    /// <param name="parameter">The parameter to visit.</param>
    public void VisitParameter(IParameter parameter) { }
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// HARDWARE MANAGER CLASS
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

/// <summary>
/// Manages hardware monitoring and data collection.
/// </summary>
public class HardwareManager
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // GLOBAL VARIABLES
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// This computer.
    /// </summary>
    public static Computer? Computer { get; set; }

    /// <summary>
    /// Flag indicating whether hardware monitoring is ready.
    /// </summary>
    public static bool HardwareMonitoringReady { get; set; }

    /// <summary>
    /// List of hardware to monitor.
    /// </summary>
    public static List<Hardware>? HardwareToMonitor { get; set; }

    /// <summary>
    /// Flag indicating whether render hardware has been collected.
    /// </summary>
    public static bool RenderHardwareCollected { get; set; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // COMPUTER
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Sets up this computer for hardware monitoring.
    /// </summary>
    public static void SetupComputer()
    {
        // Set up the computer, allowing only the CPU, GPU, and Memory to be monitored
        Computer = new Computer
        {
            IsCpuEnabled = true,
            IsGpuEnabled = true,
            IsMemoryEnabled = true,
            IsMotherboardEnabled = false,
            IsControllerEnabled = false,
            IsNetworkEnabled = false,
            IsStorageEnabled = false,
        };

        Computer.Open();
        Computer.Accept(new UpdateVisitor());
        HardwareMonitoringReady = true;
    }

    /// <summary>
    /// Closes the computer.
    /// </summary>
    public static void CloseComputer()
    {
        Computer?.Close();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // HARDWARE MONITORING
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Collects useful hardware components and sensors to monitor for rendering.
    /// </summary>
    public static void CollectHardwareToMonitor()
    {
        // Initialize the list of hardware to monitor
        HardwareToMonitor = new List<Hardware>();

        // Get all hardware
        var hardwareMap = MapHardware(Computer);

        // Cpu details to monitor
        var requiredCpuSensorsTypes = new List<SensorType> { SensorType.Load };
        var requiredCpuSensorNames = new List<string> { "CPU Total" };

        // Gpu details to monitor
        var gpuTypes = new List<HardwareType>
        {
            HardwareType.GpuNvidia,
            HardwareType.GpuAmd,
            HardwareType.GpuIntel,
        };
        var requiredGpuSensorsTypes = new List<SensorType>
        {
            SensorType.Load,
            SensorType.Temperature,
            SensorType.SmallData,
        };
        var requiredGpuSensorNames = new List<string> { "GPU Core", "GPU Memory Used" };

        // Memory details to monitor
        var requiredMemorySensorsTypes = new List<SensorType> { SensorType.Load };
        var requiredMemorySensorNames = new List<string> { "Memory" };

        var cpuHardware = new List<Hardware>();
        var gpuHardware = new List<Hardware>();
        var memoryHardware = new List<Hardware>();

        foreach (var hardware in hardwareMap)
        {
            // Cpu filter
            if (
                hardware.Type == HardwareType.Cpu
                && requiredCpuSensorsTypes.Contains(hardware.SensorType)
                && requiredCpuSensorNames.Contains(hardware.SensorName)
            )
            {
                cpuHardware.Add(hardware);
            }
            // Gpu filter
            else if (
                gpuTypes.Contains(hardware.Type)
                && requiredGpuSensorsTypes.Contains(hardware.SensorType)
                && requiredGpuSensorNames.Contains(hardware.SensorName)
            )
            {
                gpuHardware.Add(hardware);
            }
            // Memory filter
            else if (
                hardware.Type == HardwareType.Memory
                && requiredMemorySensorsTypes.Contains(hardware.SensorType)
                && requiredMemorySensorNames.Contains(hardware.SensorName)
            )
            {
                memoryHardware.Add(hardware);
            }
        }

        // Add all hardware to the RenderHardware list in the order of CPU, GPU, Memory
        HardwareToMonitor.AddRange(cpuHardware);
        HardwareToMonitor.AddRange(gpuHardware);
        HardwareToMonitor.AddRange(memoryHardware);

        // Set the flag to indicate that render hardware has been collected to true
        RenderHardwareCollected = true;
    }

    /// <summary>
    /// Maps the hardware components of the computer so that it can be referenced easily using a path of indices.
    /// </summary>
    /// <param name="computer">This computer.</param>
    /// <returns>A list of hardware components.</returns>
    private static List<Hardware> MapHardware(Computer? computer)
    {
        var map = new List<Hardware>();

        if (computer != null)
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
                            new List<int> { i, j, k }
                        );

                        map.Add(hardware);
                    }
                }

                for (var j = 0; j < computer.Hardware[i].Sensors.Length; j++)
                {
                    var hardware = new Hardware(
                        computer.Hardware[i].Name,
                        computer.Hardware[i].HardwareType,
                        computer.Hardware[i].Sensors[j].Name,
                        computer.Hardware[i].Sensors[j].SensorType,
                        computer.Hardware[i].Sensors[j].Value,
                        FindUnit(computer.Hardware[i].Sensors[j].SensorType),
                        new List<int> { i, j }
                    );

                    map.Add(hardware);
                }
            }

        return map;
    }

    /// <summary>
    /// Gets the most recent value of the specified hardware.
    /// </summary>
    /// <param name="computer">This computer.</param>
    /// <param name="hardware">The hardware to refresh.</param>
    public static void RefreshHardware(Computer? computer, Hardware hardware)
    {
        var path = hardware.Path;
        computer?.Hardware[path[0]].Update();

        // The location of the value depends on the number of indices in the path.
        switch (path.Count)
        {
            // A path length of two indicates that the hardware has a sensor attached to it directly.
            case 2:
            {
                // Update the value.
                hardware.UpdateValue(computer?.Hardware[path[0]].Sensors[path[1]].Value);
                break;
            }
            // A path length of three indicates that the hardware has a sub-hardware with a sensor attached to it.
            case 3:
            {
                // Update the value.
                hardware.UpdateValue(
                    computer?.Hardware[path[0]].SubHardware[path[1]].Sensors[path[2]].Value
                );
                break;
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // HELPER FUNCTIONS
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Formats the name of the hardware for display in the UI.
    /// </summary>
    /// <param name="hardware">The hardware to format.</param>
    /// <returns>The formatted name.</returns>
    public static string FormatName(Hardware hardware)
    {
        // Create a new name to display in the UI
        var newName = hardware.Name;

        // Remove certain words from the name to reduce length in the UI
        var redundantWords = new List<string> { "Generic ", "NVIDIA", "Intel", "AMD", "GeForce" };
        foreach (var word in redundantWords)
        {
            newName = newName.Replace(word, "");
        }

        // If the name ends with a space followed by a single alphanumeric character, remove it, for example
        // "CPU 1" -> "CPU"
        // "CPU A" -> "CPU"
        if (newName.Length > 1 && char.IsLetterOrDigit(newName[^1]) && newName[^2] == ' ')
        {
            newName = newName[..^2];
        }

        // Remove any leading or trailing whitespace
        newName = newName.Trim();

        // Add "VRAM" to the end of GPU memory sensor names
        var gpuTypes = new List<HardwareType>
        {
            HardwareType.GpuNvidia,
            HardwareType.GpuAmd,
            HardwareType.GpuIntel,
        };
        if (gpuTypes.Contains(hardware.Type) && hardware.SensorType == SensorType.SmallData)
        {
            newName = $"{newName} VRAM";
        }

        // Add "Temperature" to the end of temperature sensor names
        if (hardware.SensorType == SensorType.Temperature)
        {
            newName = $"{newName} Temp";
        }

        return newName;
    }

    /// <summary>
    /// Finds the unit for a given sensor type.
    /// </summary>
    /// <param name="sensorType">The sensor type.</param>
    /// <returns>The unit as a string.</returns>
    private static string? FindUnit(SensorType sensorType)
    {
        return sensorType switch
        {
            SensorType.Load => "%",
            SensorType.Power => "W",
            SensorType.Clock => "MHz",
            SensorType.Factor => "?",
            SensorType.Voltage => "V",
            SensorType.Temperature => "°C",
            SensorType.Data => "GB",
            SensorType.Fan => "RPM",
            SensorType.Control => "?",
            SensorType.SmallData => "MB",
            SensorType.Throughput => "KB/s",
            _ => null,
        };
    }
}
