namespace Orthographic.Renderer.Interfaces;

public interface IPathSelector
{
    void FixPath();
    bool CheckPath();
    void MarkValid();
    void MarkInvalid();
    string GetPath();
    void SetPath(string path);
}