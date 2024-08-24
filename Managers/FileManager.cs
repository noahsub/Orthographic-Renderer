using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;

namespace Orthographic.Renderer.Managers;

public class FileManager
{
    public static dynamic ReadJsonKeyValue(string path)
    {
        var text = File.ReadAllText(path);
        return JsonConvert.DeserializeObject(text) ?? string.Empty;
    }

    public static List<string> ReadJsonArray(string path, string key)
    {
        var text = File.ReadAllText(path);
        var json = JsonConvert.DeserializeObject<Dictionary<string, object>>(text);
        if (json != null && json.TryGetValue(key, out var value) && value is Newtonsoft.Json.Linq.JArray jArray)
        {
            return jArray.ToObject<List<string>>() ?? [];
        }
        return [];
    }

    public static void WriteKeyValueToJsonFile(string path, string key, string value)
    {
        var json = ReadJsonKeyValue(path);
        if (json == null)
        {
            return;
        }

        json[key] = value;
        File.WriteAllText(path, JsonConvert.SerializeObject(json, Formatting.Indented));
    }

    public static void WriteArrayToJsonFile(string path, string key, List<string?> value)
    {
        var json = new Dictionary<string, object> { { key, value } };
        var jsonString = JsonConvert.SerializeObject(json, Formatting.Indented);
        File.WriteAllText(path, jsonString);
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

    public static bool VerifyDirectoryPath(string path)
    {
        return Directory.Exists(path);
    }

    public static bool VerifyProgramPath(string key, string path)
    {
        // Ensure that the file exists
        if (!File.Exists(path))
        {
            return false;
        }

        // Ensure that the key matches the file
        // for example if the key is "blender" then the path should be "blender" in its last segment
        var lastSegment = Path.GetFileNameWithoutExtension(path);
        if (!lastSegment.Contains(key, System.StringComparison.CurrentCultureIgnoreCase))
        {
            return false;
        }

        // Check that the --version argument of the executable is able to be run
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = path,
                Arguments = "--version",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            },
        };

        process.Start();
        var output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();

        return !string.IsNullOrEmpty(output);
    }
}
