namespace Orthographic.Renderer.Entities;

public class Light
{
    public Position Position { get; set; }
    public string Colour { get; set; }
    public float Power { get; set; }
    public float Size { get; set; }
    public float Distance { get; set; }

    public Light(Position position, string colour, float power, float size, float distance)
    {
        Position = position;
        Colour = colour;
        Power = power;
        Size = size;
        Distance = distance;
    }
}