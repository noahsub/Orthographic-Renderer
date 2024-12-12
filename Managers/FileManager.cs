////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// FileManager.cs
// This file contains the logic for managing file operations such as reading and writing JSON files, and verifying paths.
//
// Copyright (C) 2024 noahsub
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// IMPORTS
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;
using OperatingSystem = LibreHardwareMonitor.Software.OperatingSystem;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// NAMESPACE
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Orthographic.Renderer.Managers
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // FILE MANAGER CLASS
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Manages file operations such as reading and writing JSON files, and verifying paths.
    /// </summary>
    public static class FileManager
    {
         ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // JSON OPERATIONS
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Reads a JSON file and returns its content as a dynamic object.
        /// </summary>
        /// <param name="path">The path to the JSON file.</param>
        /// <returns>The content of the JSON file as a dynamic object.</returns>
        public static dynamic ReadJsonKeyValue(string path)
        {
            // Get all the text from the file
            var text = File.ReadAllText(path);
            // Deserialize it, with a fallback to an empty string if it fails
            return JsonConvert.DeserializeObject(text) ?? string.Empty;
        }

        /// <summary>
        /// Reads a JSON file and returns the value of a specified key as a list of strings.
        /// </summary>
        /// <param name="path">The path to the JSON file.</param>
        /// <param name="key">The key whose value is to be read.</param>
        /// <returns>A list of strings representing the value of the specified key.</returns>
        public static List<string> ReadJsonArray(string path, string key)
        {
            // Get all the text from the file
            var text = File.ReadAllText(path);
            // Deserialize it into a dictionary
            var json = JsonConvert.DeserializeObject<Dictionary<string, object>>(text);
            // If the dictionary is not null and the key exists, return the value as a list of strings
            if (json != null && json.TryGetValue(key, out var value) && value is Newtonsoft.Json.Linq.JArray jArray)
            {
                return jArray.ToObject<List<string>>() ?? new List<string>();
            }
            // Otherwise, return an empty list
            return new List<string>();
        }

        /// <summary>
        /// Writes a key-value pair to a JSON file.
        /// </summary>
        /// <param name="path">The path to the JSON file.</param>
        /// <param name="key">The key to be written.</param>
        /// <param name="value">The value to be written.</param>
        public static void WriteKeyValueToJsonFile(string path, string key, string value)
        {
            // Read the JSON file
            var json = ReadJsonKeyValue(path);
            // If the JSON is null, return
            if (json == null)
            {
                return;
            }

            // Update the JSON with the new key-value pair
            json[key] = value;
            File.WriteAllText(path, JsonConvert.SerializeObject(json, Formatting.Indented));
        }

        /// <summary>
        /// Writes an array to a JSON file mapped to specified key.
        /// </summary>
        /// <param name="path">The path to the JSON file.</param>
        /// <param name="key">The key under which the array is to be mapped.</param>
        /// <param name="value">The array to be written.</param>
        public static void WriteArrayToJsonFile(string path, string key, List<string> value)
        {
            // Dictionary to store the key-value pair
            var json = new Dictionary<string, object> { { key, value } };
            // Serialize the dictionary to JSON
            var jsonString = JsonConvert.SerializeObject(json, Formatting.Indented);
            // Write the JSON to the file
            File.WriteAllText(path, jsonString);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // PATH OPERATIONS
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Reformats a file path by removing double quotes, converting backslashes to forward slashes, and replacing %20 with spaces.
        /// </summary>
        /// <param name="path">The path to be reformatted.</param>
        /// <returns>The reformatted path.</returns>
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

        /// <summary>
        /// Gets the absolute path of a given local path.
        /// </summary>
        /// <param name="localPath">The local path.</param>
        /// <returns>The absolute path.</returns>
        public static string GetAbsolutePath(string localPath)
        {
            return Path.GetFullPath(localPath);
        }

        /// <summary>
        /// Checks if the path requires elevated permissions to access.
        /// </summary>
        /// <returns>True, if the path requires elevated permissions, otherwise false.</returns>
        public static bool ElevatedPath(string path)
        {
            try
            {
                using (File.Open(path, FileMode.Open, FileAccess.Read))
                {
                    // If we can open the file, it does not require elevated permissions
                    return false;
                }
            }
            catch (UnauthorizedAccessException)
            {
                // If an UnauthorizedAccessException is caught, it requires elevated permissions
                return true;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // USER FILES
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///  Copies the user files from the data directory to the user directory.
        /// </summary>
        public static void CopyUserFiles()
        {
            var userDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                "Documents/Orthographic Renderer"
            );

            if (!Directory.Exists(userDirectory))
            {
                Directory.CreateDirectory(userDirectory);
            }

            // foreach file in the data directory, if it doesn't exist in the user directory or the user directory file is older, copy it
            var dataFiles = Directory.GetFiles("Data");
            foreach (var file in dataFiles)
            {
                var fileName = Path.GetFileName(file);
                var userFilePath = Path.Combine(userDirectory, fileName);
                if (!File.Exists(userFilePath) || File.GetLastWriteTime(file) > File.GetLastWriteTime(userFilePath))
                {
                    File.Copy(file, userFilePath, true);
                }
            }
        }

        /// <summary>
        /// Gets the path to the program paths file.
        /// </summary>
        /// <returns>The path to the program paths file.</returns>
        public static string GetProgramPathsFile()
        {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                "Documents/Orthographic Renderer/program_paths.json"
            );
        }

        /// <summary>
        /// Gets the path to the recent models file.
        /// </summary>
        /// <returns>The path to the recent models file.</returns>
        public static string GetRecentModelsFile()
        {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                "Documents/Orthographic Renderer/recent_models.json"
            );
        }

        /// <summary>
        /// Gets the list of paths of recent 3D models.
        /// </summary>
        /// <returns></returns>
        public static List<string> GetRecentModels()
        {
            // Get the path to the serialized recent models file
            var recentModelsFile = GetRecentModelsFile();
            // Read the 'paths' array from the file
            return ReadJsonArray(recentModelsFile, "paths");
        }

        public static string GetTempDirectoryPath()
        {
            return Path.GetTempPath().Replace("\\", "/");
        }
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // VERIFICATION
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    
        /// <summary>
        /// Verifies if a blender path is valid.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool VerifyBlenderPath(string path)
        {
            // Check if the path is the bundled blender windows executable
            if (OperatingSystem.IsWindows8OrGreater && path == "/Blender/Windows/blender.exe")
            {
                return true;
            }
            
            // Check if the path is the bundled blender Linux executable
            if (OperatingSystem.IsUnix && path == "/Blender/Linux/blender")
            {
                return true;
            }

            // Check if the custom path is valid
            return VerifyProgramPath("blender", path);
        }

        /// <summary>
        /// Checks if the path to the 3D model is valid.
        /// </summary>
        /// <param name="path">The path to verify.</param>
        /// <returns></returns>
        public static bool VerifyModelPath(string path)
        {
            // Assuming that the file already exists, check if the extension is valid
            var extension = Path.GetExtension(path).ToLower();
            return ModelManager.ValidTypes.Contains(extension);
        }
        
        /// <summary>
        /// Verifies if a directory path exists.
        /// </summary>
        /// <param name="path">The directory path to verify.</param>
        /// <returns>True if the directory exists, otherwise false.</returns>
        public static bool VerifyDirectoryPath(string path)
        {
            return Directory.Exists(path);
        }
        
        /// <summary>
        /// Verifies if a file path exists.
        /// </summary>
        /// <param name="path">The file path to verify.</param>
        /// <returns></returns>
        public static bool VerifyFilePath(string path)
        {
            return File.Exists(path);
        }

        /// <summary>
        /// Verifies if a program path is valid by checking if the file exists, if the key matches the file name, and if the executable can run with the --version argument.
        /// </summary>
        /// <param name="key">The key to match with the file name.</param>
        /// <param name="path">The program path to verify.</param>
        /// <returns>True if the program path is valid, otherwise false.</returns>
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
            if (!lastSegment.Contains(key, StringComparison.CurrentCultureIgnoreCase))
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

            // Start the process and get the output
            process.Start();
            var output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            // Return true if the output is not empty
            return !string.IsNullOrEmpty(output);
        }
    }
}