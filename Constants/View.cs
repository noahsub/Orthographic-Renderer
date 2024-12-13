﻿////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// View.cs
// This file contains the constants for different views.
//
// Copyright (C) 2024 noahsub
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// IMPORTS
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System.Collections.Generic;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// NAMESPACE
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Orthographic.Renderer.Constants;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// VIEW CLASS
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

/// <summary>
/// Stores view constants.
/// </summary>
public static class View
{
    /// <summary>
    /// A list of all the views that can be rendered.
    /// </summary>
    public static readonly List<string> RenderViews =
    [
        "top-front-right",
        "top-right-back",
        "top-back-left",
        "top-left-front",
        "front-right-bottom",
        "right-back-bottom",
        "back-left-bottom",
        "left-front-bottom",
        "top-front",
        "top-right",
        "top-back",
        "top-left",
        "front-bottom",
        "right-bottom",
        "back-bottom",
        "left-bottom",
        "front-right",
        "right-back",
        "back-left",
        "left-front",
        "front",
        "right",
        "back",
        "left",
        "top",
        "bottom",
    ];
}
