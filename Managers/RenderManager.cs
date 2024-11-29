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
using System.Globalization;
using System.Linq;
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
    // GLOBALS
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// A list of all the views that can be rendered.
    /// </summary>
    public static List<string> RenderViews { get; } =
        [
            "top-front-right",
            "top-right-back",
            "top-back-left",
            "top-left-front",
            "front-right-bottom",
            "right-back-bottom",
            "back-left-bottom",
            "left-front-bottom",
            "top-front",
            "top-right",
            "top-back",
            "top-left",
            "front-bottom",
            "right-bottom",
            "back-bottom",
            "left-bottom",
            "front-right",
            "right-back",
            "back-left",
            "left-front",
            "front",
            "right",
            "back",
            "left",
            "top",
            "bottom",
        ];

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

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // VIEWS
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Sorts the views based on the given keys.
    /// </summary>
    /// <param name="keys">The keys to sort by.</param>
    /// <returns>The sorted list of views.</returns>
    public static List<string> SortViews(List<string> keys)
    {
        var exactMatches = new List<string>();
        var subsets = new List<string>();
        var supersets = new List<string>();
        var noMatches = new List<string>();

        foreach (var view in RenderViews)
        {
            var viewFaces = view.Split('-');

            // if viewFaces is an equal set to keys
            if (viewFaces.Length == keys.Count && viewFaces.All(keys.Contains))
            {
                exactMatches.Add(view);
            }
            // if viewFaces is a subset of keys
            else if (viewFaces.All(keys.Contains))
            {
                subsets.Add(view);
            }
            // if viewFaces is a superset of keys
            else if (keys.All(viewFaces.Contains))
            {
                supersets.Add(view);
            }
            else
            {
                noMatches.Add(view);
            }
        }

        // combine the lists in the order of x, y, z, w
        return exactMatches.Concat(subsets).Concat(supersets).Concat(noMatches).ToList();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // FORMATTING
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Gets the formatted view name by replacing hyphens with spaces and converting to title case.
    /// </summary>
    /// <param name="view">The view name.</param>
    /// <returns>The formatted view name.</returns>
    public static string GetFormattedViewName(string view)
    {
        var formattedName = view;
        formattedName = formattedName.Replace("-", " ");
        formattedName = ToTitleCase(formattedName);
        return formattedName;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // HELPER FUNCTIONS
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

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
    /// Converts a string to title case.
    /// </summary>
    /// <param name="str">The string to convert.</param>
    /// <returns>The string in title case.</returns>
    private static string ToTitleCase(string str)
    {
        return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str.ToLower());
    }

    public static bool Render(RenderOptions renderOptions)
    {
        var jsonRenderOptions = renderOptions.GetJsonRepresentation().Replace("\"", "\\\"");
        Debug.WriteLine(jsonRenderOptions);
        var scriptPath = FileManager.GetAbsolutePath("Scripts/render2.py");
        return ProcessManager.RunProcessCheck(DataManager.BlenderPath, $"-b -P \"{scriptPath}\" -- " + $"--options \"{jsonRenderOptions}\"");
    }
}
