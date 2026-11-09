using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Controls;

namespace POS.Avalonia.Controls;

public class BarChartItem : INotifyPropertyChanged
{
    private string _label = "";
    private double _value;
    private double _barHeight;

    public string Label
    {
        get => _label;
        set { _label = value; OnPropertyChanged(); }
    }

    public double Value
    {
        get => _value;
        set { _value = value; OnPropertyChanged(); }
    }

    public double BarHeight
    {
        get => _barHeight;
        internal set { _barHeight = value; OnPropertyChanged(); }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}

public partial class BarChart : UserControl
{
    private const double MaxBarHeight = 120;

    public static readonly StyledProperty<ObservableCollection<BarChartItem>?> ItemsProperty =
        AvaloniaProperty.Register<BarChart, ObservableCollection<BarChartItem>?>(nameof(Items));

    public static readonly StyledProperty<double> MaxValueProperty =
        AvaloniaProperty.Register<BarChart, double>(nameof(MaxValue));

    public ObservableCollection<BarChartItem>? Items
    {
        get => GetValue(ItemsProperty);
        set => SetValue(ItemsProperty, value);
    }

    public double MaxValue
    {
        get => GetValue(MaxValueProperty);
        set => SetValue(MaxValueProperty, value);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == ItemsProperty)
        {
            if (change.OldValue is ObservableCollection<BarChartItem> oldCol)
                oldCol.CollectionChanged -= OnItemsChanged;
            if (change.NewValue is ObservableCollection<BarChartItem> newCol)
                newCol.CollectionChanged += OnItemsChanged;
            RecalculateHeights();
        }
        else if (change.Property == MaxValueProperty)
        {
            RecalculateHeights();
        }
    }

    private void OnItemsChanged(object? sender, NotifyCollectionChangedEventArgs e)
        => RecalculateHeights();

    private void RecalculateHeights()
    {
        var items = Items;
        if (items is null || items.Count == 0) return;

        var max = MaxValue > 0 ? MaxValue : items.Max(i => i.Value);
        if (max <= 0) max = 1;

        foreach (var item in items)
            item.BarHeight = Math.Max(4, item.Value / max * MaxBarHeight);
    }
}
