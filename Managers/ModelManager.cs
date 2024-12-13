////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
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
using Orthographic.Renderer.Constants;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// NAMESPACE
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Orthographic.Renderer.Managers
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // MODEL MANAGER CLASS
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Manages 3D model operations.
    /// </summary>
    public static class ModelManager
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // VALID MODEL TYPES
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

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

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // MODEL DIMENSIONS
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets the dimensions of a 3D model.
        /// </summary>
        /// <param name="path">The path to the 3D model.</param>
        /// <returns>A vector representing the x, y, and z dimensions of the object.</returns>
        /// <exception cref="ArgumentException">Thrown when the file type is invalid.</exception>
        public static Vector3 GetDimensions(string path)
        {
            var extension = Path.GetExtension(path).ToLower();

            return extension switch
            {
                ".blend" => new Vector3(0, 0, 0),
                ".obj" => GetObjDimensions(path),
                ".stl" => GetStlDimensions(path),
                _ => throw new ArgumentException("Invalid file type."),
            };
        }

        /// <summary>
        /// Gets the maximum dimension of a vector.
        /// </summary>
        /// <param name="dimensions"></param>
        /// <returns></returns>
        public static float GetMaxDimension(Vector3 dimensions)
        {
            return Math.Max(dimensions.X, Math.Max(dimensions.Y, dimensions.Z));
        }

        /// <summary>
        /// Gets the dimensions of a .obj file.
        /// </summary>
        /// <param name="path">The path to the .obj file.</param>
        /// <returns>A vector representing the x, y, and z dimensions of the object.</returns>
        private static Vector3 GetObjDimensions(string path)
        {
            var vertices = new List<Vector3>();

            using (var reader = new StreamReader(path))
            {
                while (reader.ReadLine() is { } line)
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

            var width = vertices.Max(v => v.X) - vertices.Min(v => v.X);
            var height = vertices.Max(v => v.Y) - vertices.Min(v => v.Y);
            var depth = vertices.Max(v => v.Z) - vertices.Min(v => v.Z);

            return new Vector3(width, height, depth);
        }

        /// <summary>
        /// Get the dimensions of a .stl file.
        /// </summary>
        /// <param name="path">The path to the .stl file</param>
        /// <returns>A vector representing the x, y, and z dimensions of the object.</returns>
        private static Vector3 GetStlDimensions(string path)
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
                while (reader.ReadLine() is { } line)
                {
                    var formattedLine = TextManager.RemoveWhitespace(line);
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
                reader.BaseStream.Seek(84, SeekOrigin.Begin);

                while (reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    reader.BaseStream.Seek(12, SeekOrigin.Current);

                    for (int i = 0; i < 3; i++)
                    {
                        float x = reader.ReadSingle();
                        float y = reader.ReadSingle();
                        float z = reader.ReadSingle();

                        vertices.Add(new Vector3(x, y, z));
                    }

                    reader.BaseStream.Seek(2, SeekOrigin.Current);
                }
            }

            var width = vertices.Max(v => v.X) - vertices.Min(v => v.X);
            var height = vertices.Max(v => v.Y) - vertices.Min(v => v.Y);
            var depth = vertices.Max(v => v.Z) - vertices.Min(v => v.Z);

            return new Vector3(width, height, depth);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // UNIT SCALE
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Sets the unit scale for the model.
        /// </summary>
        /// <param name="unit"></param>
        public static void SetModelUnit(string unit)
        {
            unit = unit.ToLower();
            DataManager.UnitScale = unit switch
            {
                "millimeters" => ModelUnit.Millimeter,
                "centimeters" => ModelUnit.Centimeter,
                "meters" => ModelUnit.Meter,
                "inches" => ModelUnit.Inch,
                "feet" => ModelUnit.Foot,
                _ => DataManager.UnitScale,
            };
        }
    }
}
