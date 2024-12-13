////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// SceneManager.cs
// This file contains the logic for the SceneManager.
//
// Copyright (C) 2024 noahsub
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// IMPORTS
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Drawing;
using Orthographic.Renderer.Controls;
using Orthographic.Renderer.Entities;

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// NAMESPACE
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

namespace Orthographic.Renderer.Managers;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// SCENE MANAGER CLASS
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
public class SceneManager
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // LIGHTING
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Gets the light properties from a light setup item.
    /// </summary>
    /// <param name="lightSetupItem">The light setup item.</param>
    /// <returns>A light with the properties from the light setup item.</returns>
    public static Light GetLight(LightSetupItem lightSetupItem)
    {
        // Get the light properties
        var lightOrientation = lightSetupItem.LightOrientationSelector.CurrentOrientation.Name;
        // We only want the first 7 characters of the hex colour, meaning the alpha channel is ignored
        var lightColour = lightSetupItem.LightColourSelector.ColourPicker.Color;
        var lightPower = float.Parse(lightSetupItem.PowerValueSelector.ValueTextBox.Text ?? "0");
        var lightSize = float.Parse(lightSetupItem.SizeValueSelector.ValueTextBox.Text ?? "0");
        var lightDistance = float.Parse(
            lightSetupItem.DistanceValueSelector.ValueTextBox.Text ?? "0"
        );
        var lightPosition = GetPosition(lightOrientation, lightDistance);

        // Create the light
        return new Light(lightOrientation, lightColour, lightPower, lightSize, lightDistance);
    }

    /// <summary>
    /// Gets a list of lights from a list of light setup items.
    /// </summary>
    /// <param name="lightSetupItems">The list of light setup items.</param>
    /// <returns>A list of lights with the properties from the light setup items.</returns>
    public static List<Light> GetLights(List<LightSetupItem> lightSetupItems)
    {
        // Get the lights
        var lights = new List<Light>();
        foreach (var lightSetupItem in lightSetupItems)
        {
            lights.Add(GetLight(lightSetupItem));
        }
        // Return the lights
        return lights;
    }

    /// <summary>
    /// Sets up one-point lighting for the scene.
    /// </summary>
    /// <returns></returns>
    public static List<Light> SetupOnePointLighting()
    {
        // The optimal distance for the light
        var distance = (float)ComputeOptimalLightDistance(DataManager.ModelMaxDimension);

        // The lights to use for one-point lighting
        var lights = new List<Light>
        {
            new Light("front", Avalonia.Media.Colors.White, 1000, 3, distance),
        };

        return lights;
    }

    /// <summary>
    /// Sets up two-point lighting for the scene.
    /// </summary>
    /// <returns></returns>
    public static List<Light> SetupThreePointLighting()
    {
        // The optimal distance for the light
        var distance = (float)ComputeOptimalLightDistance(DataManager.ModelMaxDimension);

        // The lights to use for three-point lighting
        var lights = new List<Light>
        {
            new Light("top-right-back", Avalonia.Media.Colors.White, 200, 3, distance),
            new Light("top-back-left", Avalonia.Media.Colors.White, 1000, 3, distance),
            new Light("top-left-front", Avalonia.Media.Colors.White, 800, 3, distance),
        };

        return lights;
    }

    /// <summary>
    /// Sets up overhead lighting for the scene.
    /// </summary>
    /// <returns></returns>
    public static List<Light> SetupOverheadLighting()
    {
        // The optimal distance for the light
        var distance = (float)ComputeOptimalLightDistance(DataManager.ModelMaxDimension);

        // The lights to use for overhead lighting
        var lights = new List<Light>
        {
            new Light("top", Avalonia.Media.Colors.White, 1000, 3, distance),
        };

        return lights;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // POSITION
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Gets the position for a given view and distance.
    /// </summary>
    /// <param name="view">The view name.</param>
    /// <param name="distance">The distance of the camera from the origin.</param>
    /// <returns>The position corresponding to the view and distance.</returns>
    public static Position GetPosition(string view, float distance)
    {
        // Compute the leg of the right-angled triangle
        var leg = ComputeTriangularLeg(distance);
        // Mapping of views to positions
        var mapping = new Dictionary<string, Position>
        {
            { "top-front-right", new Position(leg, -leg, distance, 45, 0, 45) },
            { "top-right-back", new Position(leg, leg, distance, 45, 0, 135) },
            { "top-back-left", new Position(-leg, leg, distance, 45, 0, 225) },
            { "top-left-front", new Position(-leg, -leg, distance, 45, 0, 315) },
            { "front-right-bottom", new Position(leg, -leg, -distance, 135, 0, 45) },
            { "right-back-bottom", new Position(leg, leg, -distance, 315, 180, -45) },
            { "back-left-bottom", new Position(-leg, leg, -distance, 315, -180, 45) },
            { "left-front-bottom", new Position(-leg, -leg, -distance, 135, 0, 315) },
            { "top-front", new Position(0, -leg, leg, 45, 0, 0) },
            { "top-right", new Position(leg, 0, leg, 45, 0, 90) },
            { "top-back", new Position(0, leg, leg, 225, 180, 0) },
            { "top-left", new Position(-leg, 0, leg, 45, 0, 270) },
            { "front-bottom", new Position(0, -leg, -leg, 135, 0, 0) },
            { "right-bottom", new Position(leg, 0, -leg, 135, 0, 90) },
            { "back-bottom", new Position(0, leg, -leg, 315, 180, 0) },
            { "left-bottom", new Position(-leg, 0, -leg, 135, 0, 270) },
            { "front-right", new Position(leg, -leg, 0, 90, 0, 45) },
            { "right-back", new Position(leg, leg, 0, 90, 0, 135) },
            { "back-left", new Position(-leg, leg, 0, 90, 0, 225) },
            { "left-front", new Position(-leg, -leg, 0, 90, 0, 315) },
            { "front", new Position(0, -distance, 0, 90, 0, 0) },
            { "right", new Position(distance, 0, 0, 90, 0, 90) },
            { "back", new Position(0, distance, 0, 90, 0, 180) },
            { "left", new Position(-distance, 0, 0, 90, 0, 270) },
            { "top", new Position(0, 0, distance, 0, 0, 0) },
            { "bottom", new Position(0, 0, -distance, 180, 0, 180) },
        };
        return mapping[view];
    }

    /// <summary>
    /// Computes the leg of a triangle given the hypotenuse.
    /// </summary>
    /// <param name="distance">The hypotenuse of the triangle.</param>
    /// <returns>The leg of the triangle.</returns>
    private static float ComputeTriangularLeg(float distance)
    {
        return (float)Math.Sqrt(Math.Pow(distance, 2) / 2);
    }

    /// <summary>
    /// Computes the optimal camera distance based on the size of the model.
    /// </summary>
    /// <param name="size"></param>
    /// <returns></returns>
    public static double ComputeOptimalCameraDistance(float size)
    {
        // Assuming size is in meters
        // The formula for the optimal camera based on testing
        return 3.2079307352327824 * Math.Pow(size, 1.0950872359751485) + 0.10375328078135429;
    }

    /// <summary>
    /// Computes the optimal light distance based on the size of the model.
    /// </summary>
    /// <param name="size"></param>
    /// <returns></returns>
    public static double ComputeOptimalLightDistance(float size)
    {
        // The formula for the optimal light distance based on testing
        // Needs to be tested for larger models
        return 8;
    }
}
