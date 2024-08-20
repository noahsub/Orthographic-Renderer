using System.Diagnostics;
using System.IO;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Newtonsoft.Json;
using Orthographic.Renderer.Managers;

namespace Orthographic.Renderer.Pages;

public partial class RequirementsPage : UserControl
{
    public RequirementsPage()
    {
        InitializeComponent();
        CheckSavedPaths();
    }

    private bool CheckPathValid(string key, string path)
    {
        // Ensure that the file exists
        if (!File.Exists(path))
        {
            return false;
        }
        
        // Ensure that the key matches the file
        // for example if the key is "blender" then the path should be "blender" in its last segment
        var lastSegment = Path.GetFileNameWithoutExtension(path).ToLower();
        if (!lastSegment.Equals(key.ToLower()))
        {
            return false;
        }
        
        // Check that the --version argument of the executable is able to be run
        var process = new System.Diagnostics.Process
        {
            StartInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = path,
                Arguments = "--version",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };
        
        process.Start();
        var output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();
        
        return !string.IsNullOrEmpty(output);
    }
    
    private void CheckSavedPaths()
    {
        var paths = FileManager.ReadJsonFile("Data/program_paths.json");
        var blenderPathValid = CheckPathValid("blender", paths["blender"].ToString());
        var pythonPathValid = CheckPathValid("python", paths["python"].ToString());
        
        if (blenderPathValid)
        {
            BlenderPathTextBox.Text = paths["blender"];
            BlenderPathTextBox.BorderBrush = Brushes.MediumSpringGreen;
        }

        else
        {
            BlenderPathTextBox.BorderBrush = Brushes.Red;
        }
        
        if (pythonPathValid)
        {
            PythonPathTextBox.Text = paths["python"];
            PythonPathTextBox.BorderBrush = Brushes.MediumSpringGreen;
        }

        else
        {
            PythonPathTextBox.BorderBrush = Brushes.Red;
        }
    }

    private string ReformatPath(string path)
    {
        var newPath = path;
        
        // Remove double quotes in the path
        newPath = newPath.Replace("\"", "");
        
        // Convert backslashes to forward slashes
        newPath = newPath.Replace("\\", "/");
        
        return newPath;
    }

    private void NextButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var blenderValid = false;
        var pythonValid = false;
        
        if (BlenderPathTextBox.Text != null)
        {
            BlenderPathTextBox.Text = ReformatPath(BlenderPathTextBox.Text);
        }

        if (PythonPathTextBox.Text != null)
        {
            PythonPathTextBox.Text = ReformatPath(PythonPathTextBox.Text);
        }
        
        var blenderPath = BlenderPathTextBox.Text;
        var pythonPath = PythonPathTextBox.Text;

        if (blenderPath != null)
        {
            if (CheckPathValid("blender", blenderPath))
            {
                FileManager.WriteToJsonFile(path: "Data/program_paths.json", key: "blender", value: blenderPath);
                BlenderPathTextBox.BorderBrush = Brushes.MediumSpringGreen;
                blenderValid = true;
            }

            else
            {
                BlenderPathTextBox.BorderBrush = Brushes.Red;
            }
        }

        if (pythonPath != null)
        {
            if (CheckPathValid("python", pythonPath))
            {
                FileManager.WriteToJsonFile(path: "Data/program_paths.json", key: "python", value: pythonPath);
                PythonPathTextBox.BorderBrush = Brushes.MediumSpringGreen;
                pythonValid = true;
            }

            else
            {
                PythonPathTextBox.BorderBrush = Brushes.Red;
            }
        }
        
        if (blenderValid && pythonValid)
        {
            // Get the ContentControl called "PageContent" from the MainWindow
            var mainWindow = (MainWindow) this.VisualRoot;
            var pageContent = mainWindow.FindControl<ContentControl>("PageContent");
            pageContent.Content = new RenderPage();
        }
    }
}