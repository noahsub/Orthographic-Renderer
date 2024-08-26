////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Hardware.cs
// This file contains the hardware entity class for representing hardware components.
//
// Author(s): https://github.com/noahsub
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// IMPORTS
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System.Collections.Generic;
using LibreHardwareMonitor.Hardware;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// NAMESPACE
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Orthographic.Renderer.Entities;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// HARDWARE CLASS
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

/// <summary>
/// A hardware component.
/// </summary>
public class Hardware
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // PROPERTIES
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// The name of the hardware component.
    /// </summary>
    public string Name;

    /// <summary>
    /// The type of the hardware component.
    /// </summary>
    public HardwareType Type { get; set; }

    /// <summary>
    /// The name of the sensor associated with the hardware component.
    /// </summary>
    public string SensorName { get; set; }

    /// <summary>
    /// The type of the sensor associated with the hardware component.
    /// </summary>
    public SensorType SensorType { get; set; }

    /// <summary>
    /// The value of the sensor reading.
    /// </summary>
    public float Value;

    /// <summary>
    /// The unit of the sensor reading.
    /// </summary>
    public string Unit { get; set; }

    /// <summary>
    /// The path to the hardware component.
    /// </summary>
    public List<int> Path { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Hardware"/> class.
    /// </summary>
    /// <param name="name">The name of the hardware component.</param>
    /// <param name="type">The type of the hardware component.</param>
    /// <param name="sensorName">The name of the sensor associated with the hardware component.</param>
    /// <param name="sensorType">The type of the sensor associated with the hardware component.</param>
    /// <param name="value">The value of the sensor reading.</param>
    /// <param name="unit">The unit of the sensor reading.</param>
    /// <param name="path">The path to the hardware component.</param>
    public Hardware(
        string name,
        HardwareType type,
        string sensorName,
        SensorType sensorType,
        float value,
        string unit,
        List<int> path
    )
    {
        Name = name;
        Type = type;
        SensorName = sensorName;
        SensorType = sensorType;
        Value = value;
        Unit = unit;
        Path = path;
    }

    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString()
    {
        var pathString = string.Join(", ", Path);
        return $"Name: {Name}, Type: {Type}, SensorName: {SensorName}, SensorType: {SensorType}, Value: {Value}, Unit: {Unit}, Path: {pathString}";
    }

    /// <summary>
    /// Updates the value of the sensor reading.
    /// </summary>
    /// <param name="value">The new value of the sensor reading.</param>
    public void UpdateValue(float value)
    {
        // Update value, but format to 2 decimal places
        Value = value;
    }
}
