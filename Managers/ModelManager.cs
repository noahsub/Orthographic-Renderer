////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// ModelManager.cs
// This file manages 3D model operations.
//
// Author(s): https://github.com/noahsub
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// IMPORTS
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// NAMESPACE
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Orthographic.Renderer.Managers;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// MODEL MANAGER
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

/// <summary>
/// Manages 3D model operations.
/// </summary>
public class ModelManager
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // MEASUREMENTS
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Gets the dimensions of a .obj file.
    /// </summary>
    /// <param name="path">The path to the .obj file.</param>
    /// <returns>A vector representing the x, y, and z dimensions of the object.</returns>
    public static Vector3 GetObjDimensions(string path)
    {
        // Read all lines in the file
        var lines = File.ReadAllLines(path);

        // Get all the vertices
        var vertices = new List<Vector3>();
        foreach (var line in lines)
        {
            if (line.StartsWith("v "))
            {
                var values = line.Split(' ');
                var x = float.Parse(values[1]);
                var y = float.Parse(values[2]);
                var z = float.Parse(values[3]);

                vertices.Add(new Vector3(x, y, z));
            }
        }

        // Get the dimensions
        var width = vertices.Max(v => v.X) - vertices.Min(v => v.X);
        var height = vertices.Max(v => v.Y) - vertices.Min(v => v.Y);
        var depth = vertices.Max(v => v.Z) - vertices.Min(v => v.Z);

        // Return the dimensions
        return new Vector3(width, height, depth);
    }

    /// <summary>
    /// Get the dimensions of a .stl file.
    /// </summary>
    /// <param name="path">The path to the .stl file</param>
    /// <returns>A vector representing the x, y, and z dimensions of the object.</returns>
    public static Vector3 GetStlDimensions(string path)
    {
        var text = File.ReadAllText(path);
        if (text.Contains("solid") && text.Contains("endsolid"))
        {
            return GetStlAsciiDimensions(path);
        }
        else
        {
            return GetStlBinaryDimensions(path);
        }
    }

    /// <summary>
    /// Get the dimensions of a .stl file in ASCII format.
    /// </summary>
    /// <param name="path">The path to the .stl file.</param>
    /// <returns>A vector representing the x, y, and z dimensions of the object.</returns>
    private static Vector3 GetStlAsciiDimensions(string path)
    {
        // Read all lines in the file
        var lines = File.ReadAllLines(path);

        // Get all the vertices
        var vertices = new List<Vector3>();

        foreach (var line in lines)
        {
            var formattedLine = RemoveWhitespace(line);
            if (formattedLine.StartsWith("vertex"))
            {
                var values = formattedLine.Split(' ');
                var x = float.Parse(values[1]);
                var y = float.Parse(values[2]);
                var z = float.Parse(values[3]);

                vertices.Add(new Vector3(x, y, z));
            }
        }

        // Get the dimensions
        var width = vertices.Max(v => v.X) - vertices.Min(v => v.X);
        var height = vertices.Max(v => v.Y) - vertices.Min(v => v.Y);
        var depth = vertices.Max(v => v.Z) - vertices.Min(v => v.Z);

        // Return the dimensions
        return new Vector3(width, height, depth);
    }

    /// <summary>
    /// Get the dimensions of a .stl file in binary format.
    /// </summary>
    /// <param name="path">The path to the .stl file.</param>
    /// <returns>A vector representing the x, y, and z dimensions of the object.</returns>
    private static Vector3 GetStlBinaryDimensions(string path)
    {
        // Read the file as bytes
        var data = File.ReadAllBytes(path);

        var vertices = new List<Vector3>();
        int offset = 84; // Skip the header (80 bytes) and the number of triangles (4 bytes)

        while (offset < data.Length)
        {
            offset += 12; // Skip the normal vector (3 floats, 12 bytes)

            for (int i = 0; i < 3; i++)
            {
                float x = BitConverter.ToSingle(data, offset);
                offset += 4;
                float y = BitConverter.ToSingle(data, offset);
                offset += 4;
                float z = BitConverter.ToSingle(data, offset);
                offset += 4;

                vertices.Add(new Vector3(x, y, z));
            }

            offset += 2; // Skip the attribute byte count (2 bytes)
        }

        // Get the dimensions
        var width = vertices.Max(v => v.X) - vertices.Min(v => v.X);
        var height = vertices.Max(v => v.Y) - vertices.Min(v => v.Y);
        var depth = vertices.Max(v => v.Z) - vertices.Min(v => v.Z);

        // Return the dimensions
        return new Vector3(width, height, depth);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // HELPERS
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Removes leading and trailing whitespace from a string as well as replaces spaces longer than one with a
    /// single space.
    /// </summary>
    /// <param name="input">The string to format</param>
    /// <returns></returns>
    private static string RemoveWhitespace(string input)
    {
        return string.Join(" ", input.Split(' ', StringSplitOptions.RemoveEmptyEntries));
    }
}
