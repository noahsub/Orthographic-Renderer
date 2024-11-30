////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// DataManager.cs
// This file is responsible for managing shared data across the application.
//
// Copyright (C) 2024 noahsub
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// NAMESPACE
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;
using System.IO;
using HarfBuzzSharp;
using Orthographic.Renderer.Entities;

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

    public static float CameraDistance { get; set; } = 0.0f;

    public static Resolution Resolution { get; set; } = new Resolution(0, 0);

    public static List<Light> Lights { get; set; } = new List<Light>();

    public static List<string> SelectedViews { get; set; } = new List<string>();
}
