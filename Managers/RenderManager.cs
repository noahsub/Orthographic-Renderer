using System.Collections.Generic;

namespace Orthographic.Renderer.Managers;

public class RenderManager
{
    /// <summary>
    /// A list of all the views that can be rendered.
    /// </summary>
    public static List<string> RenderViews { get; set; } =
        [
            "top-front-right",
            "top-right-back",
            "top-back-left",
            "top-left-front",
            "front-right-bottom",
            "right-back-bottom",
            "back-left-bottom",
            "left-front-bottom",
            "top-front",
            "top-right",
            "top-back",
            "top-left",
            "front-bottom",
            "right-bottom",
            "back-bottom",
            "left-bottom",
            "front-right",
            "right-back",
            "back-left",
            "left-front",
            "front",
            "right",
            "back",
            "left",
            "top",
            "bottom",
        ];
}
