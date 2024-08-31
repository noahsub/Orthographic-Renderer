////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// ProcessManager.cs
// This file contains the logic for managing process operations such as running processes and checking their exit codes.
//
// Copyright (C) 2024 noahsub
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// IMPORTS
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System.Diagnostics;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// NAMESPACE
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Orthographic.Renderer.Managers;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// PROCESS MANAGER CLASS
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

/// <summary>
/// Manages process operations such as running processes and checking their exit codes.
/// </summary>
public static class ProcessManager
{
    /// <summary>
    /// Runs a process with the specified path and arguments, and returns the output.
    /// </summary>
    /// <param name="path">The path to the executable.</param>
    /// <param name="arguments">The arguments to pass to the executable.</param>
    /// <returns>The output of the process.</returns>
    public static string RunProcess(string path, string arguments)
    {
        // The process to run and its arguments
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = path,
                Arguments = arguments,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            },
        };

        // Start the process
        process.Start();
        // Read the output
        var output = process.StandardOutput.ReadToEnd();
        // Wait for the process to exit
        process.WaitForExit();

        return output;
    }

    /// <summary>
    /// Runs a process with the specified path and arguments, and checks if it exits successfully.
    /// </summary>
    /// <param name="path">The path to the executable.</param>
    /// <param name="arguments">The arguments to pass to the executable.</param>
    /// <returns>True if the process exits with a code of 0, otherwise false.</returns>
    public static bool RunProcessCheck(string path, string arguments)
    {
        // The process to run and its arguments
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = path,
                Arguments = arguments,
                RedirectStandardOutput = false,
                UseShellExecute = false,
                CreateNoWindow = true,
            },
        };

        // Start the process
        process.Start();
        // Wait for the process to exit
        process.WaitForExit();

        // Return true if the exit code is 0
        return process.ExitCode == 0;
    }
}
