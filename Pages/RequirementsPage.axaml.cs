using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Orthographic.Renderer.Controls;
using Orthographic.Renderer.Managers;

namespace Orthographic.Renderer.Pages;

public partial class RequirementsPage : UserControl
{
    public RequirementsPage()
    {
        InitializeComponent();
        LoadPaths();
        PathValid("blender", BlenderPathTextBox);
        // PathValid("python", PythonPathTextBox);
    }

    private bool PathValid(string key, BrowsableFileTextBox pathTextBox, bool save = false)
    {
        var path = NormalizePath(pathTextBox);
        if (path == null)
        {
            return false;
        }

        if (FileManager.VerifyProgramPath(key, path))
        {
            SetBorder(pathTextBox, true);
            if (save)
            {
                FileManager.WriteKeyValueToJsonFile("Data/program_paths.json", key, path);
            }
            return true;
        }

        SetBorder(pathTextBox, false);
        return false;
    }

    private void LoadPaths()
    {
        var paths = FileManager.ReadJsonKeyValue("Data/program_paths.json");
        var blenderPath = paths.blender;
        // var pythonPath = paths?.python;

        if (blenderPath != null)
        {
            BlenderPathTextBox.PathTextBox.Text = blenderPath;
        }

        // if (pythonPath != null)
        // {
        //     PythonPathTextBox.PathTextBox.Text = pythonPath;
        // }
    }

    private string? NormalizePath(BrowsableFileTextBox pathTextBox)
    {
        var path = pathTextBox.PathTextBox.Text;
        if (path == null)
        {
            return null;
        }

        path = FileManager.ReformatPath(path);
        pathTextBox.PathTextBox.Text = path;
        return path;
    }

    private void SetBorder(BrowsableFileTextBox pathTextBox, bool isValid)
    {
        pathTextBox.PathTextBox.BorderBrush = isValid
            ? Brushes.MediumSpringGreen
            : Brushes.IndianRed;
    }

    private void NextButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var blenderValid = PathValid("blender", BlenderPathTextBox, true);
        // var pythonValid = PathValid("python", PythonPathTextBox, true);

        // if (!blenderValid || !pythonValid)
        // {
        //     return;
        // }
        
        if (!blenderValid)
        {
            return;
        }
        
        DataManager.BlenderPath = BlenderPathTextBox.PathTextBox.Text ?? string.Empty;
        // DataManager.PythonPath = PythonPathTextBox.PathTextBox.Text;

        var mainWindow = (Windows.MainWindow)this.VisualRoot!;
        NavigationManager.SwitchPage(mainWindow, new ModelPage());
    }

    private void BlenderInstallButton_OnClick(object? sender, RoutedEventArgs e)
    {
        WebManager.OpenUrl("https://www.blender.org/download/");
    }

    // private void PythonInstallButton_OnClick(object? sender, RoutedEventArgs e)
    // {
    //     WebManager.OpenUrl("https://www.python.org/downloads/");
    // }
}
