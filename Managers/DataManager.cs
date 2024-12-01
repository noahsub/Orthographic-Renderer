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
using System.IO;
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
    /// The views to render.
    /// </summary>
    public static List<string> SelectedViews { get; set; } = new List<string>();
}
