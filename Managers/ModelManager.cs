﻿////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// ModelManager.cs
// This file manages 3D model operations.
//
// Copyright (C) 2024 noahsub
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
    // TYPES
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// The valid types of 3D models.
    /// </summary>
    public static readonly List<string> ValidTypes =
    [
        ".blend",
        ".obj",
        ".stl",
        ".BLEND",
        ".OBJ",
        ".STL",
    ];

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // MEASUREMENTS
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Gets the dimensions of a 3D model.
    /// </summary>
    /// <param name="path">The path to the 3D model.</param>
    /// <returns>A vector representing the x, y, and z dimensions of the object.</returns>
    /// <exception cref="ArgumentException">Thrown when the file type is invalid.</exception>
    public static Vector3 GetDimensions(string path)
    {
        // Get the extension of the file and convert it to lowercase
        var extension = Path.GetExtension(path).ToLower();

        // Check the extension and return the appropriate dimensions
        return extension switch
        {
            // If the extension is .blend, return a vector of 0, 0, 0
            ".blend" => new Vector3(0, 0, 0),
            ".obj" => GetObjDimensions(path),
            ".stl" => GetStlDimensions(path),
            _ => throw new ArgumentException("Invalid file type."),
        };
    }

    /// <summary>
    /// Gets the dimensions of a .obj file.
    /// </summary>
    /// <param name="path">The path to the .obj file.</param>
    /// <returns>A vector representing the x, y, and z dimensions of the object.</returns>
    public static Vector3 GetObjDimensions(string path)
    {
        // List to store the vertices
        var vertices = new List<Vector3>();

        // Get all the vertices by reading the file line by line
        using (var reader = new StreamReader(path))
        {
            string? line;
            while ((line = reader.ReadLine()) != null)
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
        using (var reader = new StreamReader(path))
        {
            var firstLine = reader.ReadLine();
            if (firstLine != null && firstLine.Contains("solid"))
            {
                return GetStlAsciiDimensions(path);
            }
        }
        return GetStlBinaryDimensions(path);
    }

    /// <summary>
    /// Get the dimensions of a .stl file in ASCII format.
    /// </summary>
    /// <param name="path">The path to the .stl file.</param>
    /// <returns>A vector representing the x, y, and z dimensions of the object.</returns>
    private static Vector3 GetStlAsciiDimensions(string path)
    {
        var vertices = new List<Vector3>();

        using (var reader = new StreamReader(path))
        {
            string? line;
            while ((line = reader.ReadLine()) != null)
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
        }

        var width = vertices.Max(v => v.X) - vertices.Min(v => v.X);
        var height = vertices.Max(v => v.Y) - vertices.Min(v => v.Y);
        var depth = vertices.Max(v => v.Z) - vertices.Min(v => v.Z);

        return new Vector3(width, height, depth);
    }

    /// <summary>
    /// Get the dimensions of a .stl file in binary format.
    /// </summary>
    /// <param name="path">The path to the .stl file.</param>
    /// <returns>A vector representing the x, y, and z dimensions of the object.</returns>
    private static Vector3 GetStlBinaryDimensions(string path)
    {
        var vertices = new List<Vector3>();

        using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
        using (var reader = new BinaryReader(stream))
        {
            // Skip the header (80 bytes) and the number of triangles (4 bytes)
            reader.BaseStream.Seek(84, SeekOrigin.Begin);

            while (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                // Skip the normal vector (3 floats, 12 bytes)
                reader.BaseStream.Seek(12, SeekOrigin.Current);

                for (int i = 0; i < 3; i++)
                {
                    float x = reader.ReadSingle();
                    float y = reader.ReadSingle();
                    float z = reader.ReadSingle();

                    vertices.Add(new Vector3(x, y, z));
                }

                // Skip the attribute byte count (2 bytes)
                reader.BaseStream.Seek(2, SeekOrigin.Current);
            }
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
