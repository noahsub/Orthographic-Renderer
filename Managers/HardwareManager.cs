////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// HardwareManager.cs
// This file manages hardware monitoring of rendering related components.
//
// Copyright (C) 2024 noahsub
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// IMPORTS
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.ComponentModel;
using LibreHardwareMonitor.Hardware;
using LibreHardwareMonitor.Hardware.Cpu;
using Newtonsoft.Json.Linq;
using Orthographic.Renderer.Entities;
using Hardware = Orthographic.Renderer.Entities.Hardware;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// NAMESPACE
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Orthographic.Renderer.Managers;

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
public static class HardwareManager
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // GLOBAL VARIABLES
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// This computer.
    /// </summary>
    public static Computer Computer { get; private set; } = new();

    /// <summary>
    /// List of hardware to monitor.
    /// </summary>
    public static List<Hardware> HardwareToMonitor { get; private set; } = [];

    /// <summary>
    /// Flag indicating whether render hardware has been collected.
    /// </summary>
    public static bool RenderHardwareCollected { get; private set; }

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
    }

    /// <summary>
    /// Closes the computer.
    /// </summary>
    public static void CloseComputer()
    {
        Computer.Close();
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
        HardwareToMonitor = [];

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

        if (gpuHardware.Count == 0)
        {
            try
            {
                gpuHardware = GetNvidiaGpuHardware();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
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
    /// Gets the hardware components of NVIDIA GPUs using the nvidia-smi command.
    /// </summary>
    /// <returns>A list of NVIDIA GPU hardware components.</returns>
    private static List<Hardware> GetNvidiaGpuHardware()
    {
        // excute `nvidia-smi --query-gpu=count --format=csv,noheader` to get the number of NVIDIA GPUs
        var gpuCount = int.Parse(
            ProcessManager.RunProcess(
                "/bin/bash",
                "-c \"nvidia-smi --query-gpu=count --format=csv,noheader\""
            )
        );

        var gpuHardware = new List<Hardware>();
        for (var i = 0; i < gpuCount; i++)
        {
            // execute `nvidia-smi -i 0 --query-gpu=name,temperature.gpu,utilization.gpu,memory.used --format=csv,noheader,nounits`
            var gpuInfo = ProcessManager
                .RunProcess(
                    "/bin/bash",
                    $"-c \"nvidia-smi -i {i} --query-gpu=name,temperature.gpu,utilization.gpu,memory.used --format=csv,noheader,nounits\""
                )
                .Split(", ");
            var gpuName = gpuInfo[0];
            var gpuTemperature = float.Parse(gpuInfo[1]);
            var gpuLoad = float.Parse(gpuInfo[2]);
            var gpuMemoryUsed = float.Parse(gpuInfo[3]);

            gpuHardware.Add(
                new Hardware(
                    gpuName,
                    HardwareType.GpuNvidia,
                    "Temperature",
                    SensorType.Temperature,
                    gpuTemperature,
                    FindUnit(SensorType.Temperature),
                    [i]
                )
            );
            gpuHardware.Add(
                new Hardware(
                    gpuName,
                    HardwareType.GpuNvidia,
                    "GPU Core",
                    SensorType.Load,
                    gpuLoad,
                    FindUnit(SensorType.Load),
                    [i]
                )
            );
            gpuHardware.Add(
                new Hardware(
                    gpuName,
                    HardwareType.GpuNvidia,
                    "GPU Memory Used",
                    SensorType.SmallData,
                    gpuMemoryUsed,
                    FindUnit(SensorType.SmallData),
                    [i]
                )
            );
        }

        return gpuHardware;
    }

    /// <summary>
    /// Maps the hardware components of the computer so that it can be referenced easily using a path of indices.
    /// </summary>
    /// <param name="computer">This computer.</param>
    /// <returns>A list of hardware components.</returns>
    private static List<Hardware> MapHardware(Computer computer)
    {
        // List to store the mapped hardware
        var map = new List<Hardware>();

        // Iterate through all hardware components
        for (var i = 0; i < computer.Hardware.Count; i++)
        {
            // Map the hardware with sub-hardware and sensors
            for (var j = 0; j < computer.Hardware[i].SubHardware.Length; j++)
            {
                for (var k = 0; k < computer.Hardware[i].SubHardware[j].Sensors.Length; k++)
                {
                    var hardware = new Hardware(
                        computer.Hardware[i].SubHardware[j].Name,
                        computer.Hardware[i].SubHardware[j].HardwareType,
                        computer.Hardware[i].SubHardware[j].Sensors[k].Name,
                        computer.Hardware[i].SubHardware[j].Sensors[k].SensorType,
                        computer.Hardware[i].SubHardware[j].Sensors[k].Value ?? 0,
                        FindUnit(computer.Hardware[i].SubHardware[j].Sensors[k].SensorType),
                        [i, j, k]
                    );

                    map.Add(hardware);
                }
            }

            // Map the hardware with only sensors
            for (var j = 0; j < computer.Hardware[i].Sensors.Length; j++)
            {
                var hardware = new Hardware(
                    computer.Hardware[i].Name,
                    computer.Hardware[i].HardwareType,
                    computer.Hardware[i].Sensors[j].Name,
                    computer.Hardware[i].Sensors[j].SensorType,
                    computer.Hardware[i].Sensors[j].Value ?? 0,
                    FindUnit(computer.Hardware[i].Sensors[j].SensorType),
                    [i, j]
                );

                map.Add(hardware);
            }
        }

        // Return the mapped hardware
        return map;
    }

    /// <summary>
    /// Gets the most recent value of the specified hardware.
    /// </summary>
    /// <param name="computer">This computer.</param>
    /// <param name="hardware">The hardware to refresh.</param>
    public static void RefreshHardware(Computer computer, Hardware hardware)
    {
        var path = hardware.Path;

        // The location of the value depends on the number of indices in the path.
        switch (path.Count)
        {
            // A path length of one indicates that the value was obtained using something other than libre hardware
            // monitor, such as nvidia-smi.
            case 1:
                RefreshNvidiaGpuHardware(hardware);
                break;

            // A path length of two indicates that the hardware has a sensor attached to it directly.
            case 2:
            {
                // Update the value.
                computer.Hardware[path[0]].Update();
                var value = computer.Hardware[path[0]].Sensors[path[1]].Value;
                if (value != null)
                {
                    hardware.UpdateValue((float)value);
                }
                else
                {
                    hardware.UpdateValue(0);
                }
                break;
            }
            // A path length of three indicates that the hardware has a sub-hardware with a sensor attached to it.
            case 3:
            {
                // Update the value.
                computer.Hardware[path[0]].Update();
                var value = computer.Hardware[path[0]].SubHardware[path[1]].Sensors[path[2]].Value;
                if (value != null)
                {
                    hardware.UpdateValue((float)value);
                }
                else
                {
                    hardware.UpdateValue(0);
                }
                break;
            }
        }
    }

    /// <summary>
    /// Refreshes the NVIDIA GPU hardware components using the nvidia-smi command.
    /// </summary>
    /// <param name="hardware"></param>
    private static void RefreshNvidiaGpuHardware(Hardware hardware)
    {
        // If the hardware is not an Nvidia GPU, return it as is
        if (hardware.Type != HardwareType.GpuNvidia)
        {
            return;
        }

        switch (hardware.SensorType)
        {
            case SensorType.Temperature:
                // execute `nvidia-smi -i 0 --query-gpu=temperature.gpu --format=csv,noheader,nounits`
                var temperature = ProcessManager.RunProcess(
                    "/bin/bash",
                    $"-c \"nvidia-smi -i {hardware.Path[0]} --query-gpu=temperature.gpu --format=csv,noheader,nounits\""
                );
                hardware.UpdateValue(float.Parse(temperature));
                break;
            case SensorType.Load:
                // execute `nvidia-smi -i 0 --query-gpu=utilization.gpu --format=csv,noheader,nounits`
                var load = ProcessManager.RunProcess(
                    "/bin/bash",
                    $"-c \"nvidia-smi -i {hardware.Path[0]} --query-gpu=utilization.gpu --format=csv,noheader,nounits\""
                );
                hardware.UpdateValue(float.Parse(load));
                break;
            case SensorType.SmallData:
                // execute `nvidia-smi -i 0 --query-gpu=memory.used --format=csv,noheader,nounits`
                var memoryUsed = ProcessManager.RunProcess(
                    "/bin/bash",
                    $"-c \"nvidia-smi -i {hardware.Path[0]} --query-gpu=memory.used --format=csv,noheader,nounits\""
                );
                hardware.UpdateValue(float.Parse(memoryUsed));
                break;
            default:
                throw new InvalidEnumArgumentException();
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // RENDER HARDWARE
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Reads the render capable hardware from Blender.
    /// </summary>
    public static void GetRenderHardware()
    {
        // The output of the Blender command to get the render devices
        var output = ProcessManager.RunProcess(
            DataManager.BlenderPath,
            $"-b --python Scripts/render_devices.py"
        );
        // Clean up the output to get the JSON data
        var jsonStartIndex = output.IndexOf('{');
        var jsonEndIndex = output.LastIndexOf('}');
        // Parse the JSON data
        var renderDevices = JObject.Parse(
            output.Substring(jsonStartIndex, jsonEndIndex - jsonStartIndex + 1)
        );

        // Set the render devices
        var optixDevices = renderDevices["OPTIX"]?.ToObject<List<string>>() ?? [];
        var cudaDevices = renderDevices["CUDA"]?.ToObject<List<string>>() ?? [];
        var cpuDevices = renderDevices["CPU"]?.ToObject<List<string>>() ?? [];

        // Set the render devices in the data manager
        DataManager.OptixDevices = optixDevices;
        DataManager.CudaDevices = cudaDevices;
        DataManager.CpuDevices = cpuDevices;

        // Create a list of render hardware
        var devices = new List<RenderHardware>();
        foreach (var device in optixDevices)
        {
            var frameworks = new List<string> { "OPTIX" };

            // Set the render framework to OPTIX
            DataManager.RenderFramework = "OPTIX";

            if (cudaDevices.Contains(device))
            {
                cudaDevices.Remove(device);
                frameworks.Add("CUDA");
            }

            if (cpuDevices.Contains(device))
            {
                cpuDevices.Remove(device);
                frameworks.Add("CPU CYCLES");
            }

            devices.Add(new RenderHardware(device, frameworks));
        }

        foreach (var device in cudaDevices)
        {
            var frameworks = new List<string> { "CUDA" };

            if (DataManager.RenderFramework == "")
            {
                DataManager.RenderFramework = "CUDA";
            }

            if (cpuDevices.Contains(device))
            {
                cpuDevices.Remove(device);
                frameworks.Add("CPU CYCLES");
            }

            devices.Add(new RenderHardware(device, frameworks));
        }

        foreach (var device in cpuDevices)
        {
            if (DataManager.RenderFramework == "")
            {
                DataManager.RenderFramework = "CPU CYCLES";
            }

            devices.Add(new RenderHardware(device, new List<string> { "CPU CYCLES" }));
        }

        DataManager.RenderDevices = devices;
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
        var redundantWords = new List<string>
        {
            "Generic ",
            "NVIDIA",
            "Intel",
            "AMD",
            "GeForce",
            "Laptop",
            "(TM)",
        };
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

        // replace spaces greater than 1 with a single space
        newName = System.Text.RegularExpressions.Regex.Replace(newName, @"\s+", " ");

        if (gpuTypes.Contains(hardware.Type) && hardware.SensorType == SensorType.SmallData)
        {
            newName = $"{newName} VRAM";
        }

        // Add "Temperature" to the end of temperature sensor names
        if (hardware.SensorType == SensorType.Temperature)
        {
            newName = $"{newName} Temp";
        }

        // If the name is longer than 18 characters, truncate it and add "..."
        if (newName.Length > 18)
        {
            newName = newName[..18] + "...";
        }

        return newName;
    }

    /// <summary>
    /// Finds the unit for a given sensor type.
    /// </summary>
    /// <param name="sensorType">The sensor type.</param>
    /// <returns>The unit as a string.</returns>
    private static string FindUnit(SensorType sensorType)
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
            _ => "NA",
        };
    }
}
