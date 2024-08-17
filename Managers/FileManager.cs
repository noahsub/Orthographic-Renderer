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
}