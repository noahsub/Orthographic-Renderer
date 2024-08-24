using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;

namespace Orthographic.Renderer.Controls;

public partial class StackGrid : UserControl
{
    public int NumColumns { get; set; }
    public int NumItems { get; set; }

    public StackGrid()
    {
        InitializeComponent();
        NumColumns = 0;
        NumItems = 0;
    }

    public void SetColumns(int columns, int marginSize = 20, bool endMargins = true)
    {
        Items.ColumnDefinitions.Clear();

        for (var i = 0; i < columns; i++)
        {
            Items.ColumnDefinitions.Add(
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

            if (i == 0 && !endMargins)
            {
                stackPanel.Margin = new Thickness(0, 0, marginSize, 0);
            }

            if (i == columns - 1)
            {
                if (endMargins)
                {
                    stackPanel.Margin = new Thickness(marginSize, 0, marginSize, 0);
                }
                else
                {
                    stackPanel.Margin = new Thickness(marginSize, 0, 0, 0);
                }
            }

            Grid.SetColumn(stackPanel, i);
            Items.Children.Add(stackPanel);
        }

        NumColumns = columns;
    }

    public void AddItem(Control item)
    {
        if (NumColumns == 0)
        {
            return;
        }

        var column = NumItems % NumColumns;
        var stackPanel = (StackPanel)Items.Children[column];
        stackPanel.Children.Add(item);
        NumItems++;
    }

    public void ClearItems()
    {
        foreach (var stackPanel in Items.Children)
        {
            ((StackPanel)stackPanel).Children.Clear();
        }

        NumItems = 0;
    }

    public IEnumerable<Control> GetItems()
    {
        for (var i = 0; i < NumItems; i++)
        {
            var column = i % NumColumns;
            var stackPanel = (StackPanel)Items.Children[column];
            yield return stackPanel.Children[i / NumColumns];
        }
    }
}
