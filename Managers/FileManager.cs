using System.IO;
using Newtonsoft.Json;

namespace Orthographic.Renderer.Managers;

public class FileManager
{
    public static dynamic? ReadJsonFile(string path)
    {
        var text = File.ReadAllText(path);
        return JsonConvert.DeserializeObject(text) ?? null;
    }
    
    public static void WriteToJsonFile(string path, string key, string value)
    {
        var json = ReadJsonFile(path);
        if (json == null)
        {
            return;
        }
        
        json[key] = value;
        File.WriteAllText(path, JsonConvert.SerializeObject(json));
    }
    
    public static string ReformatPath(string path)
    {
        var newPath = path;
        
        // Remove double quotes in the path
        newPath = newPath.Replace("\"", "");
        
        // Convert backslashes to forward slashes
        newPath = newPath.Replace("\\", "/");
        
        // Account for spaces in the path that have been replaced with %20
        newPath = newPath.Replace("%20", " ");
        
        return newPath;
    }
    
    public static string GetAbsolutePath(string localPath)
    {
        return Path.GetFullPath(localPath);
    }
}