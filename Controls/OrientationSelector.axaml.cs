using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Orthographic.Renderer.Entities;

namespace Orthographic.Renderer.Controls;

public partial class OrientationSelector : UserControl
{
    public OrientationNode CurrentOrientation { get; set; }
    
    public OrientationSelector()
    {
        InitializeComponent();
        
        var backNode = new OrientationNode(name:"back", icon:"Assets/Images/RenderAngles/back.png");
        var backBottomNode = new OrientationNode(name:"back-bottom", icon:"Assets/Images/RenderAngles/back-bottom.png");
        var backLeftNode = new OrientationNode(name:"back-left", icon:"Assets/Images/RenderAngles/back-left.png");
        var backLeftBottomNode = new OrientationNode(name:"back-left-bottom", icon:"Assets/Images/RenderAngles/back-left-bottom.png");
        var bottomNode = new OrientationNode(name:"bottom", icon:"Assets/Images/RenderAngles/bottom.png");
        var frontNode = new OrientationNode(name:"front", icon:"Assets/Images/RenderAngles/front.png");
        var frontBottomNode = new OrientationNode(name:"front-bottom", icon:"Assets/Images/RenderAngles/front-bottom.png");
        var frontRightNode = new OrientationNode(name:"front-right", icon:"Assets/Images/RenderAngles/front-right.png");
        var frontRightBottomNode = new OrientationNode(name:"front-right-bottom", icon:"Assets/Images/RenderAngles/front-right-bottom.png");
        var leftNode = new OrientationNode(name:"left", icon:"Assets/Images/RenderAngles/left.png");
        var leftBottomNode = new OrientationNode(name:"left-bottom", icon:"Assets/Images/RenderAngles/left-bottom.png");
        var leftFrontNode = new OrientationNode(name:"left-front", icon:"Assets/Images/RenderAngles/left-front.png");
        var leftFrontBottomNode = new OrientationNode(name:"left-front-bottom", icon:"Assets/Images/RenderAngles/left-front-bottom.png");
        var rightNode = new OrientationNode(name:"right", icon:"Assets/Images/RenderAngles/right.png");
        var rightBackNode = new OrientationNode(name:"right-back", icon:"Assets/Images/RenderAngles/right-back.png");
        var rightBackBottomNode = new OrientationNode(name:"right-back-bottom", icon:"Assets/Images/RenderAngles/right-back-bottom.png");
        var rightBottomNode = new OrientationNode(name:"right-bottom", icon:"Assets/Images/RenderAngles/right-bottom.png");
        var topNode = new OrientationNode(name:"top", icon:"Assets/Images/RenderAngles/top.png");
        var topBackNode = new OrientationNode(name:"top-back", icon:"Assets/Images/RenderAngles/top-back.png");
        var topBackLeftNode = new OrientationNode(name:"top-back-left", icon:"Assets/Images/RenderAngles/top-back-left.png");
        var topFrontNode = new OrientationNode(name:"top-front", icon:"Assets/Images/RenderAngles/top-front.png");
        var topFrontRightNode = new OrientationNode(name:"top-front-right", icon:"Assets/Images/RenderAngles/top-front-right.png");
        var topLeftNode = new OrientationNode(name:"top-left", icon:"Assets/Images/RenderAngles/top-left.png");
        var topLeftFrontNode = new OrientationNode(name:"top-left-front", icon:"Assets/Images/RenderAngles/top-left-front.png");
        var topRightNode = new OrientationNode(name:"top-right", icon:"Assets/Images/RenderAngles/top-right.png");
        var topRightBackNode = new OrientationNode(name:"top-right-back", icon:"Assets/Images/RenderAngles/top-right-back.png");
        
        backNode.AddLeft(backLeftNode);
        backNode.AddRight(rightBackNode);
        backNode.AddUp(topBackNode);
        backNode.AddDown(backBottomNode);
        
        backBottomNode.AddLeft(rightBackBottomNode);
        backBottomNode.AddRight(backLeftBottomNode);
        backBottomNode.AddUp(backNode);
        backBottomNode.AddDown(bottomNode);
        
        backLeftNode.AddLeft(backNode);
        backLeftNode.AddRight(leftNode);
        backLeftNode.AddUp(topBackLeftNode);
        backLeftNode.AddDown(backLeftBottomNode);
        
        backLeftBottomNode.AddLeft(backBottomNode);
        backLeftBottomNode.AddRight(leftBottomNode);
        backLeftBottomNode.AddUp(backLeftNode);
        backLeftBottomNode.AddDown(bottomNode);
        
        bottomNode.AddLeft(leftBottomNode);
        bottomNode.AddRight(rightBottomNode);
        bottomNode.AddUp(frontRightNode);
        bottomNode.AddDown(backBottomNode);
        
        frontNode.AddLeft(leftFrontNode);
        frontNode.AddRight(frontRightNode);
        frontNode.AddUp(topFrontNode);
        frontNode.AddDown(frontBottomNode);
        
        frontBottomNode.AddLeft(leftFrontBottomNode);
        frontBottomNode.AddRight(frontRightBottomNode);
        frontBottomNode.AddUp(frontNode);
        frontBottomNode.AddDown(bottomNode);
        
        leftNode.AddLeft(backLeftNode);
        leftNode.AddRight(leftFrontNode);
        leftNode.AddUp(topLeftNode);
        leftNode.AddDown(leftBottomNode);
        
        leftBottomNode.AddLeft(backLeftBottomNode);
        leftBottomNode.AddRight(leftFrontBottomNode);
        leftBottomNode.AddUp(leftNode);
        leftBottomNode.AddDown(bottomNode);
        
        leftFrontNode.AddLeft(leftNode);
        leftFrontNode.AddRight(frontNode);
        leftFrontNode.AddUp(topLeftFrontNode);
        leftFrontNode.AddDown(leftFrontBottomNode);
        
        leftFrontBottomNode.AddLeft(leftBottomNode);
        leftFrontBottomNode.AddRight(frontBottomNode);
        leftFrontBottomNode.AddUp(leftFrontNode);
        leftFrontBottomNode.AddDown(bottomNode);
        
        rightNode.AddLeft(frontRightNode);
        rightNode.AddRight(rightBackNode);
        rightNode.AddUp(topRightNode);
        rightNode.AddDown(rightBottomNode);
        
        rightBackNode.AddLeft(rightNode);
        rightBackNode.AddRight(backNode);
        rightBackNode.AddUp(topRightBackNode);
        rightBackNode.AddDown(rightBackBottomNode);
        
        rightBackBottomNode.AddLeft(rightBottomNode);
        rightBackBottomNode.AddRight(backBottomNode);
        rightBackBottomNode.AddUp(rightBackNode);
        rightBackBottomNode.AddDown(bottomNode);
        
        rightBottomNode.AddLeft(frontRightBottomNode);
        rightBottomNode.AddRight(rightBackBottomNode);
        rightBottomNode.AddUp(rightNode);
        rightBottomNode.AddDown(bottomNode);
        
        topNode.AddLeft(topLeftNode);
        topNode.AddRight(topRightNode);
        topNode.AddUp(topBackNode);
        topNode.AddDown(topFrontNode);
        
        topBackNode.AddLeft(topRightBackNode);
        topBackNode.AddRight(topBackLeftNode);
        topBackNode.AddUp(topNode);
        topBackNode.AddDown(backNode);
        
        topBackLeftNode.AddLeft(topBackNode);
        topBackLeftNode.AddRight(topLeftNode);
        topBackLeftNode.AddUp(topNode);
        topBackLeftNode.AddDown(backLeftNode);
        
        topFrontNode.AddLeft(topLeftFrontNode);
        topFrontNode.AddRight(topFrontRightNode);
        topFrontNode.AddUp(topNode);
        topFrontNode.AddDown(frontNode);
        
        topFrontRightNode.AddLeft(topFrontNode);
        topFrontRightNode.AddRight(topRightNode);
        topFrontRightNode.AddUp(topNode);
        topFrontRightNode.AddDown(frontRightNode);
        
        topLeftNode.AddLeft(topBackLeftNode);
        topLeftNode.AddRight(topLeftFrontNode);
        topLeftNode.AddUp(topNode);
        topLeftNode.AddDown(leftNode);
        
        topLeftFrontNode.AddLeft(topLeftNode);
        topLeftFrontNode.AddRight(topFrontRightNode);
        topLeftFrontNode.AddUp(topNode);
        topLeftFrontNode.AddDown(leftFrontNode);
        
        topRightNode.AddLeft(topFrontRightNode);
        topRightNode.AddRight(topRightBackNode);
        topRightNode.AddUp(topNode);
        topRightNode.AddDown(rightNode);
        
        topRightBackNode.AddLeft(topRightNode);
        topRightBackNode.AddRight(topBackNode);
        topRightBackNode.AddUp(topNode);
        topRightBackNode.AddDown(rightBackNode);
        
        frontRightNode.AddLeft(frontNode);
        frontRightNode.AddRight(rightNode);
        frontRightNode.AddUp(topFrontRightNode);
        frontRightNode.AddDown(frontRightBottomNode);
        
        frontRightBottomNode.AddLeft(frontBottomNode);
        frontRightBottomNode.AddRight(rightBottomNode);
        frontRightBottomNode.AddUp(frontRightNode);
        frontRightBottomNode.AddDown(bottomNode);

        CurrentOrientation = frontNode;
        OrientationImage.Source = new Bitmap(frontNode.Icon);
    }

    private void LeftButton_OnClick(object? sender, RoutedEventArgs e)
    {
        CurrentOrientation = CurrentOrientation.Left;
        OrientationImage.Source = new Bitmap(CurrentOrientation.Icon);
    }

    private void UpButton_OnClick(object? sender, RoutedEventArgs e)
    {
        CurrentOrientation = CurrentOrientation.Up;
        OrientationImage.Source = new Bitmap(CurrentOrientation.Icon);
    }

    private void RightButton_OnClick(object? sender, RoutedEventArgs e)
    {
        CurrentOrientation = CurrentOrientation.Right;
        OrientationImage.Source = new Bitmap(CurrentOrientation.Icon);
    }

    private void DownButton_OnClick(object? sender, RoutedEventArgs e)
    {
        CurrentOrientation = CurrentOrientation.Down;
        OrientationImage.Source = new Bitmap(CurrentOrientation.Icon);
    }
}