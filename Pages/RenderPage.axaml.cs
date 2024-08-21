////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// RenderPage.axaml.cs
// This file contains the logic for the RenderPage.axaml file. This file is responsible for displaying hardware
// monitoring, selecting render views, and starting blender render jobs.
//
// Author(s): https://github.com/noahsub
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// IMPORTS
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using LibreHardwareMonitor.Hardware;
using Orthographic.Renderer.Controls;
using Orthographic.Renderer.Managers;
using Hardware = Orthographic.Renderer.Entities.Hardware;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// RENDER PAGE CLASS
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Orthographic.Renderer.Pages;

/// <summary>
/// Handles displaying hardware monitoring, selecting render views, and starting blender render jobs.
/// </summary>
public partial class RenderPage : UserControl
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // GLOBAL VARIABLES
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // INITIALIZATION
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Initializes the RenderPage.
    /// </summary>
    public RenderPage()
    {
        InitializeComponent();

        

        // Populate the angles
        PopulateViews(RenderManager.RenderViews);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // MONITOR
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////











    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // VIEWS
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Determines the appropriate stack panel for a given index.
    /// </summary>
    /// <param name="index">The index to determine the stack panel for.</param>
    /// <returns>The stack panel corresponding to the index.</returns>
    /*
       ┌───┐ ┌───┐ ┌───┐ ┌───┐
       │   │ │   │ │   │ │   │
    ┌──► 0 ┼─► 1 ┼─► 2 ┼─► 3 ┼──┐
    │  │   │ │   │ │   │ │   │  │
    │  └───┘ └───┘ └───┘ └───┘  │
    │                           │
    └───────────────────────────┘
     */
    private StackPanel DetermineViewSelectionStackPanel(int index)
    {
        return index switch
        {
            0 => ViewSelectionStackPanel0,
            1 => ViewSelectionStackPanel1,
            2 => ViewSelectionStackPanel2,
            3 => ViewSelectionStackPanel3,
            _ => ViewSelectionStackPanel0,
        };
    }

    /// <summary>
    /// Populates the view selection stack panels with render view controls for different angles from left to right, top
    /// to bottom.
    /// </summary>
    /*
    ┌───┐ ┌───┐ ┌───┐ ┌───┐
    │ A │ │ B │ │ C │ │ D │
    │ E │ │ F │ │ G │ │ H │
    │ I │ │ J │ │ K │ │ . │
    └───┘ └───┘ └───┘ └───┘
     */
    private void PopulateViews(List<string> views)
    {
        var index = 0;
        foreach (var view in views)
        {
            // Create a new RenderViewControl
            var renderSelectionControl = new ViewSelection();

            // Set the image source
            var image = new Bitmap($"Assets/Images/RenderAngles/{view}.png");
            renderSelectionControl.Image.Source = image;

            var formattedName =
                // remove hyphens from angle name
                string.Join(
                    " ",
                    view.Replace("-", " ")
                        // capitalize the first letter of each word
                        .Split(' ')
                        .Select(word => char.ToUpper(word[0]) + word[1..].ToLower())
                );
            // set the name label
            renderSelectionControl.Name.Content = formattedName;

            // add the control to the stack panel in the correct column
            var viewSelectionStackPanel = DetermineViewSelectionStackPanel(index % 4);
            viewSelectionStackPanel.Children.Add(renderSelectionControl);
            index++;
        }
    }

    private void ViewSort_OnTapped(object? sender, TappedEventArgs e)
    {
        var allViewSelectionStackPanels = new List<StackPanel>();
        allViewSelectionStackPanels.Add(ViewSelectionStackPanel0);
        allViewSelectionStackPanels.Add(ViewSelectionStackPanel1);
        allViewSelectionStackPanels.Add(ViewSelectionStackPanel2);
        allViewSelectionStackPanels.Add(ViewSelectionStackPanel3);
        
        var selectedViews = new List<string>();
        foreach (var stackPanel in allViewSelectionStackPanels)
        {
            foreach (var item in stackPanel.Children)
            {
                var renderViewControl = (ViewSelection)item;
                if (renderViewControl.CheckBox.IsChecked == true)
                {
                    selectedViews.Add(renderViewControl.Name.Content.ToString());
                }
            }
        }
        
        ViewSelectionStackPanel0.Children.Clear();
        ViewSelectionStackPanel1.Children.Clear();
        ViewSelectionStackPanel2.Children.Clear();
        ViewSelectionStackPanel3.Children.Clear();
        
        var viewsToSortBy = new List<string>();
        foreach (var child in ViewSort.Faces.Children.OfType<ToggleButton>())
        {
            if (child.IsChecked == true)
            {
                viewsToSortBy.Add(child.Content.ToString().ToLower());
            }
        }

        var matchingViews = new List<string>();
        var nonMatchingViews = new List<string>();
        // find the views that contain all the selected views
        foreach (var view in RenderManager.RenderViews)
        {
            if (viewsToSortBy.All(view.Contains))
            {
                matchingViews.Add(view);
            }
            
            else
            {
                nonMatchingViews.Add(view);
            }
        }
        
        List<string> sortedViews = [];
        sortedViews.AddRange(matchingViews);
        sortedViews.AddRange(nonMatchingViews);
        
        PopulateViews(sortedViews);
        
        foreach (var stackPanel in allViewSelectionStackPanels)
        {
            foreach (var item in stackPanel.Children)
            {
                var renderViewControl = (ViewSelection)item;
                if (selectedViews.Contains(renderViewControl.Name.Content.ToString()))
                {
                    renderViewControl.CheckBox.IsChecked = true;
                }
            }
        }
    }

    private void ViewSelect_OnTapped(object? sender, TappedEventArgs e)
    {
        var allViewSelectionStackPanels = new List<StackPanel>();
        allViewSelectionStackPanels.Add(ViewSelectionStackPanel0);
        allViewSelectionStackPanels.Add(ViewSelectionStackPanel1);
        allViewSelectionStackPanels.Add(ViewSelectionStackPanel2);
        allViewSelectionStackPanels.Add(ViewSelectionStackPanel3);
        
        if (ViewSelect.AllButton.IsChecked == true)
        {
            foreach (var stackPanel in allViewSelectionStackPanels)
            {
                foreach (var item in stackPanel.Children.OfType<ViewSelection>())
                {
                    item.CheckBox.IsChecked = true;
                }
            }
            
            ViewSelect.AllButton.IsChecked = false;
        }
        
        else if (ViewSelect.NoneButton.IsChecked == true)
        {
            foreach (var stackPanel in allViewSelectionStackPanels)
            {
                foreach (var item in stackPanel.Children.OfType<ViewSelection>())
                {
                    item.CheckBox.IsChecked = false;
                }
            }
            
            ViewSelect.NoneButton.IsChecked = false;
        }
        
        else if (ViewSelect.InvertButton.IsChecked == true)
        {
            foreach (var stackPanel in allViewSelectionStackPanels)
            {
                foreach (var item in stackPanel.Children.OfType<ViewSelection>())
                {
                    item.CheckBox.IsChecked = !item.CheckBox.IsChecked;
                }
            }
            
            ViewSelect.InvertButton.IsChecked = false;
        }
    }
}
