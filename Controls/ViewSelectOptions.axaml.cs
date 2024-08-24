using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Orthographic.Renderer.Controls;

public partial class ViewSelectOptions : UserControl
{
    public ViewSelectOptions()
    {
        InitializeComponent();
    }

    public void ClearSelection()
    {
        AllButton.IsChecked = false;
        NoneButton.IsChecked = false;
        InvertButton.IsChecked = false;
    }
}
