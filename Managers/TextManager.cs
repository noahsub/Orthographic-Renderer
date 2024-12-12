using System.Globalization;

namespace Orthographic.Renderer.Managers;

public class TextManager
{
    /// <summary>
    /// Converts a string to title case.
    /// </summary>
    /// <param name="str">The string to convert.</param>
    /// <returns>The string in title case.</returns>
    public static string ToTitleCase(string str)
    {
        return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str.ToLower());
    }
}