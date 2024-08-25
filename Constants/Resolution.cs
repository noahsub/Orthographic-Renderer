using System;

namespace Orthographic.Renderer.Entities;

public class Resolution
{
    public static readonly Tuple<int, int> _480p = new(854, 480);
    public static readonly Tuple<int, int> _720p = new(1280, 720);
    public static readonly Tuple<int, int> _1080p = new(1920, 1080);
    public static readonly Tuple<int, int> _1440p = new(2560, 1440);
    public static readonly Tuple<int, int> _4K = new(3840, 2160);
    public static readonly Tuple<int, int> _5K = new(5120, 2880);
    public static readonly Tuple<int, int> _8K = new(7680, 4320);
    public static readonly Tuple<int, int> _10K = new(10240, 5760);
    public static readonly Tuple<int, int> _12K = new(12288, 6480);
    public static readonly Tuple<int, int> _16K = new(15360, 8640);
}