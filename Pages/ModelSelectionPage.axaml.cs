﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using Orthographic.Renderer.Controls;
using Orthographic.Renderer.Managers;
using Hardware = Orthographic.Renderer.Entities.Hardware;

namespace Orthographic.Renderer.Pages;



public partial class ModelSelectionPage : UserControl
{
    public ModelSelectionPage()
    {
        InitializeComponent();
        
        // wait for RenderHardwareCollected to be true asynchronously

        Task.Run(() =>
        {
            while (!HardwareStatusManager.RenderHardwareCollected)
            {
                Thread.Sleep(500);
            }
        })
            .ContinueWith(task =>
            {
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    SetupHardwareGrid();
                    PopulateHardwareGrid();
                });
            });

        Task.Run(async () =>
        {
            var currentSize = MainWindow.GetSize();
            var currentPosition = MainWindow.GetPosition();
            var currentScrollPosition = 0.0;
            var currentMousePressed = MainWindow.IsPointerPressed;
            
            while (true)
            {
                await Task.Delay(1500);
                
                var newSize = MainWindow.GetSize();
                var newPosition = MainWindow.GetPosition();
                var newMousePressed = MainWindow.IsPointerPressed;
                
                // If the window is currently being moved or resized, skip the update
                if (currentSize != newSize || currentPosition != newPosition || newMousePressed != currentMousePressed)
                {
                    currentSize = newSize;
                    currentPosition = newPosition;
                    continue;
                }
                
                Dispatcher.UIThread.Post(() =>
                {
                    // If the scroll position has changed, skip the update
                    // Must be done in the UI thread
                    var newScrollPosition = RenderSelectionScrollViewer.Offset.Y;
                    
                    if (Math.Abs(currentScrollPosition - newScrollPosition) > 0.5f)
                    {
                        currentScrollPosition = newScrollPosition;
                        return;
                    }
                    
                    UpdateHardwareStatus();
                });
            }
        });
        

        PopulateAngles();
    }

    private void SetupHardwareGrid()
    {
        for (int i = 0; i < HardwareStatusManager.RenderHardware.Count; i++)
        {
            HardwareStatusGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        }
    }

    private void PopulateHardwareGrid()
    {
        for (int i = 0; i < HardwareStatusManager.RenderHardware.Count; i++)
        {
            var hardwareStatusControl = CreateHardwareStatusControl(HardwareStatusManager.RenderHardware[i]);
            Grid.SetColumn(hardwareStatusControl, i);
            HardwareStatusGrid.Children.Add(hardwareStatusControl);
        }
    }

    private void UpdateHardwareStatus()
    {
        // Catch concurrency issue where the grid has not been initialized yet
        if (HardwareStatusGrid is null)
        {
            return;
        }
        
        if (HardwareStatusGrid.Children.Count != HardwareStatusManager.RenderHardware.Count)
        {
            return;
        }
        
        for (int i = 0; i < HardwareStatusManager.RenderHardware.Count; i++)
        {
            HardwareStatusManager.RefreshHardware(HardwareStatusManager.Computer,
                HardwareStatusManager.RenderHardware[i]);
            var formattedValue = $"{HardwareStatusManager.RenderHardware[i].Value:0.00}";
            ((HardwareStatusControl)HardwareStatusGrid.Children[i]).ValueLabel.Content = formattedValue;
        }
    }
    

    private static Control CreateHardwareStatusControl(Hardware hardware)
    {
        var hardwareStatusControl = new HardwareStatusControl();
        hardwareStatusControl.TypeLabel.Content = HardwareStatusManager.FormatName(hardware);
        hardwareStatusControl.ValueLabel.Content = 0.00;
        hardwareStatusControl.UnitLabel.Content = hardware.Unit;
        return hardwareStatusControl;
    }

    private StackPanel GetAngleSelectionStackPanel(int index)
    {
        return index switch
        {
            0 => AngleSelectionStackPanel0,
            1 => AngleSelectionStackPanel1,
            2 => AngleSelectionStackPanel2,
            3 => AngleSelectionStackPanel3,
            _ => AngleSelectionStackPanel0
        };
    }

    private void PopulateAngles()
    {
        var angles = new List<string>
        {
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
            "bottom"
        };

        int i = 0;
        foreach (var angle in angles)
        {
            var renderSelectionControl = new RenderSelectionControl();
        
            var image = new Bitmap($"Assets/Images/RenderAngles/{angle}.png");
            renderSelectionControl.Image.Source = image;
            // remove hyphens from angle name
            var formattedName = angle.Replace("-", " ");
            // convert to title case
            formattedName = string.Concat(formattedName[..1].ToUpper(), formattedName.AsSpan(1));
            // set the name label
            renderSelectionControl.Name.Content = formattedName;
            // add the control to the stack panel
            var AngleSelectionStackPanel = GetAngleSelectionStackPanel(i % 4);
            AngleSelectionStackPanel.Children.Add(renderSelectionControl);
            i++;
        }
    }
}