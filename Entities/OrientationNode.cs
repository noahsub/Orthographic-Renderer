namespace Orthographic.Renderer.Entities;

public class OrientationNode
{
    public string Name { get; set; }
    public string Icon { get; set; }
    public OrientationNode Left { get; set; }
    public OrientationNode Up { get; set; }
    public OrientationNode Right { get; set; }
    public OrientationNode Down { get; set; }

    public OrientationNode(string name, string icon)
    {
        Name = name;
        Icon = icon;
    }

    public void AddLeft(OrientationNode node)
    {
        Left = node;
    }

    public void AddUp(OrientationNode node)
    {
        Up = node;
    }

    public void AddRight(OrientationNode node)
    {
        Right = node;
    }

    public void AddDown(OrientationNode node)
    {
        Down = node;
    }
}
