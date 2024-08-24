using Avalonia.Controls;
using Orthographic.Renderer.Pages;

namespace Orthographic.Renderer.Managers;

public class NavigationManager
{
    public static void SwitchPage(MainWindow mainWindow, UserControl page)
    {
        var pageContent = mainWindow.FindControl<ContentControl>("PageContent");
        
        if (pageContent != null)
        {
            pageContent.Content = new ModelPage();
        }
    }
}