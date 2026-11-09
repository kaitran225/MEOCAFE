using Avalonia;
using Avalonia.Controls;
using Avalonia.VisualTree;
using POS.Avalonia.Controls;

namespace POS.Avalonia.Views;

public partial class BentoHomeView : UserControl
{
    public BentoHomeView() => InitializeComponent();

    private void OnRootSizeChanged(object? sender, SizeChangedEventArgs e)
    {
        var width = e.NewSize.Width;
        var columns = width switch
        {
            < 640 => 2,
            < 920 => 3,
            _ => 4
        };

        foreach (var panel in this.GetVisualDescendants())
        {
            if (panel is BentoGridPanel grid)
                grid.Columns = columns;
        }
    }
}
