namespace Orthographic.Renderer.Managers;

public class DialogManager
{
    public static void ShowElevatedPermissionsWarningDialog()
    {
        var warning = new Windows.Warning();
        warning.SetWarning("This path requires elevated permissions. Please run the program as an administrator or select a different path.");
        warning.Show();
    }
}