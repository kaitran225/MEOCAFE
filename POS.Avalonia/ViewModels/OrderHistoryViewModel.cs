using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using POS.Avalonia.Services;
using POS.Core.Contracts;
using POS.Core.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace POS.Avalonia.ViewModels;

public partial class OrderHistoryViewModel : ViewModelBase
{
    private readonly IOrderRepository _orderRepo;
    private readonly IRefundRepository _refundRepo;
    private readonly IReceiptService _receiptService;
    private readonly IPrintService _printService;
    private readonly IKitchenDisplayService _kitchenService;

    [ObservableProperty] private ObservableCollection<Order> _orders = new();
    [ObservableProperty] private Order? _selectedOrder;
    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private bool _isRefundOpen;
    [ObservableProperty] private string _refundAmountText = "";
    [ObservableProperty] private string _refundReason = "";

    public string Title => "Order history";

    public OrderHistoryViewModel(IOrderRepository orderRepo, IRefundRepository refundRepo, IReceiptService receiptService, IPrintService printService, IKitchenDisplayService kitchenService)
    {
        _orderRepo = orderRepo;
        _refundRepo = refundRepo;
        _receiptService = receiptService;
        _printService = printService;
        _kitchenService = kitchenService;
        _ = LoadOrdersAsync();
    }

    [RelayCommand]
    private async Task LoadOrdersAsync()
    {
        IsLoading = true;
        try
        {
            var list = await _orderRepo.GetAllAsync(default).ConfigureAwait(true);
            Orders = new ObservableCollection<Order>(list);
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand(CanExecute = nameof(CanReprint))]
    private async Task ReprintAsync()
    {
        if (SelectedOrder == null) return;
        var receiptData = await _receiptService.BuildReceiptFromOrderAsync(SelectedOrder.Id, "Cash", default).ConfigureAwait(true);
        var text = _receiptService.FormatReceiptAsText(receiptData);
        _printService.PrintReceipt(text);
    }

    private bool CanReprint() => SelectedOrder != null;

    [RelayCommand(CanExecute = nameof(CanReprint))]
    private async Task ReprintKitchenTicketAsync()
    {
        if (SelectedOrder == null) return;
        var display = await _kitchenService.GetOrderDisplayAsync(SelectedOrder.Id, default).ConfigureAwait(true);
        if (display == null) return;
        var text = _kitchenService.FormatKitchenOrderAsText(display);
        _printService.PrintKitchenTicket(text);
    }

    [RelayCommand]
    private void OpenRefund()
    {
        if (SelectedOrder == null) return;
        RefundAmountText = SelectedOrder.TotalPrice.ToString("N0");
        RefundReason = "";
        IsRefundOpen = true;
    }

    [RelayCommand]
    private async Task ConfirmRefundAsync()
    {
        if (SelectedOrder == null) return;
        if (!decimal.TryParse(RefundAmountText, out var amount) || amount <= 0) return;
        await _refundRepo.AddAsync(new POS.Core.Models.Refund { OrderId = SelectedOrder.Id, Amount = amount, Method = "Cash", Reason = RefundReason }, default).ConfigureAwait(true);
        IsRefundOpen = false;
        _ = LoadOrdersAsync();
    }

    [RelayCommand]
    private void CloseRefund() => IsRefundOpen = false;

    partial void OnSelectedOrderChanged(Order? value)
    {
        ReprintCommand.NotifyCanExecuteChanged();
        ReprintKitchenTicketCommand.NotifyCanExecuteChanged();
    }
}
