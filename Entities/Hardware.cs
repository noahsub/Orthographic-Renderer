using System.Collections.Generic;
using LibreHardwareMonitor.Hardware;

namespace Orthographic.Renderer.Entities;

public class Hardware
{
    public string Name;
    public HardwareType Type { get; set; }
    public string SensorName { get; set; }
    public SensorType SensorType { get; set; }
    public float? Value;
    public string? Unit { get; set; }
    public List<int> Path { get; set; }

    public Hardware(string name, HardwareType type, string sensorName, SensorType sensorType, float? value, string? unit, List<int> path)
    {
        Name = name;
        Type = type;
        SensorName = sensorName;
        SensorType = sensorType;
        Value = value;
        Unit = unit;
        Path = path;
    }

    public override string ToString()
    {
        var pathString = string.Join(", ", Path);
        return $"Name: {Name}, Type: {Type}, SensorName: {SensorName}, SensorType: {SensorType}, Value: {Value}, Unit: {Unit}, Path: {pathString}";
    }

    public void UpdateValue(float? value)
    {
        // Update value, but format to 2 decimal places
        Value = value;
    }
}