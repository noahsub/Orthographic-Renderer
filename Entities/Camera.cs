﻿////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Camera.cs
// A class that represents a camera in 3D space.
//
// Copyright (C) 2024 noahsub
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// NAMESPACE
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Orthographic.Renderer.Entities;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// CAMERA CLASS
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

/// <summary>
/// Represents a camera in 3D space.
/// </summary>
public class Camera
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // PROPERTIES
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// The distance from the camera to the origin of the model.
    /// </summary>
    public float Distance { get; set; }

    /// <summary>
    /// The position of the camera, including rotation.
    /// </summary>
    public Position Position { get; set; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // INITIALIZATION
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Creates a new instance of the <see cref="Camera"/> class.
    /// </summary>
    /// <param name="distance">The distance from the camera to the origin of the model.</param>
    /// <param name="position">The position of the camera, including rotation.</param>
    public Camera(float distance, Position position)
    {
        this.Distance = distance;
        this.Position = position;
    }
}
