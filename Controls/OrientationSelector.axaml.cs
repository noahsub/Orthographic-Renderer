////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// OrientationSelector.axaml.cs
// This file contains the logic for the OrientationSelector control.
//
// Copyright (C) 2024 noahsub
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// IMPORTS
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
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
    public OrientationNode CurrentOrientation { get; private set; }

    public event EventHandler OrientationChanged = delegate { };

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // ORIENTATION NODES
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// The node representing the back orientation.
    /// </summary>
    private readonly OrientationNode _backNode = new OrientationNode(
        name: "back",
        icon: "Assets/Images/RenderAngles/back.png"
    );

    /// <summary>
    /// The node representing the back-bottom orientation.
    /// </summary>
    private readonly OrientationNode _backBottomNode = new OrientationNode(
        name: "back-bottom",
        icon: "Assets/Images/RenderAngles/back-bottom.png"
    );

    /// <summary>
    /// The node representing the back-left orientation.
    /// </summary>
    private readonly OrientationNode _backLeftNode = new OrientationNode(
        name: "back-left",
        icon: "Assets/Images/RenderAngles/back-left.png"
    );

    /// <summary>
    /// The node representing the back-left-bottom orientation.
    /// </summary>
    private readonly OrientationNode _backLeftBottomNode = new OrientationNode(
        name: "back-left-bottom",
        icon: "Assets/Images/RenderAngles/back-left-bottom.png"
    );

    /// <summary>
    /// The node representing the bottom orientation.
    /// </summary>
    private readonly OrientationNode _bottomNode = new OrientationNode(
        name: "bottom",
        icon: "Assets/Images/RenderAngles/bottom.png"
    );

    private readonly OrientationNode _frontNode = new OrientationNode(
        name: "front",
        icon: "Assets/Images/RenderAngles/front.png"
    );

    /// <summary>
    /// The node representing the front-bottom orientation.
    /// </summary>
    private readonly OrientationNode _frontBottomNode = new OrientationNode(
        name: "front-bottom",
        icon: "Assets/Images/RenderAngles/front-bottom.png"
    );

    /// <summary>
    /// The node representing the front-right orientation.
    /// </summary>
    private readonly OrientationNode _frontRightNode = new OrientationNode(
        name: "front-right",
        icon: "Assets/Images/RenderAngles/front-right.png"
    );

    /// <summary>
    /// The node representing the front-right-bottom orientation.
    /// </summary>
    private readonly OrientationNode _frontRightBottomNode = new OrientationNode(
        name: "front-right-bottom",
        icon: "Assets/Images/RenderAngles/front-right-bottom.png"
    );

    /// <summary>
    /// The node representing the left orientation.
    /// </summary>
    private readonly OrientationNode _leftNode = new OrientationNode(
        name: "left",
        icon: "Assets/Images/RenderAngles/left.png"
    );

    /// <summary>
    /// The node representing the left-bottom orientation.
    /// </summary>
    private readonly OrientationNode _leftBottomNode = new OrientationNode(
        name: "left-bottom",
        icon: "Assets/Images/RenderAngles/left-bottom.png"
    );

    /// <summary>
    /// The node representing the left-front orientation.
    /// </summary>
    private readonly OrientationNode _leftFrontNode = new OrientationNode(
        name: "left-front",
        icon: "Assets/Images/RenderAngles/left-front.png"
    );

    /// <summary>
    /// The node representing the left-front-bottom orientation.
    /// </summary>
    private readonly OrientationNode _leftFrontBottomNode = new OrientationNode(
        name: "left-front-bottom",
        icon: "Assets/Images/RenderAngles/left-front-bottom.png"
    );

    /// <summary>
    /// The node representing the right orientation.
    /// </summary>
    private readonly OrientationNode _rightNode = new OrientationNode(
        name: "right",
        icon: "Assets/Images/RenderAngles/right.png"
    );

    /// <summary>
    /// The node representing the right-back orientation.
    /// </summary>
    private readonly OrientationNode _rightBackNode = new OrientationNode(
        name: "right-back",
        icon: "Assets/Images/RenderAngles/right-back.png"
    );

    /// <summary>
    /// The node representing the right-back-bottom orientation.
    /// </summary>
    private readonly OrientationNode _rightBackBottomNode = new OrientationNode(
        name: "right-back-bottom",
        icon: "Assets/Images/RenderAngles/right-back-bottom.png"
    );

    /// <summary>
    /// The node representing the right-bottom orientation.
    /// </summary>
    private readonly OrientationNode _rightBottomNode = new OrientationNode(
        name: "right-bottom",
        icon: "Assets/Images/RenderAngles/right-bottom.png"
    );

    /// <summary>
    /// The node representing the top orientation.
    /// </summary>
    private readonly OrientationNode _topNode = new OrientationNode(
        name: "top",
        icon: "Assets/Images/RenderAngles/top.png"
    );

    /// <summary>
    /// The node representing the top-back orientation.
    /// </summary>
    private readonly OrientationNode _topBackNode = new OrientationNode(
        name: "top-back",
        icon: "Assets/Images/RenderAngles/top-back.png"
    );

    /// <summary>
    /// The node representing the top-back-left orientation.
    /// </summary>
    private readonly OrientationNode _topBackLeftNode = new OrientationNode(
        name: "top-back-left",
        icon: "Assets/Images/RenderAngles/top-back-left.png"
    );

    /// <summary>
    /// The node representing the top-front orientation.
    /// </summary>
    private readonly OrientationNode _topFrontNode = new OrientationNode(
        name: "top-front",
        icon: "Assets/Images/RenderAngles/top-front.png"
    );

    /// <summary>
    /// The node representing the top-front-right orientation.
    /// </summary>
    private readonly OrientationNode _topFrontRightNode = new OrientationNode(
        name: "top-front-right",
        icon: "Assets/Images/RenderAngles/top-front-right.png"
    );

    /// <summary>
    /// The node representing the top-left orientation.
    /// </summary>
    private readonly OrientationNode _topLeftNode = new OrientationNode(
        name: "top-left",
        icon: "Assets/Images/RenderAngles/top-left.png"
    );

    /// <summary>
    /// The node representing the top-left-front orientation.
    /// </summary>
    private readonly OrientationNode _topLeftFrontNode = new OrientationNode(
        name: "top-left-front",
        icon: "Assets/Images/RenderAngles/top-left-front.png"
    );

    /// <summary>
    /// The node representing the top-right orientation.
    /// </summary>
    private readonly OrientationNode _topRightNode = new OrientationNode(
        name: "top-right",
        icon: "Assets/Images/RenderAngles/top-right.png"
    );

    /// <summary>
    /// The node representing the top-right-back orientation.
    /// </summary>
    private readonly OrientationNode _topRightBackNode = new OrientationNode(
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
        _backNode.AddLeft(_backLeftNode);
        _backNode.AddRight(_rightBackNode);
        _backNode.AddUp(_topBackNode);
        _backNode.AddDown(_backBottomNode);

        _backBottomNode.AddLeft(_rightBackBottomNode);
        _backBottomNode.AddRight(_backLeftBottomNode);
        _backBottomNode.AddUp(_backNode);
        _backBottomNode.AddDown(_bottomNode);

        _backLeftNode.AddLeft(_backNode);
        _backLeftNode.AddRight(_leftNode);
        _backLeftNode.AddUp(_topBackLeftNode);
        _backLeftNode.AddDown(_backLeftBottomNode);

        _backLeftBottomNode.AddLeft(_backBottomNode);
        _backLeftBottomNode.AddRight(_leftBottomNode);
        _backLeftBottomNode.AddUp(_backLeftNode);
        _backLeftBottomNode.AddDown(_bottomNode);

        _bottomNode.AddLeft(_leftBottomNode);
        _bottomNode.AddRight(_rightBottomNode);
        _bottomNode.AddUp(_frontRightNode);
        _bottomNode.AddDown(_backBottomNode);

        _frontNode.AddLeft(_leftFrontNode);
        _frontNode.AddRight(_frontRightNode);
        _frontNode.AddUp(_topFrontNode);
        _frontNode.AddDown(_frontBottomNode);

        _frontBottomNode.AddLeft(_leftFrontBottomNode);
        _frontBottomNode.AddRight(_frontRightBottomNode);
        _frontBottomNode.AddUp(_frontNode);
        _frontBottomNode.AddDown(_bottomNode);

        _leftNode.AddLeft(_backLeftNode);
        _leftNode.AddRight(_leftFrontNode);
        _leftNode.AddUp(_topLeftNode);
        _leftNode.AddDown(_leftBottomNode);

        _leftBottomNode.AddLeft(_backLeftBottomNode);
        _leftBottomNode.AddRight(_leftFrontBottomNode);
        _leftBottomNode.AddUp(_leftNode);
        _leftBottomNode.AddDown(_bottomNode);

        _leftFrontNode.AddLeft(_leftNode);
        _leftFrontNode.AddRight(_frontNode);
        _leftFrontNode.AddUp(_topLeftFrontNode);
        _leftFrontNode.AddDown(_leftFrontBottomNode);

        _leftFrontBottomNode.AddLeft(_leftBottomNode);
        _leftFrontBottomNode.AddRight(_frontBottomNode);
        _leftFrontBottomNode.AddUp(_leftFrontNode);
        _leftFrontBottomNode.AddDown(_bottomNode);

        _rightNode.AddLeft(_frontRightNode);
        _rightNode.AddRight(_rightBackNode);
        _rightNode.AddUp(_topRightNode);
        _rightNode.AddDown(_rightBottomNode);

        _rightBackNode.AddLeft(_rightNode);
        _rightBackNode.AddRight(_backNode);
        _rightBackNode.AddUp(_topRightBackNode);
        _rightBackNode.AddDown(_rightBackBottomNode);

        _rightBackBottomNode.AddLeft(_rightBottomNode);
        _rightBackBottomNode.AddRight(_backBottomNode);
        _rightBackBottomNode.AddUp(_rightBackNode);
        _rightBackBottomNode.AddDown(_bottomNode);

        _rightBottomNode.AddLeft(_frontRightBottomNode);
        _rightBottomNode.AddRight(_rightBackBottomNode);
        _rightBottomNode.AddUp(_rightNode);
        _rightBottomNode.AddDown(_bottomNode);

        _topNode.AddLeft(_topLeftNode);
        _topNode.AddRight(_topRightNode);
        _topNode.AddUp(_topBackNode);
        _topNode.AddDown(_topFrontNode);

        _topBackNode.AddLeft(_topRightBackNode);
        _topBackNode.AddRight(_topBackLeftNode);
        _topBackNode.AddUp(_topNode);
        _topBackNode.AddDown(_backNode);

        _topBackLeftNode.AddLeft(_topBackNode);
        _topBackLeftNode.AddRight(_topLeftNode);
        _topBackLeftNode.AddUp(_topNode);
        _topBackLeftNode.AddDown(_backLeftNode);

        _topFrontNode.AddLeft(_topLeftFrontNode);
        _topFrontNode.AddRight(_topFrontRightNode);
        _topFrontNode.AddUp(_topNode);
        _topFrontNode.AddDown(_frontNode);

        _topFrontRightNode.AddLeft(_topFrontNode);
        _topFrontRightNode.AddRight(_topRightNode);
        _topFrontRightNode.AddUp(_topNode);
        _topFrontRightNode.AddDown(_frontRightNode);

        _topLeftNode.AddLeft(_topBackLeftNode);
        _topLeftNode.AddRight(_topLeftFrontNode);
        _topLeftNode.AddUp(_topNode);
        _topLeftNode.AddDown(_leftNode);

        _topLeftFrontNode.AddLeft(_topLeftNode);
        _topLeftFrontNode.AddRight(_topFrontNode);
        _topLeftFrontNode.AddUp(_topNode);
        _topLeftFrontNode.AddDown(_leftFrontNode);

        _topRightNode.AddLeft(_topFrontRightNode);
        _topRightNode.AddRight(_topRightBackNode);
        _topRightNode.AddUp(_topNode);
        _topRightNode.AddDown(_rightNode);

        _topRightBackNode.AddLeft(_topRightNode);
        _topRightBackNode.AddRight(_topBackNode);
        _topRightBackNode.AddUp(_topNode);
        _topRightBackNode.AddDown(_rightBackNode);

        _frontRightNode.AddLeft(_frontNode);
        _frontRightNode.AddRight(_rightNode);
        _frontRightNode.AddUp(_topFrontRightNode);
        _frontRightNode.AddDown(_frontRightBottomNode);

        _frontRightBottomNode.AddLeft(_frontBottomNode);
        _frontRightBottomNode.AddRight(_rightBottomNode);
        _frontRightBottomNode.AddUp(_frontRightNode);
        _frontRightBottomNode.AddDown(_bottomNode);

        // Set the current orientation to the top-front-right node
        CurrentOrientation = _topFrontRightNode;
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
            "back" => _backNode,
            "back-bottom" => _backBottomNode,
            "back-left" => _backLeftNode,
            "back-left-bottom" => _backLeftBottomNode,
            "bottom" => _bottomNode,
            "front" => _frontNode,
            "front-bottom" => _frontBottomNode,
            "front-right" => _frontRightNode,
            "front-right-bottom" => _frontRightBottomNode,
            "left" => _leftNode,
            "left-bottom" => _leftBottomNode,
            "left-front" => _leftFrontNode,
            "left-front-bottom" => _leftFrontBottomNode,
            "right" => _rightNode,
            "right-back" => _rightBackNode,
            "right-back-bottom" => _rightBackBottomNode,
            "right-bottom" => _rightBottomNode,
            "top" => _topNode,
            "top-back" => _topBackNode,
            "top-back-left" => _topBackLeftNode,
            "top-front" => _topFrontNode,
            "top-front-right" => _topFrontRightNode,
            "top-left" => _topLeftNode,
            "top-left-front" => _topLeftFrontNode,
            "top-right" => _topRightNode,
            "top-right-back" => _topRightBackNode,
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
        OrientationChanged(this, e);
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
        OrientationChanged(this, e);
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
        OrientationChanged(this, e);
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
        OrientationChanged(this, e);
    }
}
