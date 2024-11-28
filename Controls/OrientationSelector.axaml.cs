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
    
    public readonly OrientationNode BackNode = new OrientationNode(name:"back", icon:"Assets/Images/RenderAngles/back.png");
    public readonly OrientationNode BackBottomNode = new OrientationNode(name:"back-bottom", icon:"Assets/Images/RenderAngles/back-bottom.png");
    public readonly OrientationNode BackLeftNode = new OrientationNode(name:"back-left", icon:"Assets/Images/RenderAngles/back-left.png");
    public readonly OrientationNode BackLeftBottomNode = new OrientationNode(name:"back-left-bottom", icon:"Assets/Images/RenderAngles/back-left-bottom.png");
    public readonly OrientationNode BottomNode = new OrientationNode(name:"bottom", icon:"Assets/Images/RenderAngles/bottom.png");
    public readonly OrientationNode FrontNode = new OrientationNode(name:"front", icon:"Assets/Images/RenderAngles/front.png");
    public readonly OrientationNode FrontBottomNode = new OrientationNode(name:"front-bottom", icon:"Assets/Images/RenderAngles/front-bottom.png");
    public readonly OrientationNode FrontRightNode = new OrientationNode(name:"front-right", icon:"Assets/Images/RenderAngles/front-right.png");
    public readonly OrientationNode FrontRightBottomNode = new OrientationNode(name:"front-right-bottom", icon:"Assets/Images/RenderAngles/front-right-bottom.png");
    public readonly OrientationNode LeftNode = new OrientationNode(name:"left", icon:"Assets/Images/RenderAngles/left.png");
    public readonly OrientationNode LeftBottomNode = new OrientationNode(name:"left-bottom", icon:"Assets/Images/RenderAngles/left-bottom.png");
    public readonly OrientationNode LeftFrontNode = new OrientationNode(name:"left-front", icon:"Assets/Images/RenderAngles/left-front.png");
    public readonly OrientationNode LeftFrontBottomNode = new OrientationNode(name:"left-front-bottom", icon:"Assets/Images/RenderAngles/left-front-bottom.png");
    public readonly OrientationNode RightNode = new OrientationNode(name:"right", icon:"Assets/Images/RenderAngles/right.png");
    public readonly OrientationNode RightBackNode = new OrientationNode(name:"right-back", icon:"Assets/Images/RenderAngles/right-back.png");
    public readonly OrientationNode RightBackBottomNode = new OrientationNode(name:"right-back-bottom", icon:"Assets/Images/RenderAngles/right-back-bottom.png");
    public readonly OrientationNode RightBottomNode = new OrientationNode(name:"right-bottom", icon:"Assets/Images/RenderAngles/right-bottom.png");
    public readonly OrientationNode TopNode = new OrientationNode(name:"top", icon:"Assets/Images/RenderAngles/top.png");
    public readonly OrientationNode TopBackNode = new OrientationNode(name:"top-back", icon:"Assets/Images/RenderAngles/top-back.png");
    public readonly OrientationNode TopBackLeftNode = new OrientationNode(name:"top-back-left", icon:"Assets/Images/RenderAngles/top-back-left.png");
    public readonly OrientationNode TopFrontNode = new OrientationNode(name:"top-front", icon:"Assets/Images/RenderAngles/top-front.png");
    public readonly OrientationNode TopFrontRightNode = new OrientationNode(name:"top-front-right", icon:"Assets/Images/RenderAngles/top-front-right.png");
    public readonly OrientationNode TopLeftNode = new OrientationNode(name:"top-left", icon:"Assets/Images/RenderAngles/top-left.png");
    public readonly OrientationNode TopLeftFrontNode = new OrientationNode(name:"top-left-front", icon:"Assets/Images/RenderAngles/top-left-front.png");
    public readonly OrientationNode TopRightNode = new OrientationNode(name:"top-right", icon:"Assets/Images/RenderAngles/top-right.png");
    public readonly OrientationNode TopRightBackNode = new OrientationNode(name:"top-right-back", icon:"Assets/Images/RenderAngles/top-right-back.png");
    
    public OrientationSelector()
    {
        InitializeComponent();
        
        
        
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

        CurrentOrientation = TopFrontRightNode;
        OrientationImage.Source = new Bitmap(CurrentOrientation.Icon);
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
            _ => CurrentOrientation
        };

        OrientationImage.Source = new Bitmap(CurrentOrientation.Icon);
    }
}