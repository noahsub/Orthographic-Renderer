////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// ViewManager.cs
// Manages render orientation operations.
//
// Copyright (C) 2024 noahsub
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// IMPORTS
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System.Collections.Generic;
using System.Linq;
using Orthographic.Renderer.Constants;
using Orthographic.Renderer.Pages;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// NAMESPACE
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Orthographic.Renderer.Managers;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// VIEW MANAGER CLASS
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

/// <summary>
/// Manages render orientation operations.
/// </summary>
public class ViewManager
{
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

        foreach (var view in View.RenderViews)
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
    
    /// <summary>
    /// Gets the formatted view name by replacing hyphens with spaces and converting to title case.
    /// </summary>
    /// <param name="view">The view name.</param>
    /// <returns>The formatted view name.</returns>
    public static string GetFormattedViewName(string view)
    {
        var formattedName = view;
        formattedName = formattedName.Replace("-", " ");
        formattedName = TextManager.ToTitleCase(formattedName);
        return formattedName;
    }
}