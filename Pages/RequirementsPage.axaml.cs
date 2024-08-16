using System;
using System.Diagnostics;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Platform;
using Newtonsoft.Json;

namespace Orthographic.Renderer.Pages;

public partial class RequirementsPage : UserControl
{
    private string _blenderPath = "";
    private string _pythonPath = "";
    
    
    public RequirementsPage()
    {
        InitializeComponent();
        GetExistingPaths();
    }

    private void GetExistingPaths()
    {
        // Open settings file and get its content
        const string filePath = "Data/settings.json";
        var content = File.ReadAllText(filePath);
        
        // Deserialize the content to a dynamic object using Newtonsoft.Json
        dynamic settings = JsonConvert.DeserializeObject(content);
        
        _blenderPath = settings["blender_path"].ToString();
        _pythonPath = settings["python_path"].ToString();

        BlenderPathTextBox.Text = _blenderPath;
        PythonPathTextBox.Text = _pythonPath;
    }

    private bool CheckBlenderPath()
    {
        // Check if the file exists
        if (!File.Exists(_blenderPath))
        {
            return false;
        }
        
        // Check if the file is called blender.exe
        if (!_blenderPath.EndsWith("blender.exe"))
        {
            return false;
        }
        
        // Run the version command to check if it's a valid Blender installation
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = _blenderPath,
                Arguments = "--version",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };
        
        process.Start();
        
        var output = process.StandardOutput.ReadToEnd();
        
        if (!output.Contains("Blender"))
        {
            return false;
        }

        return true;
    }
    
    private bool CheckPythonPath()
    {
        // Check if the file exists
        if (!File.Exists(_pythonPath))
        {
            return false;
        }
        
        // Check if the file is called python.exe
        if (!_pythonPath.EndsWith("python.exe"))
        {
            return false;
        }
        
        // Run the version command to check if it's a valid Python installation
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = _pythonPath,
                Arguments = "--version",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };
        
        process.Start();
        
        var output = process.StandardOutput.ReadToEnd();
        
        if (!output.Contains("Python"))
        {
            return false;
        }

        return true;
    }
    
    private string ReformatPath(string path)
    {
        string newPath = path;
        
        // Remove quotes from the path
        newPath = newPath.Replace("\"", "");
        
        // Replace all backslashes with forward slashes
        newPath = newPath.Replace("\\", "/");

        return newPath;
    }

    private void NextButton_OnClick(object? sender, RoutedEventArgs e)
    {
        BlenderPathTextBox.Text = ReformatPath(BlenderPathTextBox.Text);
        PythonPathTextBox.Text = ReformatPath(PythonPathTextBox.Text);
        
        _blenderPath = BlenderPathTextBox.Text;
        _pythonPath = PythonPathTextBox.Text;
     
        var blenderValid = CheckBlenderPath();
        
        if (blenderValid)
        {
            BlenderPathTextBox.BorderBrush = Brushes.MediumSpringGreen;
            // save new path to settings file
            dynamic settings = new
            {
                blender_path = _blenderPath,
                python_path = _pythonPath
            };
            
            var json = JsonConvert.SerializeObject(settings, Formatting.Indented);
            File.WriteAllText("Data/settings.json", json);
        }

        else
        {
            BlenderPathTextBox.BorderBrush = Brushes.Red;
        }
        
        var pythonValid = CheckPythonPath();
        
        if (pythonValid)
        {
            PythonPathTextBox.BorderBrush = Brushes.MediumSpringGreen;
            // save new path to settings file
            dynamic settings = new
            {
                blender_path = _blenderPath,
                python_path = _pythonPath
            };
            
            var json = JsonConvert.SerializeObject(settings, Formatting.Indented);
            File.WriteAllText("Data/settings.json", json);
        }

        else
        {
            PythonPathTextBox.BorderBrush = Brushes.Red;
        }
        
        if (blenderValid && pythonValid)
        {
            var parent = this.Parent as Window;
            if (parent != null)
            {
                parent.Content = new ModelSelectionPage();
            } 
        }
    }
}