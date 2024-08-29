////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// RenderValues.cs
// This file contains the constants for default render values.
//
// Author(s): https://github.com/noahsub
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// NAMESPACE
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Orthographic.Renderer.Constants;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// RENDER VALUES CLASS
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

/// <summary>
/// Contains constants for default render values.
/// </summary>
public static class RenderValues
{
    /// <summary>
    /// The default mode index.
    /// </summary>
    public const int DefaultMode = 0;

    /// <summary>
    /// The default number of threads.
    /// </summary>
    public const int DefaultThreads = 1;

    /// <summary>
    /// The default prefix for render files.
    /// </summary>
    public const string DefaultPrefix = "Render";

    /// <summary>
    /// The default distance between the camera and origin.
    /// </summary>
    public const decimal DefaultDistance = 8.0m;

    /// <summary>
    /// The default distance between the lights and origin.
    /// </summary>
    public const decimal DefaultLightDistance = 8.0m;

    /// <summary>
    /// The default render resolution width.
    /// </summary>
    public const int DefaultResolutionWidth = 1920;

    /// <summary>
    /// The default render resolution height.
    /// </summary>
    public const int DefaultResolutionHeight = 1080;

    /// <summary>
    /// The default render scale.
    /// </summary>
    public const int DefaultScale = 100;
}
