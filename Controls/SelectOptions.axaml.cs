using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Orthographic.Renderer.Controls;

public partial class SelectOptions : UserControl
{
    public SelectOptions()
    {
        InitializeComponent();
    }

    private void AllButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (AllButton.IsChecked != true)
        {
            return;
        }

        NoneButton.IsChecked = false;
        InvertButton.IsChecked = false;
    }

    private void NoneButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (NoneButton.IsChecked != true)
        {
            return;
        }

        AllButton.IsChecked = false;
        InvertButton.IsChecked = false;
    }

    private void InvertButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (InvertButton.IsChecked != true)
        {
            return;
        }

        AllButton.IsChecked = false;
        NoneButton.IsChecked = false;
    }
}
