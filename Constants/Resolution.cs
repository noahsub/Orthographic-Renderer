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
using System.Collections.Generic;

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
    /// The default width of a render.
    /// </summary>
    public static readonly int DefaultWidth = 1920;
    
    /// <summary>
    /// The default height of a render.
    /// </summary>
    public static readonly int DefaultHeight = 1080;
    
    /// <summary>
    /// List of 4:3 aspect ratios.
    /// </summary>
    public static readonly List<string> AspectRatio4X3 =
    [
        "VGA",
        "SVGA",
        "XGA",
        "XGA+",
        "SXGA",
        "SXGA+",
        "UXGA",
        "QXGA",
        "QSXGA",
        "QUXGA",
        "HUXGA",
        "WHUXGA",
    ];

    /// <summary>
    /// List of 16:9 aspect ratios.
    /// </summary>
    public static readonly List<string> AspectRatio16X9 =
    [
        "720p",
        "1080p",
        "900p",
        "1440p",
        "4K",
        "5K",
        "8K",
        "10K",
        "12K",
        "14K",
        "16K",
        "24K",
    ];

    /// <summary>
    /// List of 21:9 aspect ratios.
    /// </summary>
    public static readonly List<string> AspectRatio21X9 =
    [
        "UWHD",
        "UWQHD",
        "UWQHD+",
        "UW4K",
        "UW5K",
        "UW8K",
        "UW10K",
        "UW12K",
        "UW14K",
        "UW16K",
        "UW32K",
        "UW64K",
    ];

    /// <summary>
    /// List of 1:1 aspect ratios.
    /// </summary>
    public static readonly List<string> AspectRatio1X1 =
    [
        "640p",
        "800p",
        "1024p",
        "1280p",
        "1600p",
        "1920p",
        "2560p",
        "3200p",
        "3840p",
        "5120p",
        "6400p",
        "8192p",
    ];

    /// <summary>
    /// Mapping of resolution names to their width and height.
    /// </summary>
    public static readonly Dictionary<string, Tuple<int, int>> ResolutionDictionary =
        new Dictionary<string, Tuple<int, int>>
        {
            { "VGA", Tuple.Create(640, 480) },
            { "SVGA", Tuple.Create(800, 600) },
            { "XGA", Tuple.Create(1024, 768) },
            { "XGA+", Tuple.Create(1152, 864) },
            { "SXGA", Tuple.Create(1280, 1024) },
            { "SXGA+", Tuple.Create(1400, 1050) },
            { "UXGA", Tuple.Create(1600, 1200) },
            { "QXGA", Tuple.Create(2048, 1536) },
            { "QSXGA", Tuple.Create(2560, 2048) },
            { "QUXGA", Tuple.Create(3200, 2400) },
            { "HUXGA", Tuple.Create(4096, 3072) },
            { "WHUXGA", Tuple.Create(5120, 4096) },
            { "720p", Tuple.Create(1280, 720) },
            { "1080p", Tuple.Create(1920, 1080) },
            { "900p", Tuple.Create(1600, 900) },
            { "1440p", Tuple.Create(2560, 1440) },
            { "4K", Tuple.Create(3840, 2160) },
            { "5K", Tuple.Create(5120, 2880) },
            { "8K", Tuple.Create(7680, 4320) },
            { "10K", Tuple.Create(10240, 5760) },
            { "12K", Tuple.Create(12288, 6480) },
            { "14K", Tuple.Create(14336, 7776) },
            { "16K", Tuple.Create(15360, 8640) },
            { "24K", Tuple.Create(24576, 13824) },
            { "UWHD", Tuple.Create(2560, 1080) },
            { "UWQHD", Tuple.Create(3440, 1440) },
            { "UWQHD+", Tuple.Create(3840, 1600) },
            { "UW4K", Tuple.Create(5120, 2160) },
            { "UW5K", Tuple.Create(5120, 2160) },
            { "UW8K", Tuple.Create(7680, 3240) },
            { "UW10K", Tuple.Create(10240, 4320) },
            { "UW12K", Tuple.Create(12288, 5184) },
            { "UW14K", Tuple.Create(14336, 6048) },
            { "UW16K", Tuple.Create(15360, 6480) },
            { "UW32K", Tuple.Create(30720, 12960) },
            { "UW64K", Tuple.Create(61440, 25920) },
            { "640p", Tuple.Create(640, 640) },
            { "800p", Tuple.Create(800, 800) },
            { "1024p", Tuple.Create(1024, 1024) },
            { "1280p", Tuple.Create(1280, 1280) },
            { "1600p", Tuple.Create(1600, 1600) },
            { "1920p", Tuple.Create(1920, 1920) },
            { "2560p", Tuple.Create(2560, 2560) },
            { "3200p", Tuple.Create(3200, 3200) },
            { "3840p", Tuple.Create(3840, 3840) },
            { "5120p", Tuple.Create(5120, 5120) },
            { "6400p", Tuple.Create(6400, 6400) },
            { "8192p", Tuple.Create(8192, 8192) },
        };
}
