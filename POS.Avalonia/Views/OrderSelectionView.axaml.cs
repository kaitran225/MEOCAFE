using System;
using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace POS.Avalonia.Views;

public partial class OrderSelectionView : UserControl
{
    public OrderSelectionView() => InitializeComponent();

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is ViewModels.OrderSelectionViewModel vm)
        {
            vm.PropertyChanged -= OnVmPropertyChanged;
            vm.PropertyChanged += OnVmPropertyChanged;
        }
    }

    private void OnVmPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ViewModels.OrderSelectionViewModel.IsCustomizationOpen)
            && DataContext is ViewModels.OrderSelectionViewModel vm
            && vm.IsCustomizationOpen
            && this.FindControl<Border>("CustomizationDialogPanel") is { } panel)
            panel.Focus();
    }

    private void OnProductDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (DataContext is ViewModels.OrderSelectionViewModel vm && vm.SelectedMenuItem != null)
            vm.AddProductCommand.Execute(vm.SelectedMenuItem);
    }
}
