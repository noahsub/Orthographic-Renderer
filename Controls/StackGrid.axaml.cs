////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// StackGrid.axaml.cs
// This file contains the logic for the StackGrid control.
//
// Copyright (C) 2024 noahsub
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// IMPORTS
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// NAMESPACE
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Orthographic.Renderer.Controls;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// STACK GRID CLASS
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

/// <summary>
/// A combination of a grid and stack panels that allow the layout of items with a specified number of columns and an
/// unlimited number of rows.
/// </summary>
public partial class StackGrid : UserControl
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // PROPERTIES
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// The number of columns in the grid.
    /// </summary>
    private int NumColumns { get; set; }

    /// <summary>
    /// The total number of items in the grid.
    /// </summary>
    private int NumItems { get; set; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // INITIALIZATION
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Initializes a new instance of the <see cref="StackGrid"/> class.
    /// </summary>
    public StackGrid()
    {
        InitializeComponent();
        // Set the default number of columns to 0
        NumColumns = 0;
        // Set the number of items to 0
        NumItems = 0;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // LAYOUT
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Sets the layout of the grid to have the specified number of columns with the specified margin size.
    /// </summary>
    /// <param name="columns">The number of columns in the grid.</param>
    /// <param name="marginSize">The size of the margin between columns and rows.</param>
    /// <param name="endMargins">If the program should have margins on left of the first column and the right of the
    /// last column.</param>
    public void SetLayout(int columns, int marginSize = 20, bool endMargins = true)
    {
        // Clear the current columns
        Items.ColumnDefinitions.Clear();

        // Create columns
        for (var i = 0; i < columns; i++)
        {
            Items.ColumnDefinitions.Add(
                // Create a new column with width that takes up the available space
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
            );

            // Create stack panel for each column
            var stackPanel = new StackPanel
            {
                Name = $"ItemColumn{i}",
                Orientation = Orientation.Vertical,
                Spacing = marginSize,
                Margin = new Thickness(marginSize, 0, 0, 0),
            };

            // If the first column and no end margins, set the left margin to 0
            if (i == 0 && !endMargins)
            {
                stackPanel.Margin = new Thickness(0, 0, 0, 0);
            }

            // If the last column
            if (i == columns - 1)
            {
                // If there are end margins, set the right margin to the margin size
                if (endMargins)
                {
                    stackPanel.Margin = new Thickness(marginSize, 0, marginSize, 0);
                }
                // If there are no end margins, set the right margin to 0
                else
                {
                    stackPanel.Margin = new Thickness(marginSize, 0, 0, 0);
                }
            }

            // Set the column of the stack panel
            Grid.SetColumn(stackPanel, i);
            // Add the stack panel to the items grid
            Items.Children.Add(stackPanel);
        }

        // Set the number of columns
        NumColumns = columns;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // ITEM MANAGEMENT
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Add an item to the stack grid.
    /// </summary>
    /// <param name="item">The item to add.</param>
    public void AddItem(Control item)
    {
        // If there are no columns, return
        if (NumColumns == 0)
        {
            return;
        }

        // Determine the column to add the item to
        var column = NumItems % NumColumns;
        // Add the item to the column
        var stackPanel = (StackPanel)Items.Children[column];
        stackPanel.Children.Add(item);
        // Increment the number of items
        NumItems++;
    }

    /// <summary>
    /// Clear all items from the stack grid.
    /// </summary>
    public void ClearItems()
    {
        // Clear all items from each stack panel
        foreach (var stackPanel in Items.Children)
        {
            ((StackPanel)stackPanel).Children.Clear();
        }

        // Set the number of items to 0
        NumItems = 0;
    }

    /// <summary>
    /// Get all items in the stack grid from left to right, top to bottom.
    /// </summary>
    /// <returns>The items in the stack grid.</returns>
    public IEnumerable<Control> GetItems()
    {
        // Iterate through each item in the stack grid
        for (var i = 0; i < NumItems; i++)
        {
            // Determine the column of the item
            var column = i % NumColumns;
            // Get the item from the stack panel
            var stackPanel = (StackPanel)Items.Children[column];
            // Return the item
            yield return stackPanel.Children[i / NumColumns];
        }
    }
}
