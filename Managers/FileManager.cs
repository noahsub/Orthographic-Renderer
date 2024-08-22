using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Orthographic.Renderer.Managers;

public class FileManager
{
    public static dynamic? ReadJsonKeyValue(string path)
    {
        var text = File.ReadAllText(path);
        return JsonConvert.DeserializeObject(text) ?? null;
    }
    
    public static List<string?> ReadJsonArray(string path, string key)
    {
        var text = File.ReadAllText(path);
        var json = JsonConvert.DeserializeObject<Dictionary<string, object>>(text);
        if (json != null && json.TryGetValue(key, out var value) && value is Newtonsoft.Json.Linq.JArray jArray)
        {
            return jArray.ToObject<List<string?>>();
        }
        return new List<string?>();
    }
    
    public static void WriteToJsonFile(string path, string key, string value)
    {
        var json = ReadJsonKeyValue(path);
        if (json == null)
        {
            return;
        }
        
        json[key] = value;
        File.WriteAllText(path, JsonConvert.SerializeObject(json, Formatting.Indented));
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