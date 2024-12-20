﻿////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// DialogManager.cs
// This file is responsible for managing dialogs.
//
// Copyright (C) 2024 noahsub
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// NAMESPACE
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Orthographic.Renderer.Managers;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// DIALOG MANAGER CLASS
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

/// <summary>
/// Manages dialogs.
/// </summary>
public static class DialogManager
{
    /// <summary>
    /// Creates a dialog to show the user that the path requires elevated permissions.
    /// </summary>
    public static void ShowElevatedPermissionsWarningDialog()
    {
        // Create a new warning dialog.
        var warning = new Windows.Warning();
        // Set the warning message.
        warning.SetWarning(
            "This path requires elevated permissions. Please run the program as an administrator or select a different path."
        );
        // Show the warning dialog.
        warning.Show();
    }
}
