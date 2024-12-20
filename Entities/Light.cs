﻿////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Light.cs
// A class that represents a blender area light.
//
// Copyright (C) 2024 noahsub
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// IMPORTS
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using Avalonia.Media;
using Orthographic.Renderer.Managers;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// NAMESPACE
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

namespace Orthographic.Renderer.Entities;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// LIGHT CLASS
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
public class Light
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // PROPERTIES
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// The orientation of the light.
    /// </summary>
    public string View { get; set; }

    /// <summary>
    /// The position of the light, including rotation.
    /// </summary>
    public Position Position { get; set; }

    /// <summary>
    /// The colour of the light.
    /// </summary>
    public string Colour { get; set; }

    /// <summary>
    /// The power of the light.
    /// </summary>
    public float Power { get; set; }

    /// <summary>
    /// The size of the light.
    /// </summary>
    public float Size { get; set; }

    /// <summary>
    /// The distance of the light.
    /// </summary>
    public float Distance { get; set; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // INITIALIZATION
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Creates a new instance of the <see cref="Light"/> class.
    /// </summary>
    /// <param name="view">The orientation of the light.</param>
    /// <param name="colour">The colour of the light.</param>
    /// <param name="power">The power of the light.</param>
    /// <param name="size">The size of the light.</param>
    /// <param name="distance">The distance of the light.</param>
    public Light(string view, Color colour, float power, float size, float distance)
    {
        View = view;
        Position = SceneManager.GetPosition(view, distance);
        Colour = $"{colour.R},{colour.G},{colour.B},{colour.A}";
        Power = power;
        Size = size;
        Distance = distance;
    }
}
