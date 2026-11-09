using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using POS.Avalonia.Controls;
using POS.Avalonia.ViewModels;

namespace POS.Avalonia.Views;

public partial class BentoHomeView : UserControl
{
    public BentoHomeView() => InitializeComponent();

    private void OnRootSizeChanged(object? sender, SizeChangedEventArgs e)
    {
        // Square tiles grow with the column width, so add columns as the window widens
        // instead of letting one tile eat the screen.
        var columns = e.NewSize.Width switch
        {
            < 520 => 2,
            < 760 => 3,
            < 1000 => 4,
            < 1240 => 5,
            < 1480 => 6,
            < 1720 => 7,
            _ => 8
        };

        foreach (var panel in this.GetVisualDescendants())
        {
            if (panel is BentoGridPanel grid)
                grid.Columns = columns;
        }
    }

    /// <summary>Tiles are generated from BentoHomeViewModel, so resolve the tile from its DataContext.</summary>
    private void OnTileClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Button { DataContext: BentoTile tile } && DataContext is BentoHomeViewModel home)
            home.OpenTileCommand.Execute(tile);
    }
}
