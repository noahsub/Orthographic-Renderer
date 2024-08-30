////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// RequirementsPage.axaml.cs
// This file contains the logic for the RequirementsPage.
//
// Author(s): https://github.com/noahsub
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// IMPORTS
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Orthographic.Renderer.Controls;
using Orthographic.Renderer.Managers;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// NAMESPACE
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Orthographic.Renderer.Pages;

/// <summary>
/// Represents the requirements page of the application.
/// </summary>
public partial class RequirementsPage : UserControl
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RequirementsPage"/> class.
    /// </summary>
    public RequirementsPage()
    {
        InitializeComponent();

        // Load serialized paths
        LoadPaths();

        // Check if the blender path is valid
        PathValid("blender", BlenderPathTextBox);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // PATH
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Loads the paths from the configuration file.
    /// </summary>
    private void LoadPaths()
    {
        // Get the paths from the file
        var paths = FileManager.ReadJsonKeyValue(FileManager.GetProgramPathsFile());
        // Get the blender path
        var blenderPath = paths.blender;

        // If the blender path is not null, set the text box to the path
        if (blenderPath != null)
        {
            BlenderPathTextBox.PathTextBox.Text = blenderPath;
        }
    }

    /// <summary>
    /// Normalizes the specified path.
    /// </summary>
    /// <param name="pathTextBox">The text box containing the path.</param>
    /// <returns>The normalized path, or <c>null</c> if the path is invalid.</returns>
    private static string? NormalizePath(BrowsableFileTextBox pathTextBox)
    {
        // Get the path from the text box
        var path = pathTextBox.PathTextBox.Text;

        // If the path is null, return null
        if (path == null)
        {
            return null;
        }

        // Reformat the path
        path = FileManager.ReformatPath(path);

        // Set the text box to the reformatted path
        pathTextBox.PathTextBox.Text = path;

        return path;
    }

    /// <summary>
    /// Validates the specified path.
    /// </summary>
    /// <param name="key">The key associated with the path.</param>
    /// <param name="pathTextBox">The text box containing the path.</param>
    /// <param name="save">Whether to save the path if valid.</param>
    /// <returns><c>true</c> if the path is valid; otherwise, <c>false</c>.</returns>
    private static bool PathValid(string key, BrowsableFileTextBox pathTextBox, bool save = false)
    {
        // Normalize the path
        var path = NormalizePath(pathTextBox);

        // If the path is null, return false
        if (path == null)
        {
            return false;
        }

        // Verify the program path
        if (FileManager.VerifyProgramPath(key, path))
        {
            // Set the border color to green
            SetBorder(pathTextBox, true);

            // Save the path if specified
            if (save)
            {
                FileManager.WriteKeyValueToJsonFile(FileManager.GetProgramPathsFile(), key, path);
            }

            return true;
        }

        // Set the border color to red
        SetBorder(pathTextBox, false);
        return false;
    }

    private static void DisplayWarning(string message)
    {
        var warning = new Windows.Warning();
        warning.SetWarning(message);
        warning.Show();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // HELPER FUNCTIONS
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Sets the border color of the specified text box based on its validity.
    /// </summary>
    /// <param name="pathTextBox">The text box to update.</param>
    /// <param name="isValid">Whether the path is valid.</param>
    private static void SetBorder(BrowsableFileTextBox pathTextBox, bool isValid)
    {
        // Set the border color based on the validity of the path (green if valid, red if invalid)
        pathTextBox.PathTextBox.BorderBrush = isValid
            ? Brushes.MediumSpringGreen
            : Brushes.IndianRed;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // EVENTS
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Handles the click event of the Next button.
    /// </summary>
    private void NextButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var blenderPath = FileManager.ReformatPath(
            BlenderPathTextBox.PathTextBox.Text ?? string.Empty
        );

        if (blenderPath == string.Empty)
        {
            SoundManager.PlaySound("Assets/Sounds/error.mp3");
            SetBorder(BlenderPathTextBox, false);
            return;
        }

        if (FileManager.ElevatedPath(blenderPath))
        {
            SoundManager.PlaySound("Assets/Sounds/error.mp3");
            SetBorder(BlenderPathTextBox, false);
            DisplayWarning(
                "Blender is installed in a protected directory. Please run this application as an administrator or install the portable version of Blender."
            );
            return;
        }

        // Check if the blender path is valid
        var blenderValid = PathValid("blender", BlenderPathTextBox, true);

        // If the blender path is not valid, play an error sound and return
        if (!blenderValid)
        {
            SoundManager.PlaySound("Assets/Sounds/error.mp3");
            SetBorder(BlenderPathTextBox, false);
            return;
        }

        SetBorder(BlenderPathTextBox, true);

        // Set the blender path
        DataManager.BlenderPath = blenderPath;

        // Switch to the ModelPage
        var mainWindow = (Windows.MainWindow)this.VisualRoot!;
        NavigationManager.SwitchPage(mainWindow, new ModelPage());
    }

    /// <summary>
    /// Handles the click event of the Blender Install button.
    /// </summary>
    private void BlenderInstallButton_OnClick(object? sender, RoutedEventArgs e)
    {
        // Open the Blender download page
        WebManager.OpenUrl("https://www.blender.org/download/");
    }
}
