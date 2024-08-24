namespace Orthographic.Renderer.Entities;

public class Position
{
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }
    public float Rx { get; set; }
    public float Ry { get; set; }
    public float Rz { get; set; }

    public Position(float x, float y, float z, float rx, float ry, float rz)
    {
        X = x;
        Y = y;
        Z = z;
        Rx = rx;
        Ry = ry;
        Rz = rz;
    }
}
