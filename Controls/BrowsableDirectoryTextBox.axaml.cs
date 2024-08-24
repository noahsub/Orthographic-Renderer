using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;

namespace Orthographic.Renderer.Controls;

public partial class BrowsableDirectoryTextBox : UserControl
{
    public BrowsableDirectoryTextBox()
    {
        InitializeComponent();
    }

    private async void BrowseButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this);

        var directory = await topLevel.StorageProvider.OpenFolderPickerAsync(
            new FolderPickerOpenOptions() { Title = "Select a Directory" }
        );

        if (directory.Count >= 1)
        {
            var path = directory[0].Path.AbsolutePath;
            PathTextBox.Text = path;
        }
    }
}
