using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml;

namespace Orthographic.Renderer.Controls;

public partial class ViewSortOptions : UserControl
{
    public ViewSortOptions()
    {
        InitializeComponent();
    }

    public List<string> GetSelectedFaces()
    {
        var selectedFaces = new List<string>();
        foreach (var control in FacesStackPanel.Children)
        {
            var button = (ToggleButton)control;
            if (button.IsChecked == true)
            {
                selectedFaces.Add(button.Content.ToString().ToLower());
            }
        }

        return selectedFaces;
    }
}
