namespace Orthographic.Renderer.Entities;

public class Resolution
{
    public int Width { get; set; }
    public int Height { get; set; }
    public int Scale { get; set; }

    public Resolution(int width, int height, int scale = 100)
    {
        Width = width;
        Height = height;
        Scale = scale;
    }
}