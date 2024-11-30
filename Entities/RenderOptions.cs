using Newtonsoft.Json;

namespace Orthographic.Renderer.Entities;

using System.Collections.Generic;

public class RenderOptions
{
    public string Name { get; set; }
    public string Model { get; set; }
    public float Unit { get; set; }
    public string OutputDirectory { get; set; }
    public Resolution Resolution { get; set; }
    public Camera Camera { get; set; }
    public List<Light> Lights { get; set; }
    public bool SaveBlenderFile { get; set; }

    public RenderOptions()
    {
        Name = "Render";
        Model = "model.obj";
        Unit = 1;
        OutputDirectory = "output";
        Resolution = new Resolution(1920, 1080);
        Camera = new Camera(0, new Position(0, 0, 0, 0, 0, 0));
        Lights = new List<Light>();
        SaveBlenderFile = false;
    }

    public void SetName(string name)
    {
        Name = name;
    }

    public void SetModel(string model)
    {
        Model = model;
    }

    public void SetUnit(float unit)
    {
        Unit = unit;
    }

    public void SetOutputDirectory(string outputDirectory)
    {
        OutputDirectory = outputDirectory;
    }

    public void SetResolution(Resolution resolution)
    {
        Resolution = resolution;
    }

    public void SetCamera(Camera camera)
    {
        Camera = camera;
    }

    public void AddLight(Light light)
    {
        Lights.Add(light);
    }

    public void AddLights(List<Light> lights)
    {
        Lights.AddRange(lights);
    }

    public void SetSaveBlenderFile(bool saveBlenderFile)
    {
        SaveBlenderFile = saveBlenderFile;
    }

    public string GetJsonRepresentation()
    {
        return JsonConvert.SerializeObject(this);
    }
}
