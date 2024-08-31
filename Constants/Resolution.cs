////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Resolution.cs
// This file contains the constants for different screen resolutions.
//
// Copyright (C) 2024 noahsub
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// IMPORTS
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// NAMESPACE
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Orthographic.Renderer.Constants;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// RESOLUTION CLASS
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

/// <summary>
/// Contains constants for different screen resolutions.
/// </summary>
public static class Resolution
{
    /// <summary>
    /// The width and height of 480p resolution.
    /// </summary>
    public static readonly Tuple<int, int> _480p = new(854, 480);

    /// <summary>
    /// The width and height of 720p resolution.
    /// </summary>
    public static readonly Tuple<int, int> _720p = new(1280, 720);

    /// <summary>
    /// The width and height of 1080p resolution.
    /// </summary>
    public static readonly Tuple<int, int> _1080p = new(1920, 1080);

    /// <summary>
    /// The width and height of 1440p resolution.
    /// </summary>
    public static readonly Tuple<int, int> _1440p = new(2560, 1440);

    /// <summary>
    /// The width and height of 4K resolution.
    /// </summary>
    public static readonly Tuple<int, int> _4K = new(3840, 2160);

    /// <summary>
    /// The width and height of 5K resolution.
    /// </summary>
    public static readonly Tuple<int, int> _5K = new(5120, 2880);

    /// <summary>
    /// The width and height of 8K resolution.
    /// </summary>
    public static readonly Tuple<int, int> _8K = new(7680, 4320);

    /// <summary>
    /// The width and height of 10K resolution.
    /// </summary>
    public static readonly Tuple<int, int> _10K = new(10240, 5760);

    /// <summary>
    /// The width and height of 12K resolution.
    /// </summary>
    public static readonly Tuple<int, int> _12K = new(12288, 6480);

    /// <summary>
    /// The width and height of 16K resolution.
    /// </summary>
    public static readonly Tuple<int, int> _16K = new(15360, 8640);
}
