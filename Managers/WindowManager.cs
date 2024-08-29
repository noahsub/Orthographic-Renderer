////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// WindowManager.cs
// 
// This file is responsible for managing the windows in the application.
// Author(s): https://github.com/noahsub
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// IMPORTS
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System.Collections.Generic;
using Avalonia.Controls;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// NAMESPACE
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Orthographic.Renderer.Managers;


////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// WINDOW MANAGER CLASS
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

/// <summary>
/// Manages the windows in the application.
/// </summary>
public class WindowManager
{
    /// <summary>
    /// The list of currently open windows in the application.
    /// </summary>
    public static List<Window> Windows { get; private set; } = new List<Window>();
    
    /// <summary>
    /// Adds a window to the list of open windows.
    /// </summary>
    /// <param name="window">The window to add.</param>
    public static void AddWindow(Window window)
    {
        Windows.Add(window);
    }
    
    /// <summary>
    /// Removes a window from the list of open windows.
    /// </summary>
    /// <param name="window">The window to remove.</param>
    public static void RemoveWindow(Window window)
    {
        Windows.Remove(window);
    }
    
    /// <summary>
    /// Closes all Render Complete windows.
    /// </summary>
    public static void CloseAllRenderCompleteWindows()
    {
        // Create a copy of the list of open windows.
        var currentWindows = new List<Window>();
        currentWindows.AddRange(Windows);

        // Iterate through each window in the list of open windows.
        foreach (var window in currentWindows)
        {
            // If the window's title contains "Render Complete", close the window.
            if (window.Title != null && window.Title.Contains("Render Complete"))
            {
                window.Close();
                RemoveWindow(window);
            }
        }
    }
}