using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Orthographic.Renderer.Managers;

namespace Orthographic.Renderer.Controls;

public partial class ViewSelection : UserControl
{
    public string Key { get; set; }

    public ViewSelection()
    {
        InitializeComponent();
    }

    public void SetName(string view)
    {
        var name = RenderManager.GetFormattedViewName(view);
        NameLabel.Content = name;
        Key = view;
    }
    
    public void SetImage(string view)
    {
        ViewImage.Source = new Bitmap($"Assets/Images/RenderAngles/{view}.png");
    }
    
    public void SetSelected(bool selected)
    {
        SelectCheckBox.IsChecked = selected;
    }
    
    public bool GetSelected()
    {
        var value = SelectCheckBox.IsChecked;
        if (value == null)
        {
            return false;
        }
        
        return (bool)SelectCheckBox.IsChecked!;
    }
}
