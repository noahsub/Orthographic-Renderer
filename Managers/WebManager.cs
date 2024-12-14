////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// WebManager.cs
// This file manages web operations such as opening URLs.
//
// Copyright (C) 2024 noahsub
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// IMPORTS
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// NAMESPACE
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Orthographic.Renderer.Managers;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// WEB MANAGER CLASS
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

/// <summary>
/// Manages web operations such as opening URLs.
/// </summary>
public static class WebManager
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // OPEN URL
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Opens the specified URL in the default web browser.
    /// </summary>
    /// <param name="url">The URL to open.</param>
    public static void OpenUrl(string url)
    {
        Process.Start(new ProcessStartInfo { FileName = url, UseShellExecute = true });
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // API CALLS
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Get the latest version of the application from the GitHub API.
    /// </summary>
    /// <returns>The latest version of the application.</returns>
    public static async Task<string> GetLatestVersion()
    {
        // Read the current version of the application from the VERSION file
        var currentVersion = File.ReadAllLines("VERSION")[0].Trim();
        var url = "https://api.github.com/repos/noahsub/Orthographic-Renderer/releases/latest";
        var client = new HttpClient
        {
            // Set a timeout of 5 seconds for the HttpClient
            Timeout = TimeSpan.FromSeconds(5),
        };
        client.DefaultRequestHeaders.UserAgent.TryParseAdd("request");

        try
        {
            // Make a GET request to the GitHub API to get the latest release
            var response = await client.GetAsync(url);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                return currentVersion;
            }

            // Get the json content of the response
            var content = await response.Content.ReadAsStringAsync();
            var json = JObject.Parse(content);

            // Get the tag name from the json content
            var latestVersion = json["tag_name"]?.ToString().Replace("v", "");
            return latestVersion ?? currentVersion;
        }
        
        catch (Exception ex)
        {
            // If the version cannot be retrieved, return the current version to skip the update screen
            return currentVersion;
        }
    }
}
