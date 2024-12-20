﻿////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// ImageManager.cs
// This file contains the logic for managing images within the application.
//
// Copyright (C) 2024 noahsub
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// IMPORTS
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Numerics;
using Orthographic.Renderer.Constants;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// NAMESPACE
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

namespace Orthographic.Renderer.Managers;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// IMAGE MANAGER CLASS
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

/// <summary>
/// Manages images.
/// </summary>
public class ImageManager
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // RESOLUTION
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Takes a resolution name and converts it to a resolution.
    /// For example, "1080p" would be converted to a resolution of 1920x1080.
    /// </summary>
    /// <param name="resolutionName">The name of the resolution.</param>
    /// <returns>The resolution associated with the resolution name.</returns>
    public static Entities.Resolution ConvertResolutionNameToResolution(string resolutionName)
    {
        // Get the width and height of the resolution from the resolution dictionary.
        var resolution = Resolution.ResolutionDictionary[resolutionName];
        // Return the resolution.
        return new Entities.Resolution(resolution.Item1, resolution.Item2);
    }

    /// <summary>
    /// Resizes a resolution to fit within a maximum dimension.
    /// </summary>
    /// <param name="resolution">The resolution to resize.</param>
    /// <param name="maxDimension">The maximum dimension to fit within.</param>
    /// <returns></returns>
    public static Entities.Resolution ResizeResolution(
        Entities.Resolution resolution,
        int maxDimension
    )
    {
        // Compute aspect ratio by determining the GCD of the width and height
        var gcd = (int)BigInteger.GreatestCommonDivisor(resolution.Width, resolution.Height);
        var aspectRatioWidth = resolution.Width / gcd;
        var aspectRatioHeight = resolution.Height / gcd;

        var previewResolution = new Entities.Resolution(
            maxDimension,
            (maxDimension * aspectRatioHeight) / aspectRatioWidth
        );

        if (
            previewResolution.Width * previewResolution.Height
            < resolution.Width * resolution.Height
        )
        {
            resolution = previewResolution;
        }

        return resolution;
    }
}
