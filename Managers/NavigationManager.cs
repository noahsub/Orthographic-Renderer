////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// NavigationManager.cs
// This file contains the logic for managing navigation operations within the application.
//
// Copyright (C) 2024 noahsub
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// IMPORTS
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

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
                _modelPage.SetDimensionsUnknown();
                pageContent.Content = _modelPage;
                _currentPage = _modelPage;
                break;
            case "ViewsPage":
                if (_viewsPage == null)
                {
                    _viewsPage = new ViewsPage();
                }
                pageContent.Content = _viewsPage;
                _currentPage = _viewsPage;
                _viewsPage.SetFileName();
                _viewsPage.Load();
                break;
            case "RenderPage":
                if (_renderPage == null)
                {
                    _renderPage = new RenderPage();
                }
                pageContent.Content = _renderPage;
                _currentPage = _renderPage;
                _renderPage.Load();
                break;
            case "RequirementsPage":
                if (_requirementsPage == null)
                {
                    _requirementsPage = new RequirementsPage();
                }
                pageContent.Content = _requirementsPage;
                _currentPage = _requirementsPage;
                _requirementsPage.Load();
                break;
            case "UpdatePage":
                if (_updatePage == null)
                {
                    _updatePage = new UpdatePage();
                }
                pageContent.Content = _updatePage;
                _currentPage = _updatePage;
                break;
            case "LightingPage":
                if (_lightingPage == null)
                {
                    _lightingPage = new LightingPage();
                }
                pageContent.Content = _lightingPage;
                _currentPage = _lightingPage;
                _lightingPage.Load();
                break;
            default:
                return;
        }
    }

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
            default:
                return;
        }
    }
}
