﻿////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// OrientationNode.cs
// A class that represents a node in a graph of orientation nodes.
//
// Copyright (C) 2024 noahsub
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// NAMESPACE
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Orthographic.Renderer.Entities;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// ORIENTATION NODE CLASS
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


/// <summary>
/// Represents a node in a graph of orientation nodes.
/// </summary>
public class OrientationNode
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // PROPERTIES
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// The name of the node, i.e, the view it represents.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The icon of the node.
    /// </summary>
    public string Icon { get; set; }

    /// <summary>
    /// The node to the left of this node.
    /// </summary>
    public OrientationNode Left { get; set; } = null!;

    /// <summary>
    /// The node above this node.
    /// </summary>
    public OrientationNode Up { get; set; } = null!;

    /// <summary>
    /// The node to the right of this node.
    /// </summary>
    public OrientationNode Right { get; set; } = null!;

    /// <summary>
    /// The node below this node.
    /// </summary>
    public OrientationNode Down { get; set; } = null!;

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // INITIALIZATION
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Initializes a new instance of the <see cref="OrientationNode"/> class.
    /// </summary>
    /// <param name="name">The name of the node, i.e, the view it represents.</param>
    /// <param name="icon">The icon of the node.</param>
    public OrientationNode(string name, string icon)
    {
        Name = name;
        Icon = icon;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // SETTERS
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Adds a node to the left of this node.
    /// </summary>
    /// <param name="node"></param>
    public void AddLeft(OrientationNode node)
    {
        Left = node;
    }

    /// <summary>
    /// Adds a node above this node.
    /// </summary>
    /// <param name="node"></param>
    public void AddUp(OrientationNode node)
    {
        Up = node;
    }

    /// <summary>
    /// Adds a node to the right of this node.
    /// </summary>
    /// <param name="node"></param>
    public void AddRight(OrientationNode node)
    {
        Right = node;
    }

    /// <summary>
    /// Adds a node below this node.
    /// </summary>
    /// <param name="node"></param>
    public void AddDown(OrientationNode node)
    {
        Down = node;
    }
}
