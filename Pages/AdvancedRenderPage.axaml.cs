using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Orthographic.Renderer.Controls;
using Orthographic.Renderer.Managers;

namespace Orthographic.Renderer.Pages;

public partial class AdvancedRenderPage : UserControl
{
    public AdvancedRenderPage()
    {
        InitializeComponent();
        ViewStackGrid.SetColumns(5);
        PopulateViews(RenderManager.RenderViews);
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
        foreach (var view in views)
        {
            // Create a new RenderViewControl
            var viewSelection = new ViewSelection();

            // Set the image source
            var image = new Bitmap($"Assets/Images/RenderAngles/{view}.png");
            viewSelection.Image.Source = image;

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
            viewSelection.Name.Content = formattedName;

            // add the control to the stack panel in the correct column
            ViewStackGrid.AddItem(viewSelection);
        }
    }

    private void ViewSelect_OnTapped(object? sender, TappedEventArgs e)
    {
        if (ViewSelect.AllButton.IsChecked == true)
        {
            foreach (ViewSelection view in ViewStackGrid.GetItems())
            {
                view.CheckBox.IsChecked = true;
            }

            ViewSelect.AllButton.IsChecked = false;
        }
        
        else if (ViewSelect.NoneButton.IsChecked == true)
        {
            foreach (ViewSelection view in ViewStackGrid.GetItems())
            {
                view.CheckBox.IsChecked = false;
            }
            
            ViewSelect.NoneButton.IsChecked = false;
        }
        
        else if (ViewSelect.InvertButton.IsChecked == true)
        {
            foreach (ViewSelection view in ViewStackGrid.GetItems())
            {
                view.CheckBox.IsChecked = !view.CheckBox.IsChecked;
            }
            
            ViewSelect.InvertButton.IsChecked = false;
        }
    }

    private void ViewSort_OnTapped(object? sender, TappedEventArgs e)
    {
        var selectedViews = GetSelectedViews();

        // Clear all views from the stack grid
        ViewStackGrid.ClearItems();

        var facesToSortBy = new List<string>();
        foreach (ToggleButton face in ViewSort.Faces.Children)
        {
            if (face.IsChecked == true)
            {
                facesToSortBy.Add(face.Content.ToString().ToLower());
            }
        }

        var matchingViews = new List<string>();
        var nonMatchingViews = new List<string>();
        foreach (var view in RenderManager.RenderViews)
        {
            if (facesToSortBy.All(view.Contains))
            {
                matchingViews.Add(view);
            }

            else
            {
                nonMatchingViews.Add(view);
            }
        }

        var sortedViews = new List<string>();
        sortedViews.AddRange(matchingViews);
        sortedViews.AddRange(nonMatchingViews);
        
        PopulateViews(sortedViews);

        foreach (ViewSelection view in ViewStackGrid.GetItems())
        {
            if (selectedViews.Contains(view.Name.Content.ToString()))
            {
                view.CheckBox.IsChecked = true;
            }
        }
    }

    private List<string> GetSelectedViews()
    {
        // Get the selected views
        var selectedViews = new List<string>();
        foreach (ViewSelection view in ViewStackGrid.GetItems())
        {
            if (view.CheckBox.IsChecked == true)
            {
                selectedViews.Add(view.Name.Content.ToString());
            }
        }

        return selectedViews;
    }
}