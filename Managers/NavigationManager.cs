////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// NavigationManager.cs
// This file contains the logic for managing navigation operations within the application
//
// Copyright (C) 2024 noahsub
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// IMPORTS
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Controls;
using Orthographic.Renderer.Pages;
using Orthographic.Renderer.Windows;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// NAMESPACE
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Orthographic.Renderer.Managers;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// NAVIGATION MANAGER CLASS
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

public static class NavigationManager
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // PAGES
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// The pages that can be navigated to.
    /// </summary>
    private static readonly Dictionary<string, UserControl?> Pages =
        new()
        {
            { "ModelPage", null },
            { "ViewsPage", null },
            { "RenderPage", null },
            { "RequirementsPage", null },
            { "UpdatePage", null },
            { "LightingPage", null },
            { "HardwarePage", null },
        };

    /// <summary>
    /// The current page.
    /// </summary>
    private static UserControl? _currentPage;

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // LOADING
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Contains a dictionary of pages and whether they have been loaded.
    /// </summary>
    private static readonly Dictionary<string, bool> LoadedPages =
        new()
        {
            { "ModelPage", false },
            { "ViewsPage", false },
            { "RenderPage", false },
            { "RequirementsPage", false },
            { "UpdatePage", false },
            { "LightingPage", false },
            { "HardwarePage", false },
        };

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // NAVIGATION
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Switches the page to the specified page.
    /// </summary>
    /// <param name="mainWindow">The main window.</param>
    /// <param name="page">The page to switch to.</param>
    public static void SwitchPage(MainWindow mainWindow, string page)
    {
        var pageContent = mainWindow.FindControl<ContentControl>("PageContent");
        if (pageContent == null)
            return;

        if (!Pages.ContainsKey(page))
            return;

        if (Pages[page] == null)
        {
            Pages[page] = CreatePageInstance(page);
        }

        if (!LoadedPages[page])
        {
            LoadedPages[page] = true;
            (Pages[page] as dynamic)?.OnFirstLoad();
        }

        pageContent.Content = Pages[page];
        _currentPage = Pages[page];
        (Pages[page] as dynamic)?.OnNavigatedTo();
    }

    /// <summary>
    /// Helper for creating an instance of the specified page.
    /// </summary>
    /// <param name="pageName"></param>
    /// <returns></returns>
    private static UserControl? CreatePageInstance(string pageName) =>
        pageName switch
        {
            "ModelPage" => new ModelPage(),
            "ViewsPage" => new ViewsPage(),
            "RenderPage" => new RenderPage(),
            "RequirementsPage" => new RequirementsPage(),
            "UpdatePage" => new UpdatePage(),
            "LightingPage" => new LightingPage(),
            "HardwarePage" => new HardwarePage(),
            _ => null,
        };

    /// <summary>
    /// Creates an instance of the specified page.
    /// </summary>
    /// <param name="name"></param>
    public static async Task CreatePage(string name)
    {
        Pages[name] = CreatePageInstance(name);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // GETTERS
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Gets the current page.
    /// </summary>
    /// <returns></returns>
    public static UserControl? GetCurrentPage() => _currentPage;

    /// <summary>
    /// Gets the specified page.
    /// </summary>
    /// <param name="name">The name of the page to get.</param>
    /// <returns></returns>
    public static Control? GetPage(string name) =>
        Pages.ContainsKey(name) ? Pages[name] : new Control();
}
