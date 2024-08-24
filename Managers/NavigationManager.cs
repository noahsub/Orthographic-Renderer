using Avalonia.Controls;

namespace Orthographic.Renderer.Managers;

public class NavigationManager
{
    public static void SwitchPage(Windows.MainWindow mainWindow, UserControl page)
    {
        var pageContent = mainWindow.FindControl<ContentControl>("PageContent");

        if (pageContent != null)
        {
            pageContent.Content = page;
        }
    }
}
