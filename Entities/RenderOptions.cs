////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// RenderOptions.cs
// A class that represents the options for rendering a model.
//
// Copyright (C) 2024 noahsub
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// IMPORTS
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using Newtonsoft.Json;
using System.Collections.Generic;


////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// NAMESPACE
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Orthographic.Renderer.Entities;


////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// RENDER OPTIONS CLASS
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

/// <summary>
/// Represents the options for rendering a model.
/// </summary>
public class RenderOptions
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // PROPERTIES
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    
    /// <summary>
    /// The name of the render.
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// The path to the model.
    /// </summary>
    public string Model { get; set; }
    
    /// <summary>
    /// The scale of the model relative to meters.
    /// </summary>
    public float Unit { get; set; }
    
    /// <summary>
    /// The directory to save the output files.
    /// </summary>
    public string OutputDirectory { get; set; }
    
    /// <summary>
    /// The resolution of the output image.
    /// </summary>
    public Resolution Resolution { get; set; }
    
    /// <summary>
    /// The camera to use for rendering.
    /// </summary>
    public Camera Camera { get; set; }
    
    /// <summary>
    /// The lights to use for rendering.
    /// </summary>
    public List<Light> Lights { get; set; }
    
    /// <summary>
    /// Whether to save the Blender file.
    /// </summary>
    public bool SaveBlenderFile { get; set; }

    /// <summary>
    /// Creates a new instance of the <see cref="RenderOptions"/> class.
    /// </summary>
    public RenderOptions()
    {
        // Default values for the render options.
        Name = "Render";
        Model = "model.obj";
        Unit = 1;
        OutputDirectory = "output";
        Resolution = new Resolution(1920, 1080);
        Camera = new Camera(0, new Position(0, 0, 0, 0, 0, 0));
        Lights = new List<Light>();
        SaveBlenderFile = false;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // SETTERS
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    
    /// <summary>
    /// Sets the name of the render.
    /// </summary>
    /// <param name="name">The name of the render.</param>
    public void SetName(string name)
    {
        Name = name;
    }

    /// <summary>
    /// Sets the path to the model.
    /// </summary>
    /// <param name="model">The path to the model.</param>
    public void SetModel(string model)
    {
        Model = model;
    }

    /// <summary>
    /// Sets the scale of the model relative to meters.
    /// </summary>
    /// <param name="unit">The scale of the model relative to meters.</param>
    public void SetUnit(float unit)
    {
        Unit = unit;
    }

    /// <summary>
    /// Sets the directory to save the output files.
    /// </summary>
    /// <param name="outputDirectory">The directory to save the output files.</param>
    public void SetOutputDirectory(string outputDirectory)
    {
        OutputDirectory = outputDirectory;
    }

    /// <summary>
    /// The resolution of the output image.
    /// </summary>
    /// <param name="resolution">The resolution of the output image.</param>
    public void SetResolution(Resolution resolution)
    {
        Resolution = resolution;
    }

    /// <summary>
    /// The camera to use for rendering.
    /// </summary>
    /// <param name="camera">The camera to use for rendering.</param>
    public void SetCamera(Camera camera)
    {
        Camera = camera;
    }

    /// <summary>
    /// The lights to use for rendering.
    /// </summary>
    /// <param name="light">The light to use for rendering.</param>
    public void AddLight(Light light)
    {
        Lights.Add(light);
    }

    /// <summary>
    /// The lights to use for rendering.
    /// </summary>
    /// <param name="lights">The lights to use for rendering.</param>
    public void AddLights(List<Light> lights)
    {
        Lights.AddRange(lights);
    }

    /// <summary>
    /// Whether to save the Blender file.
    /// </summary>
    /// <param name="saveBlenderFile">Whether to save the Blender file.</param>
    public void SetSaveBlenderFile(bool saveBlenderFile)
    {
        SaveBlenderFile = saveBlenderFile;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // JSON REPRESENTATION
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    
    /// <summary>
    /// Converts the render options to a JSON representation.
    /// </summary>
    /// <returns>A JSON representation of the render options.</returns>
    public string GetJsonRepresentation()
    {
        return JsonConvert.SerializeObject(this);
    }
}
