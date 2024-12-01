////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// OrientationSelector.axaml.cs
// This file contains the logic for the OrientationSelector control.
//
// Copyright (C) 2024 noahsub
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// IMPORTS
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Orthographic.Renderer.Entities;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// NAMESPACE
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Orthographic.Renderer.Controls;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// ORIENTATION SELECTOR CLASS
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

/// <summary>
/// A control that allows the user to select the orientation of a 3D model in an orthographic perspective.
/// </summary>
public partial class OrientationSelector : UserControl
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // GLOBALS
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    
    /// <summary>
    /// The current orientation of the 3D model.
    /// </summary>
    public OrientationNode CurrentOrientation { get; set; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // ORIENTATION NODES
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    
    /// <summary>
    /// The node representing the back orientation.
    /// </summary>
    public readonly OrientationNode BackNode = new OrientationNode(
        name: "back",
        icon: "Assets/Images/RenderAngles/back.png"
    );
    
    /// <summary>
    /// The node representing the back-bottom orientation.
    /// </summary>
    public readonly OrientationNode BackBottomNode = new OrientationNode(
        name: "back-bottom",
        icon: "Assets/Images/RenderAngles/back-bottom.png"
    );
    
    /// <summary>
    /// The node representing the back-left orientation.
    /// </summary>
    public readonly OrientationNode BackLeftNode = new OrientationNode(
        name: "back-left",
        icon: "Assets/Images/RenderAngles/back-left.png"
    );
    
    /// <summary>
    /// The node representing the back-left-bottom orientation.
    /// </summary>
    public readonly OrientationNode BackLeftBottomNode = new OrientationNode(
        name: "back-left-bottom",
        icon: "Assets/Images/RenderAngles/back-left-bottom.png"
    );
    
    /// <summary>
    /// The node representing the bottom orientation.
    /// </summary>
    public readonly OrientationNode BottomNode = new OrientationNode(
        name: "bottom",
        icon: "Assets/Images/RenderAngles/bottom.png"
    );
    
    public readonly OrientationNode FrontNode = new OrientationNode(
        name: "front",
        icon: "Assets/Images/RenderAngles/front.png"
    );
    
    /// <summary>
    /// The node representing the front-bottom orientation.
    /// </summary>
    public readonly OrientationNode FrontBottomNode = new OrientationNode(
        name: "front-bottom",
        icon: "Assets/Images/RenderAngles/front-bottom.png"
    );
    
    /// <summary>
    /// The node representing the front-right orientation.
    /// </summary>
    public readonly OrientationNode FrontRightNode = new OrientationNode(
        name: "front-right",
        icon: "Assets/Images/RenderAngles/front-right.png"
    );
    
    /// <summary>
    /// The node representing the front-right-bottom orientation.
    /// </summary>
    public readonly OrientationNode FrontRightBottomNode = new OrientationNode(
        name: "front-right-bottom",
        icon: "Assets/Images/RenderAngles/front-right-bottom.png"
    );
    
    /// <summary>
    /// The node representing the left orientation.
    /// </summary>
    public readonly OrientationNode LeftNode = new OrientationNode(
        name: "left",
        icon: "Assets/Images/RenderAngles/left.png"
    );
    
    /// <summary>
    /// The node representing the left-bottom orientation.
    /// </summary>
    public readonly OrientationNode LeftBottomNode = new OrientationNode(
        name: "left-bottom",
        icon: "Assets/Images/RenderAngles/left-bottom.png"
    );
    
    /// <summary>
    /// The node representing the left-front orientation.
    /// </summary>
    public readonly OrientationNode LeftFrontNode = new OrientationNode(
        name: "left-front",
        icon: "Assets/Images/RenderAngles/left-front.png"
    );
    
    /// <summary>
    /// The node representing the left-front-bottom orientation.
    /// </summary>
    public readonly OrientationNode LeftFrontBottomNode = new OrientationNode(
        name: "left-front-bottom",
        icon: "Assets/Images/RenderAngles/left-front-bottom.png"
    );
    
    /// <summary>
    /// The node representing the right orientation.
    /// </summary>
    public readonly OrientationNode RightNode = new OrientationNode(
        name: "right",
        icon: "Assets/Images/RenderAngles/right.png"
    );
    
    /// <summary>
    /// The node representing the right-back orientation.
    /// </summary>
    public readonly OrientationNode RightBackNode = new OrientationNode(
        name: "right-back",
        icon: "Assets/Images/RenderAngles/right-back.png"
    );
    
    /// <summary>
    /// The node representing the right-back-bottom orientation.
    /// </summary>
    public readonly OrientationNode RightBackBottomNode = new OrientationNode(
        name: "right-back-bottom",
        icon: "Assets/Images/RenderAngles/right-back-bottom.png"
    );
    
    /// <summary>
    /// The node representing the right-bottom orientation.
    /// </summary>
    public readonly OrientationNode RightBottomNode = new OrientationNode(
        name: "right-bottom",
        icon: "Assets/Images/RenderAngles/right-bottom.png"
    );
    
    /// <summary>
    /// The node representing the top orientation.
    /// </summary>
    public readonly OrientationNode TopNode = new OrientationNode(
        name: "top",
        icon: "Assets/Images/RenderAngles/top.png"
    );
    
    /// <summary>
    /// The node representing the top-back orientation.
    /// </summary>
    public readonly OrientationNode TopBackNode = new OrientationNode(
        name: "top-back",
        icon: "Assets/Images/RenderAngles/top-back.png"
    );
    
    /// <summary>
    /// The node representing the top-back-left orientation.
    /// </summary>
    public readonly OrientationNode TopBackLeftNode = new OrientationNode(
        name: "top-back-left",
        icon: "Assets/Images/RenderAngles/top-back-left.png"
    );
    
    /// <summary>
    /// The node representing the top-front orientation.
    /// </summary>
    public readonly OrientationNode TopFrontNode = new OrientationNode(
        name: "top-front",
        icon: "Assets/Images/RenderAngles/top-front.png"
    );
    
    /// <summary>
    /// The node representing the top-front-right orientation.
    /// </summary>
    public readonly OrientationNode TopFrontRightNode = new OrientationNode(
        name: "top-front-right",
        icon: "Assets/Images/RenderAngles/top-front-right.png"
    );
    
    /// <summary>
    /// The node representing the top-left orientation.
    /// </summary>
    public readonly OrientationNode TopLeftNode = new OrientationNode(
        name: "top-left",
        icon: "Assets/Images/RenderAngles/top-left.png"
    );
    
    /// <summary>
    /// The node representing the top-left-front orientation.
    /// </summary>
    public readonly OrientationNode TopLeftFrontNode = new OrientationNode(
        name: "top-left-front",
        icon: "Assets/Images/RenderAngles/top-left-front.png"
    );
    
    /// <summary>
    /// The node representing the top-right orientation.
    /// </summary>
    public readonly OrientationNode TopRightNode = new OrientationNode(
        name: "top-right",
        icon: "Assets/Images/RenderAngles/top-right.png"
    );
    
    /// <summary>
    /// The node representing the top-right-back orientation.
    /// </summary>
    public readonly OrientationNode TopRightBackNode = new OrientationNode(
        name: "top-right-back",
        icon: "Assets/Images/RenderAngles/top-right-back.png"
    );

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // INITIALIZATION
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    
    /// <summary>
    /// Creates a new instance of the <see cref="OrientationSelector"/> class.
    /// </summary>
    public OrientationSelector()
    {
        InitializeComponent();

        // Set the next states of each node
        BackNode.AddLeft(BackLeftNode);
        BackNode.AddRight(RightBackNode);
        BackNode.AddUp(TopBackNode);
        BackNode.AddDown(BackBottomNode);

        BackBottomNode.AddLeft(RightBackBottomNode);
        BackBottomNode.AddRight(BackLeftBottomNode);
        BackBottomNode.AddUp(BackNode);
        BackBottomNode.AddDown(BottomNode);

        BackLeftNode.AddLeft(BackNode);
        BackLeftNode.AddRight(LeftNode);
        BackLeftNode.AddUp(TopBackLeftNode);
        BackLeftNode.AddDown(BackLeftBottomNode);

        BackLeftBottomNode.AddLeft(BackBottomNode);
        BackLeftBottomNode.AddRight(LeftBottomNode);
        BackLeftBottomNode.AddUp(BackLeftNode);
        BackLeftBottomNode.AddDown(BottomNode);

        BottomNode.AddLeft(LeftBottomNode);
        BottomNode.AddRight(RightBottomNode);
        BottomNode.AddUp(FrontRightNode);
        BottomNode.AddDown(BackBottomNode);

        FrontNode.AddLeft(LeftFrontNode);
        FrontNode.AddRight(FrontRightNode);
        FrontNode.AddUp(TopFrontNode);
        FrontNode.AddDown(FrontBottomNode);

        FrontBottomNode.AddLeft(LeftFrontBottomNode);
        FrontBottomNode.AddRight(FrontRightBottomNode);
        FrontBottomNode.AddUp(FrontNode);
        FrontBottomNode.AddDown(BottomNode);

        LeftNode.AddLeft(BackLeftNode);
        LeftNode.AddRight(LeftFrontNode);
        LeftNode.AddUp(TopLeftNode);
        LeftNode.AddDown(LeftBottomNode);

        LeftBottomNode.AddLeft(BackLeftBottomNode);
        LeftBottomNode.AddRight(LeftFrontBottomNode);
        LeftBottomNode.AddUp(LeftNode);
        LeftBottomNode.AddDown(BottomNode);

        LeftFrontNode.AddLeft(LeftNode);
        LeftFrontNode.AddRight(FrontNode);
        LeftFrontNode.AddUp(TopLeftFrontNode);
        LeftFrontNode.AddDown(LeftFrontBottomNode);

        LeftFrontBottomNode.AddLeft(LeftBottomNode);
        LeftFrontBottomNode.AddRight(FrontBottomNode);
        LeftFrontBottomNode.AddUp(LeftFrontNode);
        LeftFrontBottomNode.AddDown(BottomNode);

        RightNode.AddLeft(FrontRightNode);
        RightNode.AddRight(RightBackNode);
        RightNode.AddUp(TopRightNode);
        RightNode.AddDown(RightBottomNode);

        RightBackNode.AddLeft(RightNode);
        RightBackNode.AddRight(BackNode);
        RightBackNode.AddUp(TopRightBackNode);
        RightBackNode.AddDown(RightBackBottomNode);

        RightBackBottomNode.AddLeft(RightBottomNode);
        RightBackBottomNode.AddRight(BackBottomNode);
        RightBackBottomNode.AddUp(RightBackNode);
        RightBackBottomNode.AddDown(BottomNode);

        RightBottomNode.AddLeft(FrontRightBottomNode);
        RightBottomNode.AddRight(RightBackBottomNode);
        RightBottomNode.AddUp(RightNode);
        RightBottomNode.AddDown(BottomNode);

        TopNode.AddLeft(TopLeftNode);
        TopNode.AddRight(TopRightNode);
        TopNode.AddUp(TopBackNode);
        TopNode.AddDown(TopFrontNode);

        TopBackNode.AddLeft(TopRightBackNode);
        TopBackNode.AddRight(TopBackLeftNode);
        TopBackNode.AddUp(TopNode);
        TopBackNode.AddDown(BackNode);

        TopBackLeftNode.AddLeft(TopBackNode);
        TopBackLeftNode.AddRight(TopLeftNode);
        TopBackLeftNode.AddUp(TopNode);
        TopBackLeftNode.AddDown(BackLeftNode);

        TopFrontNode.AddLeft(TopLeftFrontNode);
        TopFrontNode.AddRight(TopFrontRightNode);
        TopFrontNode.AddUp(TopNode);
        TopFrontNode.AddDown(FrontNode);

        TopFrontRightNode.AddLeft(TopFrontNode);
        TopFrontRightNode.AddRight(TopRightNode);
        TopFrontRightNode.AddUp(TopNode);
        TopFrontRightNode.AddDown(FrontRightNode);

        TopLeftNode.AddLeft(TopBackLeftNode);
        TopLeftNode.AddRight(TopLeftFrontNode);
        TopLeftNode.AddUp(TopNode);
        TopLeftNode.AddDown(LeftNode);

        TopLeftFrontNode.AddLeft(TopLeftNode);
        TopLeftFrontNode.AddRight(TopFrontNode);
        TopLeftFrontNode.AddUp(TopNode);
        TopLeftFrontNode.AddDown(LeftFrontNode);

        TopRightNode.AddLeft(TopFrontRightNode);
        TopRightNode.AddRight(TopRightBackNode);
        TopRightNode.AddUp(TopNode);
        TopRightNode.AddDown(RightNode);

        TopRightBackNode.AddLeft(TopRightNode);
        TopRightBackNode.AddRight(TopBackNode);
        TopRightBackNode.AddUp(TopNode);
        TopRightBackNode.AddDown(RightBackNode);

        FrontRightNode.AddLeft(FrontNode);
        FrontRightNode.AddRight(RightNode);
        FrontRightNode.AddUp(TopFrontRightNode);
        FrontRightNode.AddDown(FrontRightBottomNode);

        FrontRightBottomNode.AddLeft(FrontBottomNode);
        FrontRightBottomNode.AddRight(RightBottomNode);
        FrontRightBottomNode.AddUp(FrontRightNode);
        FrontRightBottomNode.AddDown(BottomNode);

        // Set the current orientation to the top-front-right node
        CurrentOrientation = TopFrontRightNode;
        // Set the image of the orientation selector to the icon of the current orientation
        OrientationImage.Source = new Bitmap(CurrentOrientation.Icon);
    }
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // SETTERS
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    
    public void SetOrientation(string orientation)
    {
        CurrentOrientation = orientation switch
        {
            "back" => BackNode,
            "back-bottom" => BackBottomNode,
            "back-left" => BackLeftNode,
            "back-left-bottom" => BackLeftBottomNode,
            "bottom" => BottomNode,
            "front" => FrontNode,
            "front-bottom" => FrontBottomNode,
            "front-right" => FrontRightNode,
            "front-right-bottom" => FrontRightBottomNode,
            "left" => LeftNode,
            "left-bottom" => LeftBottomNode,
            "left-front" => LeftFrontNode,
            "left-front-bottom" => LeftFrontBottomNode,
            "right" => RightNode,
            "right-back" => RightBackNode,
            "right-back-bottom" => RightBackBottomNode,
            "right-bottom" => RightBottomNode,
            "top" => TopNode,
            "top-back" => TopBackNode,
            "top-back-left" => TopBackLeftNode,
            "top-front" => TopFrontNode,
            "top-front-right" => TopFrontRightNode,
            "top-left" => TopLeftNode,
            "top-left-front" => TopLeftFrontNode,
            "top-right" => TopRightNode,
            "top-right-back" => TopRightBackNode,
            _ => CurrentOrientation,
        };

        OrientationImage.Source = new Bitmap(CurrentOrientation.Icon);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // EVENTS
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    
    /// <summary>
    /// Changes the orientation of the 3D model the left node of the current orientation.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void LeftButton_OnClick(object? sender, RoutedEventArgs e)
    {
        CurrentOrientation = CurrentOrientation.Left;
        OrientationImage.Source = new Bitmap(CurrentOrientation.Icon);
    }

    /// <summary>
    /// Changes the orientation of the 3D model the up node of the current orientation.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void UpButton_OnClick(object? sender, RoutedEventArgs e)
    {
        CurrentOrientation = CurrentOrientation.Up;
        OrientationImage.Source = new Bitmap(CurrentOrientation.Icon);
    }

    /// <summary>
    /// Changes the orientation of the 3D model the right node of the current orientation.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void RightButton_OnClick(object? sender, RoutedEventArgs e)
    {
        CurrentOrientation = CurrentOrientation.Right;
        OrientationImage.Source = new Bitmap(CurrentOrientation.Icon);
    }

    /// <summary>
    /// Changes the orientation of the 3D model the down node of the current orientation.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void DownButton_OnClick(object? sender, RoutedEventArgs e)
    {
        CurrentOrientation = CurrentOrientation.Down;
        OrientationImage.Source = new Bitmap(CurrentOrientation.Icon);
    }
}
