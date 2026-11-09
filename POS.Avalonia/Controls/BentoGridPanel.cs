using System;
using Avalonia;
using Avalonia.Controls;

namespace POS.Avalonia.Controls;

/// <summary>Flow layout with fixed columns and per-child row/column span (bento grid).</summary>
public class BentoGridPanel : Panel
{
    public static readonly StyledProperty<int> ColumnsProperty =
        AvaloniaProperty.Register<BentoGridPanel, int>(nameof(Columns), 4);

    public static readonly StyledProperty<double> GapProperty =
        AvaloniaProperty.Register<BentoGridPanel, double>(nameof(Gap), 12);

    public static readonly StyledProperty<double> RowHeightProperty =
        AvaloniaProperty.Register<BentoGridPanel, double>(nameof(RowHeight), 116);

    public static readonly AttachedProperty<int> ColumnSpanProperty =
        AvaloniaProperty.RegisterAttached<BentoGridPanel, Control, int>("ColumnSpan", 1);

    public static readonly AttachedProperty<int> RowSpanProperty =
        AvaloniaProperty.RegisterAttached<BentoGridPanel, Control, int>("RowSpan", 1);

    public int Columns
    {
        get => GetValue(ColumnsProperty);
        set => SetValue(ColumnsProperty, value);
    }

    public double Gap
    {
        get => GetValue(GapProperty);
        set => SetValue(GapProperty, value);
    }

    public double RowHeight
    {
        get => GetValue(RowHeightProperty);
        set => SetValue(RowHeightProperty, value);
    }

    public static int GetColumnSpan(Control element) => element.GetValue(ColumnSpanProperty);
    public static void SetColumnSpan(Control element, int value) => element.SetValue(ColumnSpanProperty, value);
    public static int GetRowSpan(Control element) => element.GetValue(RowSpanProperty);
    public static void SetRowSpan(Control element, int value) => element.SetValue(RowSpanProperty, value);

    protected override Size MeasureOverride(Size availableSize)
    {
        var cols = Math.Max(1, Columns);
        var gap = Gap;
        var rowH = RowHeight;
        var width = double.IsInfinity(availableSize.Width)
            ? cols * 168 + gap * (cols - 1)
            : availableSize.Width;
        var cellW = Math.Max(0, (width - gap * (cols - 1)) / cols);

        foreach (var child in Children)
        {
            var colSpan = Math.Clamp(GetColumnSpan(child), 1, cols);
            var rowSpan = Math.Max(1, GetRowSpan(child));
            var w = cellW * colSpan + gap * (colSpan - 1);
            var h = rowH * rowSpan + gap * (rowSpan - 1);
            child.Measure(new Size(w, h));
        }

        var rows = LayoutRows(cols);
        var totalH = rows * rowH + Math.Max(0, rows - 1) * gap;
        return new Size(width, totalH);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var cols = Math.Max(1, Columns);
        var gap = Gap;
        var rowH = RowHeight;
        var cellW = Math.Max(0, (finalSize.Width - gap * (cols - 1)) / cols);
        var rows = LayoutRows(cols);
        var occ = new bool[rows, cols];

        foreach (var child in Children)
        {
            var colSpan = Math.Clamp(GetColumnSpan(child), 1, cols);
            var rowSpan = Math.Max(1, GetRowSpan(child));
            var (row, col) = FindSlot(occ, cols, colSpan, rowSpan);

            for (var r = row; r < row + rowSpan; r++)
            for (var c = col; c < col + colSpan; c++)
                occ[r, c] = true;

            var x = col * (cellW + gap);
            var y = row * (rowH + gap);
            var w = cellW * colSpan + gap * (colSpan - 1);
            var h = rowH * rowSpan + gap * (rowSpan - 1);
            child.Arrange(new Rect(x, y, w, h));
        }

        var totalH = rows * rowH + Math.Max(0, rows - 1) * gap;
        return new Size(finalSize.Width, totalH);
    }

    private int LayoutRows(int cols)
    {
        var rows = 1;
        var occ = new bool[rows, cols];

        foreach (var child in Children)
        {
            var colSpan = Math.Clamp(GetColumnSpan(child), 1, cols);
            var rowSpan = Math.Max(1, GetRowSpan(child));
            var (row, col) = FindSlot(occ, cols, colSpan, rowSpan);
            if (row + rowSpan > rows)
            {
                var next = new bool[row + rowSpan, cols];
                for (var r = 0; r < rows; r++)
                for (var c = 0; c < cols; c++)
                    next[r, c] = occ[r, c];
                occ = next;
                rows = row + rowSpan;
            }

            for (var r = row; r < row + rowSpan; r++)
            for (var c = col; c < col + colSpan; c++)
                occ[r, c] = true;
        }

        return rows;
    }

    private static (int row, int col) FindSlot(bool[,] occ, int cols, int colSpan, int rowSpan)
    {
        var rows = occ.GetLength(0);
        for (var r = 0; r < rows; r++)
        for (var c = 0; c <= cols - colSpan; c++)
        {
            if (!Fits(occ, r, c, colSpan, rowSpan)) continue;
            return (r, c);
        }

        return (rows, 0);
    }

    private static bool Fits(bool[,] occ, int row, int col, int colSpan, int rowSpan)
    {
        var rows = occ.GetLength(0);
        var cols = occ.GetLength(1);
        if (row + rowSpan > rows) return true;
        for (var r = row; r < row + rowSpan; r++)
        for (var c = col; c < col + colSpan; c++)
            if (occ[r, c]) return false;
        return true;
    }
}
