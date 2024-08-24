using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;

namespace Orthographic.Renderer.Controls;

public partial class BrowsableFileTextBox : UserControl
{
    public BrowsableFileTextBox()
    {
        InitializeComponent();
    }

    private async void BrowseButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this);

        var file = await topLevel.StorageProvider.OpenFilePickerAsync(
            new FilePickerOpenOptions { Title = "Select a file", AllowMultiple = false }
        );

        if (file.Count >= 1)
        {
            var path = file[0].Path.AbsolutePath;
            PathTextBox.Text = path;
        }
    }
}
