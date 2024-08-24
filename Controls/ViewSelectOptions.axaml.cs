using Avalonia.Controls;

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
