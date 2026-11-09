using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using POS.Avalonia.Services;
using POS.Core.Contracts;
using POS.Core.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
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
    [ObservableProperty] private ObservableCollection<Order> _displayedOrders = new();
    [ObservableProperty] private Order? _selectedOrder;
    [ObservableProperty] private string _searchText = "";
    [ObservableProperty] private DateTime? _filterDateFrom;
    [ObservableProperty] private DateTime? _filterDateTo;
    [ObservableProperty] private int _pageIndex;
    [ObservableProperty] private int _pageSize = 20;
    [ObservableProperty] private int _filteredCount;
    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private bool _isRefundOpen;
    [ObservableProperty] private string _refundAmountText = "";
    [ObservableProperty] private string _refundReason = "";

    public string Title => "Order history";
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)FilteredCount / PageSize) : 0;
    public bool HasNextPage => PageIndex < TotalPages - 1;
    public bool HasPrevPage => PageIndex > 0;
    public string PaginationSummary => FilteredCount > 0
        ? $"{PageIndex * PageSize + 1}-{Math.Min((PageIndex + 1) * PageSize, FilteredCount)} of {FilteredCount}"
        : "0 of 0";

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
            UpdateDisplayedOrders();
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void UpdateDisplayedOrders()
    {
        var q = Orders.AsEnumerable();
        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            var term = SearchText.Trim().ToLowerInvariant();
            q = q.Where(o => (o.Id.ToString().Contains(term)) ||
                o.CreatedAt.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture).Contains(term) ||
                (o.ServiceType?.ToLowerInvariant().Contains(term) == true));
        }
        if (FilterDateFrom.HasValue)
            q = q.Where(o => o.CreatedAt.Date >= FilterDateFrom.Value.Date);
        if (FilterDateTo.HasValue)
            q = q.Where(o => o.CreatedAt.Date <= FilterDateTo.Value.Date);
        var filtered = q.ToList();
        FilteredCount = filtered.Count;
        var page = filtered.Skip(PageIndex * PageSize).Take(PageSize).ToList();
        DisplayedOrders = new ObservableCollection<Order>(page);
        OnPropertyChanged(nameof(TotalPages));
        OnPropertyChanged(nameof(HasNextPage));
        OnPropertyChanged(nameof(HasPrevPage));
        OnPropertyChanged(nameof(PaginationSummary));
        NextPageCommand.NotifyCanExecuteChanged();
        PrevPageCommand.NotifyCanExecuteChanged();
    }

    partial void OnSearchTextChanged(string value) => UpdateDisplayedOrders();
    partial void OnFilterDateFromChanged(DateTime? value) => UpdateDisplayedOrders();
    partial void OnFilterDateToChanged(DateTime? value) => UpdateDisplayedOrders();
    partial void OnPageIndexChanged(int value) => UpdateDisplayedOrders();

    private bool CanNextPage() => HasNextPage;
    private bool CanPrevPage() => HasPrevPage;

    [RelayCommand(CanExecute = nameof(CanNextPage))]
    private void NextPage()
    {
        PageIndex = Math.Min(PageIndex + 1, TotalPages - 1);
        NextPageCommand.NotifyCanExecuteChanged();
        PrevPageCommand.NotifyCanExecuteChanged();
    }

    [RelayCommand(CanExecute = nameof(CanPrevPage))]
    private void PrevPage()
    {
        PageIndex = Math.Max(0, PageIndex - 1);
        NextPageCommand.NotifyCanExecuteChanged();
        PrevPageCommand.NotifyCanExecuteChanged();
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
    private void SelectOrder(Order? order) => SelectedOrder = order;

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
