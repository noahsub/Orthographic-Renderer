namespace Orthographic.Renderer.Entities;

public class Camera
{
    public float Distance { get; set; }
    public Position Position { get; set; }
    
    public Camera(float distance, Position position)
    {
        this.Distance = distance;
        this.Position = position;
    }
}