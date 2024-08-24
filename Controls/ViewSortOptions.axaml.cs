using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

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
                selectedFaces.Add(button.Content?.ToString()?.ToLower() ?? string.Empty);
            }
        }

        return selectedFaces;
    }
}
