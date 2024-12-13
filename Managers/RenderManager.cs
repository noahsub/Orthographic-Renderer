////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// RenderManager.cs
// This file contains the logic for managing rendering operations within the application.
//
// Copyright (C) 2024 noahsub
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// IMPORTS
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using Orthographic.Renderer.Entities;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// NAMESPACE
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Orthographic.Renderer.Managers;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// RENDER MANAGER CLASS
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

/// <summary>
/// Manages rendering operations within the application.
/// </summary>
public static class RenderManager
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // RENDERING
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Renders the model with the specified options.
    /// </summary>
    /// <param name="renderOptions">The options to use when rendering the model.</param>
    /// <param name="quality">The quality of the render.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>True if the model was rendered successfully, otherwise false.</returns>
    public static async Task<bool> Render(
        RenderOptions renderOptions,
        string quality,
        CancellationToken cancellationToken
    )
    {
        // Get the JSON representation of the render options
        var jsonRenderOptions = renderOptions.GetJsonRepresentation().Replace("\"", "\\\"");
        // Get the path to the render script
        var scriptPath = FileManager.GetAbsolutePath("Scripts/render.py");

        // Try to run the process
        try
        {
            return await Task.Run(
                () =>
                {
                    // If the cancellation token has been requested, return false
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return false;
                    }

                    // Run the blender process with the provided arguments
                    return ProcessManager.RunProcessCheck(
                        DataManager.BlenderPath,
                        $"-b -P \"{scriptPath}\" -- "
                            + $"--options \"{jsonRenderOptions}\" --quality {quality}"
                    );
                },
                cancellationToken
            );
        }
        // Otherwise, return false
        catch (OperationCanceledException)
        {
            return false;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // SERIALIZATION
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Saves the base render options to the data manager.
    /// </summary>
    /// <param name="renderOptions"></param>
    public static void SaveRenderOptions(RenderOptions renderOptions)
    {
        // Save the camera distance, resolution, lights, and background colour
        DataManager.CameraDistance = renderOptions.Camera.Distance;
        DataManager.Resolution = renderOptions.Resolution;
        DataManager.Lights = renderOptions.Lights;

        var colourComponents = renderOptions.BackgroundColour.Split(',');
        var red = byte.Parse(colourComponents[0]);
        var green = byte.Parse(colourComponents[1]);
        var blue = byte.Parse(colourComponents[2]);
        var alpha = byte.Parse(colourComponents[3]);
        DataManager.BackgroundColour = Avalonia.Media.Color.FromArgb(alpha, red, green, blue);
    }
}
