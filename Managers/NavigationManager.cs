////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// NavigationManager.cs
// This file contains the logic for managing navigation operations within the application.
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

/// <summary>
/// Manages navigation operations within the application.
/// </summary>
public static class NavigationManager
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // PAGES
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// A reference to the model page.
    /// </summary>
    private static ModelPage? _modelPage = null;

    /// <summary>
    /// A reference to the views page.
    /// </summary>
    private static ViewsPage? _viewsPage = null;

    /// <summary>
    /// A reference to the render page.
    /// </summary>
    private static RenderPage? _renderPage = null;

    /// <summary>
    /// A reference to the requirements page.
    /// </summary>
    private static RequirementsPage? _requirementsPage = null;

    /// <summary>
    /// A reference to the update page.
    /// </summary>
    private static UpdatePage? _updatePage = null;

    /// <summary>
    /// A reference to the lighting page.
    /// </summary>
    private static LightingPage? _lightingPage = null;

    /// <summary>
    /// A reference to the hardware page.
    /// </summary>
    private static HardwarePage? _hardwarePage = null;

    /// <summary>
    /// The current page displayed in the main window.
    /// </summary>
    private static UserControl? _currentPage = null;

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // NAVIGATION
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public static UserControl? GetCurrentPage()
    {
        return _currentPage;
    }

    public static Dictionary<string, bool> LoadedPages =
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

    /// <summary>
    /// Switches the current page displayed in the main window.
    /// </summary>
    /// <param name="mainWindow">The main window of the application.</param>
    /// <param name="page">The new page to be displayed.</param>
    public static void SwitchPage(MainWindow mainWindow, string page)
    {
        var pageContent = mainWindow.FindControl<ContentControl>("PageContent");

        if (pageContent == null)
        {
            return;
        }

        switch (page)
        {
            case "ModelPage":
                if (_modelPage == null)
                {
                    _modelPage = new ModelPage();
                }

                if (!LoadedPages["ModelPage"])
                {
                    LoadedPages["ModelPage"] = true;
                    _modelPage.OnFirstLoad();
                }
                pageContent.Content = _modelPage;
                _currentPage = _modelPage;
                _modelPage.OnNavigatedTo();
                break;
            case "ViewsPage":
                if (_viewsPage == null)
                {
                    _viewsPage = new ViewsPage();
                }

                if (!LoadedPages["ViewsPage"])
                {
                    LoadedPages["ViewsPage"] = true;
                    _viewsPage.OnFirstLoad();
                }

                pageContent.Content = _viewsPage;
                _currentPage = _viewsPage;
                _viewsPage.OnNavigatedTo();
                break;
            case "RenderPage":
                if (_renderPage == null)
                {
                    _renderPage = new RenderPage();
                }

                if (!LoadedPages["RenderPage"])
                {
                    LoadedPages["RenderPage"] = true;
                    _renderPage.OnFirstLoad();
                }

                pageContent.Content = _renderPage;
                _currentPage = _renderPage;
                _renderPage.OnNavigatedTo();
                break;
            case "RequirementsPage":
                if (_requirementsPage == null)
                {
                    _requirementsPage = new RequirementsPage();
                }

                if (!LoadedPages["RequirementsPage"])
                {
                    LoadedPages["RequirementsPage"] = true;
                    _requirementsPage.OnFirstLoad();
                }

                pageContent.Content = _requirementsPage;
                _currentPage = _requirementsPage;
                _requirementsPage.OnNavigatedTo();
                break;
            case "UpdatePage":
                if (_updatePage == null)
                {
                    _updatePage = new UpdatePage();
                }

                if (!LoadedPages["UpdatePage"])
                {
                    LoadedPages["UpdatePage"] = true;
                }

                pageContent.Content = _updatePage;
                _currentPage = _updatePage;
                break;
            case "LightingPage":
                if (_lightingPage == null)
                {
                    _lightingPage = new LightingPage();
                }

                if (!LoadedPages["LightingPage"])
                {
                    LoadedPages["LightingPage"] = true;
                    _lightingPage.OnFirstLoad();
                }

                pageContent.Content = _lightingPage;
                _currentPage = _lightingPage;
                _lightingPage.OnNavigatedTo();
                break;
            case "HardwarePage":
                if (_hardwarePage == null)
                {
                    _hardwarePage = new HardwarePage();
                }

                if (!LoadedPages["HardwarePage"])
                {
                    LoadedPages["HardwarePage"] = true;
                    _hardwarePage.OnFirstLoad();
                }

                pageContent.Content = _hardwarePage;
                _currentPage = _hardwarePage;
                _hardwarePage.OnNavigatedTo();
                break;
            default:
                return;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // PAGE CREATION
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Create a new page of the specified type.
    /// </summary>
    /// <param name="name"></param>
    public static async Task CreatePage(string name)
    {
        switch (name)
        {
            case "ModelPage":
                _modelPage = new ModelPage();
                break;
            case "ViewsPage":
                _viewsPage = new ViewsPage();
                break;
            case "RenderPage":
                _renderPage = new RenderPage();
                break;
            case "RequirementsPage":
                _requirementsPage = new RequirementsPage();
                break;
            case "UpdatePage":
                _updatePage = new UpdatePage();
                break;
            case "LightingPage":
                _lightingPage = new LightingPage();
                break;
            case "HardwarePage":
                _hardwarePage = new HardwarePage();
                break;
            default:
                return;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // GETTERS
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public static Control? GetPage(string name)
    {
        switch (name)
        {
            case "ModelPage":
                return _modelPage;
            case "ViewsPage":
                return _viewsPage;
            case "RenderPage":
                return _renderPage;
            case "RequirementsPage":
                return _requirementsPage;
            case "UpdatePage":
                return _updatePage;
            case "LightingPage":
                return _lightingPage;
            case "HardwarePage":
                return _hardwarePage;
            default:
                return new Control();
        }
    }
}
