using System.Collections.Generic;

namespace Orthographic.Renderer.Entities;

public class RenderHardware
{
    public string Name { get; set; }
    public string Type { get; set; }
    public List<string> Frameworks { get; set; }

    public RenderHardware(string name, List<string> frameworks)
    {
        Name = name;
        
        if (!frameworks.Contains("CPU CYCLES"))
        {
            Type = "GPU";
        }
        
        else
        {
            Type = "CPU";
        }
        
        Frameworks = frameworks;
    }
}