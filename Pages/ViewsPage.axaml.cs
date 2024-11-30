////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// RenderPage.axaml.cs
// This file contains the logic for the RenderPage.
//
// Copyright (C) 2024 noahsub
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// IMPORTS
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Orthographic.Renderer.Constants;
using Orthographic.Renderer.Controls;
using Orthographic.Renderer.Managers;
using Orthographic.Renderer.Windows;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// NAMESPACE
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Orthographic.Renderer.Pages;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// RENDER PAGE CLASS
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
public partial class ViewsPage : UserControl
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // INITIALIZATION
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Initializes a new instance of the <see cref="ViewsPage"/> class.
    /// </summary>
    public ViewsPage()
    {
        InitializeComponent();

        // Set the layout of the ViewStackGrid.
        ViewStackGrid.SetLayout(5);

        // Populate the views.
        PopulateViews(RenderManager.RenderViews);
    }

    public void Load()
    {
        // Set the file label to the name of the model file.
        FileLabel.Content = Path.GetFileName(DataManager.ModelPath);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // SELECT AND SORT
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Selects the views based on the selection options.
    /// </summary>
    private void SelectViews()
    {
        // Select the views based on the selection options.
        foreach (var control in ViewStackGrid.GetItems())
        {
            var view = (ViewSelection)control;

            // Select all views.
            if (ViewSelectOptions.AllButton.IsChecked == true)
            {
                view.SetSelected(true);
            }
            // Deselect all views.
            else if (ViewSelectOptions.NoneButton.IsChecked == true)
            {
                view.SetSelected(false);
            }
            // Invert the selection of all views.
            else if (ViewSelectOptions.InvertButton.IsChecked == true)
            {
                view.SetSelected(!view.GetSelected());
            }
        }

        // Clear the selection options.
        ViewSelectOptions.ClearSelection();
    }

    /// <summary>
    /// Sorts the views based on the selected faces.
    /// </summary>
    private void SortViews()
    {
        var selectedViews = GetSelectedViews();
        ViewStackGrid.ClearItems();
        var selectedFaces = ViewSortOptions.GetSelectedFaces();
        var sortedViews = RenderManager.SortViews(selectedFaces);
        PopulateViews(sortedViews);
        foreach (var control in ViewStackGrid.GetItems())
        {
            var viewSelection = (ViewSelection)control;
            if (selectedViews.Contains(viewSelection.Key))
            {
                viewSelection.SetSelected(true);
            }
        }
    }

    /// <summary>
    /// Gets the selected views.
    /// </summary>
    /// <returns>A list of selected views.</returns>
    private List<string> GetSelectedViews()
    {
        List<string> selectedViews = [];

        foreach (var control in ViewStackGrid.GetItems())
        {
            var viewSelection = (ViewSelection)control;
            if (viewSelection.GetSelected())
            {
                selectedViews.Add(viewSelection.Key);
            }
        }

        return selectedViews;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // VIEWS
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Populates the views in the ViewStackGrid.
    /// </summary>
    /// <param name="views">The list of views to populate.</param>
    private void PopulateViews(List<string> views)
    {
        // foreach view in the list of views
        foreach (var view in views)
        {
            // create a new ViewSelection control
            var viewSelection = new ViewSelection();
            viewSelection.SetName(view);
            viewSelection.SetImage(view);

            // add the view to the ViewStackGrid
            ViewStackGrid.AddItem(viewSelection);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // RENDERING
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // SETTERS
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public void SetFileName()
    {
        FileLabel.Content = Path.GetFileName(DataManager.ModelPath);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // EVENTS
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Handles the tap event of the ViewSelect button.
    /// </summary>
    private void ViewSelect_OnTapped(object? sender, TappedEventArgs e)
    {
        SelectViews();
    }

    /// <summary>
    /// Handles the tap event of the ViewSort button.
    /// </summary>
    private void ViewSort_OnTapped(object? sender, TappedEventArgs e)
    {
        SortViews();
    }

    /// <summary>
    /// Handles the click event of the Back button.
    /// </summary>
    private void BackButton_OnClick(object? sender, RoutedEventArgs e)
    {
        // Switch to the ModelPage
        var mainWindow = (MainWindow)this.VisualRoot!;
        NavigationManager.SwitchPage(mainWindow, "LightingPage");
    }

    private void NextButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var selectedViews = GetSelectedViews();
        if (selectedViews.Count == 0)
        {
            foreach (var control in ViewStackGrid.GetItems())
            {
                var view = (ViewSelection)control;
                if (view.Key == "top-front-right")
                {
                    view.SetSelected(true);
                    selectedViews.Add(view.Key);
                    break;
                }
            }
        }

        DataManager.SelectedViews = selectedViews;

        // Switch to the ModelPage
        var mainWindow = (MainWindow)this.VisualRoot!;
        NavigationManager.SwitchPage(mainWindow, "RenderPage");
    }
}
