////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// DataManager.cs
// This file is responsible for managing shared data across the application.
//
// Copyright (C) 2024 noahsub
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// IMPORTS
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Numerics;
using Orthographic.Renderer.Entities;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// NAMESPACE
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Orthographic.Renderer.Managers;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// DATA MANAGER CLASS
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

/// <summary>
/// Manages shared data across the application.
/// </summary>
public static class DataManager
{
    /// <summary>
    /// The path to the Blender executable.
    /// </summary>
    public static string BlenderPath { get; set; } = "";

    /// <summary>
    /// The path to the model.
    /// </summary>
    public static string ModelPath { get; set; } = "";

    /// <summary>
    /// The unit scale of the model.
    /// </summary>
    public static float UnitScale { get; set; } = 0.001f;

    /// <summary>
    /// The current version of the application.
    /// </summary>
    public static string CurrentVersion { get; set; } = File.ReadAllText("VERSION");

    /// <summary>
    /// The latest version of the application.
    /// </summary>
    public static string LatestVersion { get; set; } = CurrentVersion;

    /// <summary>
    /// The distance of the camera from the origin of the model.
    /// </summary>
    public static float CameraDistance { get; set; } = 0.0f;

    /// <summary>
    /// The resolution of the rendered image.
    /// </summary>
    public static Resolution Resolution { get; set; } = new Resolution(0, 0);

    /// <summary>
    /// The lights in the scene.
    /// </summary>
    public static List<Light> Lights { get; set; } = new List<Light>();

    /// <summary>
    /// The background colour of the rendered image.
    /// </summary>
    public static Avalonia.Media.Color BackgroundColour { get; set; } =
        Avalonia.Media.Color.FromRgb(255, 255, 255);

    /// <summary>
    /// The views to render.
    /// </summary>
    public static List<string> SelectedViews { get; set; } = [];

    /// <summary>
    /// The OPTIX devices available.
    /// </summary>
    public static List<string> OptixDevices { get; set; } = [];

    /// <summary>
    /// The CUDA devices available.
    /// </summary>
    public static List<string> CudaDevices { get; set; } = [];

    /// <summary>
    /// The CPU devices available.
    /// </summary>
    public static List<string> CpuDevices { get; set; } = [];

    /// <summary>
    /// The render devices available.
    /// </summary>
    public static List<RenderHardware> RenderDevices = [];

    /// <summary>
    /// The framework to use with the cycle render engine.
    /// </summary>
    public static string RenderFramework { get; set; } = "";

    /// <summary>
    /// The dimensions of the model.
    /// </summary>
    public static Vector3 ModelDimensions { get; set; } = new Vector3(0, 0, 0);

    /// <summary>
    /// The biggest dimension of the model.
    /// </summary>
    public static float ModelMaxDimension { get; set; } = 0.0f;
}
