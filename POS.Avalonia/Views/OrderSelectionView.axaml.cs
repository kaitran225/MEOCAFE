using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace POS.Avalonia.Views;

public partial class OrderSelectionView : UserControl
{
    public OrderSelectionView() => InitializeComponent();

    private void OnProductDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (DataContext is ViewModels.OrderSelectionViewModel vm && vm.SelectedMenuItem != null)
            vm.AddProductCommand.Execute(vm.SelectedMenuItem);
    }
}
