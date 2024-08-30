////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// NavigationManager.cs
// This file contains the logic for managing navigation operations within the application.
//
// Author(s): https://github.com/noahsub
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// IMPORTS
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

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
    /// A reference to the render page.
    /// </summary>
    private static RenderPage? _renderPage = null;

    /// <summary>
    /// A reference to the requirements page.
    /// </summary>
    private static RequirementsPage? _requirementsPage = null;

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
            case "RenderPage":
                if (_renderPage == null)
                {
                    _renderPage = new RenderPage();
                }
                _renderPage.SetFileName();
                pageContent.Content = _renderPage;
                _currentPage = _renderPage;
                break;
            case "RequirementsPage":
                if (_requirementsPage == null)
                {
                    _requirementsPage = new RequirementsPage();
                }
                pageContent.Content = _requirementsPage;
                _currentPage = _requirementsPage;
                break;
            default:
                return;
        }
    }
}
