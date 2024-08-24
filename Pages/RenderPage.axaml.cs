using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Orthographic.Renderer.Controls;
using Orthographic.Renderer.Managers;

namespace Orthographic.Renderer.Pages;

public partial class RenderPage : UserControl
{
    public RenderPage()
    {
        InitializeComponent();
        ViewStackGrid.SetColumns(5);
        PopulateViews(RenderManager.RenderViews);
    }

    private void PopulateViews(List<string> views)
    {
        foreach (var view in views)
        {
            var viewSelection = new ViewSelection();
            viewSelection.SetName(view);
            viewSelection.SetImage(view);
            ViewStackGrid.AddItem(viewSelection);
        }
    }

    private void SelectViews()
    {
        foreach (var control in ViewStackGrid.GetItems())
        {
            var view = (ViewSelection)control;

            if (ViewSelectOptions.AllButton.IsChecked == true)
            {
                view.SetSelected(true);
            }
            
            else if (ViewSelectOptions.NoneButton.IsChecked == true)
            {
                view.SetSelected(false);
            }

            else if (ViewSelectOptions.InvertButton.IsChecked == true)
            {
                view.SetSelected(!view.GetSelected());
            }
        }
        
        ViewSelectOptions.ClearSelection();
    }

    public void SortViews()
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

    private void ViewSelect_OnTapped(object? sender, TappedEventArgs e)
    {
        SelectViews();
    }

    private void ViewSort_OnTapped(object? sender, TappedEventArgs e)
    {
        SortViews();
    }

    private async void RenderButton_OnClick(object? sender, RoutedEventArgs e)
    {
        
    }

    private void LockPage()
    {
        Settings.IsEnabled = false;
        ViewSelectOptions.IsEnabled = false;
        ViewSortOptions.IsEnabled = false;
        ViewStackGrid.IsEnabled = false;
        RenderButton.IsEnabled = false;
    }

    private void UnlockPage()
    {
        Settings.IsEnabled = true;
        ViewSelectOptions.IsEnabled = true;
        ViewSortOptions.IsEnabled = true;
        ViewStackGrid.IsEnabled = true;
        RenderButton.IsEnabled = true;
    }
}
