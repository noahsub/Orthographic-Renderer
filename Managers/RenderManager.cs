using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Orthographic.Renderer.Entities;

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

    public static Position GetPosition(string view, float distance)
    {
        var leg = ComputeTriangularLeg(distance);
        var mapping = new Dictionary<string, Position>
        {
            { "top-front-right", new Position(leg, -leg, distance, 45, 0, 45) },
            { "top-right-back", new Position(leg, leg, distance, 45, 0, 135) },
            { "top-back-left", new Position(-leg, leg, distance, 45, 0, 225) },
            { "top-left-front", new Position(-leg, -leg, distance, 45, 0, 315) },
            { "front-right-bottom", new Position(leg, -leg, -distance, 135, 0, 45) },
            { "right-back-bottom", new Position(leg, leg, -distance, 315, 180, -45) },
            { "back-left-bottom", new Position(-leg, leg, -distance, 315, -180, 45) },
            { "left-front-bottom", new Position(-leg, -leg, -distance, 135, 0, 315) },
            { "top-front", new Position(0, -leg, leg, 45, 0, 0) },
            { "top-right", new Position(leg, 0, leg, 45, 0, 90) },
            { "top-back", new Position(0, leg, leg, 225, 180, 0) },
            { "top-left", new Position(-leg, 0, leg, 45, 0, 270) },
            { "front-bottom", new Position(0, -leg, -leg, 135, 0, 0) },
            { "right-bottom", new Position(leg, 0, -leg, 135, 0, 90) },
            { "back-bottom", new Position(0, leg, -leg, 315, 180, 0) },
            { "left-bottom", new Position(-leg, 0, -leg, 135, 0, 270) },
            { "front-right", new Position(leg, -leg, 0, 90, 0, 45) },
            { "right-back", new Position(leg, leg, 0, 90, 0, 135) },
            { "back-left", new Position(-leg, leg, 0, 90, 0, 225) },
            { "left-front", new Position(-leg, -leg, 0, 90, 0, 315) },
            { "front", new Position(0, -distance, 0, 90, 0, 0) },
            { "right", new Position(distance, 0, 0, 90, 0, 90) },
            { "back", new Position(0, distance, 0, 90, 0, 180) },
            { "left", new Position(-distance, 0, 0, 90, 0, 270) },
            { "top", new Position(0, 0, distance, 0, 0, 0) },
            { "bottom", new Position(0, 0, -distance, 180, 0, 180) },
        };
        return mapping[view];
    }

    private static float ComputeTriangularLeg(float distance)
    {
        return (float)Math.Sqrt(Math.Pow(distance, 2) / 2);
    }
    
    public static string GetFormattedViewName(string view)
    {
        var formattedName = view;
        formattedName = formattedName.Replace("-", " ");
        formattedName = ToTitleCase(formattedName);
        return formattedName;
    }
    
    private static string ToTitleCase(string str)
    {
        return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str.ToLower());
    }
    
    public static List<string> SortViews(List<string> keys)
    {
        var matchingViews = new List<string>();
        var nonMatchingViews = new List<string>();

        foreach (var view in RenderViews)
        {
            var numMatchingViews = NumMatchingViews(view, keys);

            if (numMatchingViews == 0)
            {
                nonMatchingViews.Add(view);
            }
            
            else if (numMatchingViews == keys.Count)
            {
                matchingViews.Insert(0, view);
            }
            
            else
            {
                matchingViews.Add(view);
            }
        }
        
        return matchingViews.Concat(nonMatchingViews).ToList();
    }
    
    private static int NumMatchingViews(string view, List<string> keys)
    {
        return keys.Count(view.Contains);
    }
}
